using System.Text.Json;
using Lab3Json.Models;
using Lab3Json.Pages;
using Lab3Json.Services;

namespace Lab3Json;

/// <summary>
/// Головна сторінка — диспетчер JSON. Обробники подій лише збирають дані з форми
/// і викликають відповідні класи (LibraryRepository, BookSearchService, BookEditPage…),
/// тому антишаблону «магічна кнопка» немає.
/// </summary>
public partial class MainPage : ContentPage
{
	private static readonly (string Text, SortOrder Value)[] SortOptions =
	{
		("за номером", SortOrder.ById),
		("за назвою", SortOrder.ByTitle),
		("за роком (нові спочатку)", SortOrder.ByYearDescending),
		("за кількістю читачів", SortOrder.ByReadersDescending),
	};

	private readonly LibraryRepository _repository = new();
	private SearchCriteria _criteria = new();

	public MainPage()
	{
		InitializeComponent();

		SortPicker.ItemsSource = SortOptions.Select(o => o.Text).ToList();
		SortPicker.SelectedIndex = 0;

		ExitService.HasUnsavedChanges = () => _repository.HasUnsavedChanges;
		RefreshTable();
	}

	private Book? SelectedBook => BooksView.SelectedItem as Book;

	// ---------------- Файл ----------------

	private async void OnNewClicked(object? sender, EventArgs e)
	{
		if (await ConfirmDiscardChangesAsync())
		{
			_repository.CreateNew();
			AfterFileChanged();
		}
	}

	private async void OnOpenClicked(object? sender, EventArgs e)
	{
		if (!await ConfirmDiscardChangesAsync())
		{
			return;
		}

		var path = await FileDialogService.PickJsonToOpenAsync();
		if (path != null)
		{
			await OpenFileAsync(path);
		}
	}

	private async void OnOpenSampleClicked(object? sender, EventArgs e)
	{
		if (await ConfirmDiscardChangesAsync())
		{
			await OpenFileAsync(await SampleFilesProvider.GetLocalCopyAsync(SampleFilesProvider.SampleJson));
		}
	}

	private async Task OpenFileAsync(string path)
	{
		try
		{
			await _repository.OpenAsync(path);
			AfterFileChanged();
		}
		catch (JsonException ex)
		{
			await DisplayAlertAsync("Помилка формату JSON",
				$"Файл не є коректним JSON для бібліотеки:\n{ex.Message}", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Помилка", $"Не вдалося відкрити файл:\n{ex.Message}", "OK");
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		if (_repository.FilePath == null)
		{
			await SaveAsAsync();
			return;
		}

		await RunSafeAsync(async () =>
		{
			await _repository.SaveAsync();
			await AfterSavedAsync();
		});
	}

	private async void OnSaveAsClicked(object? sender, EventArgs e) => await SaveAsAsync();

	/// <summary>Стандартне вікно Windows «Зберегти як» — користувач сам вибирає папку та ім'я.</summary>
	private async Task SaveAsAsync()
	{
		var suggestedName = _repository.FilePath != null ? Path.GetFileName(_repository.FilePath) : "library.json";
		var path = await FileDialogService.PickJsonToSaveAsync(suggestedName);
		if (path == null)
		{
			return;
		}

		await RunSafeAsync(async () =>
		{
			await _repository.SaveAsAsync(path);
			await AfterSavedAsync();
		});
	}

	private async Task AfterSavedAsync()
	{
		UpdateStatus();
		var path = _repository.FilePath!;
		if (await DisplayAlertAsync("Збереження",
				$"Дані збережено у файл:\n{path}\n\nПоказати файл у Провіднику?", "Так", "Ні"))
		{
			FileDialogService.ShowInExplorer(path);
		}
	}

	private void OnShowInExplorerClicked(object? sender, EventArgs e)
	{
		if (_repository.FilePath != null)
		{
			FileDialogService.ShowInExplorer(_repository.FilePath);
		}
	}

	/// <summary>Якщо є незбережені зміни — питає, чи можна їх втратити.</summary>
	private async Task<bool> ConfirmDiscardChangesAsync()
	{
		return !_repository.HasUnsavedChanges ||
			   await DisplayAlertAsync("Незбережені зміни",
				   "Є незбережені зміни, вони будуть втрачені. Продовжити?", "Так", "Ні");
	}

	private void AfterFileChanged()
	{
		TypePicker.ItemsSource = _repository.GetBookTypes();
		ResetFilters();
		RefreshTable();
	}

	// ---------------- Додавання / редагування / видалення ----------------

	private async void OnAddClicked(object? sender, EventArgs e)
	{
		var book = await BookEditPage.ShowAsync(Navigation, null);
		if (book != null)
		{
			_repository.Add(book);
			AfterDataChanged(book);
		}
	}

	private async void OnEditClicked(object? sender, EventArgs e)
	{
		if (SelectedBook is not Book selected)
		{
			await DisplayAlertAsync("Редагування", "Спочатку виберіть видання в таблиці.", "OK");
			return;
		}

		var edited = await BookEditPage.ShowAsync(Navigation, selected);
		if (edited != null)
		{
			_repository.Update(edited);
			AfterDataChanged(edited);
		}
	}

	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (SelectedBook is not Book selected)
		{
			await DisplayAlertAsync("Видалення", "Спочатку виберіть видання в таблиці.", "OK");
			return;
		}

		if (await DisplayAlertAsync("Видалення", $"Видалити «{selected.Title}»?", "Так", "Ні"))
		{
			_repository.Delete(selected);
			AfterDataChanged(null);
		}
	}

	private void AfterDataChanged(Book? toSelect)
	{
		TypePicker.ItemsSource = _repository.GetBookTypes();
		TypePicker.SelectedItem = _criteria.Type;
		RefreshTable();
		if (toSelect != null)
		{
			BooksView.SelectedItem = (BooksView.ItemsSource as List<Book>)?.FirstOrDefault(b => b.Id == toSelect.Id);
		}
	}

	// ---------------- Пошук ----------------

	private async void OnSearchClicked(object? sender, EventArgs e)
	{
		var criteria = BuildCriteria(out var error);
		if (error != null)
		{
			await DisplayAlertAsync("Пошук", error, "OK");
			return;
		}

		_criteria = criteria;
		RefreshTable();
	}

	private void OnResetClicked(object? sender, EventArgs e)
	{
		ResetFilters();
		RefreshTable();
	}

	private SearchCriteria BuildCriteria(out string? error)
	{
		error = null;
		var criteria = new SearchCriteria
		{
			Author = AuthorEntry.Text,
			Type = TypePicker.SelectedItem as string,
			Reader = ReaderEntry.Text,
			Keyword = KeywordEntry.Text,
			Sort = SortOptions[Math.Max(SortPicker.SelectedIndex, 0)].Value
		};

		if (!TryParseYear(YearFromEntry.Text, out var from) || !TryParseYear(YearToEntry.Text, out var to))
		{
			error = "Рік має бути цілим числом.";
			return criteria;
		}

		criteria.YearFrom = from;
		criteria.YearTo = to;
		return criteria;
	}

	private static bool TryParseYear(string? text, out int? year)
	{
		year = null;
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}

		if (int.TryParse(text, out var value))
		{
			year = value;
			return true;
		}

		return false;
	}

	private void ResetFilters()
	{
		AuthorEntry.Text = YearFromEntry.Text = YearToEntry.Text = ReaderEntry.Text = KeywordEntry.Text = string.Empty;
		TypePicker.SelectedIndex = -1;
		SortPicker.SelectedIndex = 0;
		_criteria = new SearchCriteria();
	}

	// ---------------- Відображення ----------------

	/// <summary>Оновлює таблицю: застосовує поточні критерії LINQ до списку видань.</summary>
	private void RefreshTable()
	{
		var books = BookSearchService.Search(_repository.Books, _criteria);
		BooksView.ItemsSource = books;
		BooksView.SelectedItem = null;

		CountLabel.Text = _criteria.IsEmpty
			? $"Усього видань: {books.Count}"
			: $"Знайдено: {books.Count} з {_repository.Books.Count}   (фільтр: {_criteria})";

		UpdateStatus();
	}

	private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		DetailsLabel.Text = SelectedBook?.DetailsText ?? "Виберіть рядок таблиці, щоб побачити анотацію та читачів.";
	}

	private void UpdateStatus()
	{
		var file = _repository.FilePath ?? "новий файл (ще не збережено)";
		var changes = _repository.HasUnsavedChanges ? "   ● є незбережені зміни" : string.Empty;
		StatusLabel.Text = $"Файл: {file}{changes}";
	}

	private async Task RunSafeAsync(Func<Task> action)
	{
		try
		{
			await action();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Помилка", ex.Message, "OK");
		}
	}

	// ---------------- Довідка / Про програму / Вихід ----------------

	private async void OnHelpClicked(object? sender, EventArgs e)
	{
		await DisplayAlertAsync("Довідка",
			"1. Відкрийте JSON-файл (Файл → Відкрити) або приклад library.json.\n" +
			"2. Список видань показано в таблиці; клік по рядку — анотація і читачі.\n" +
			"3. «Додати» / «Редагувати» відкривають окрему форму; «Видалити» — з підтвердженням.\n" +
			"4. Пошук (LINQ): автор, вид видання, роки з–по, читач, ключове слово, сортування.\n" +
			"   Критерії можна комбінувати; «Скинути фільтри» показує весь список.\n" +
			"5. «Зберегти» записує дані у той самий файл, «Зберегти як» — у новий.\n" +
			"6. Під час виходу програма попередить про незбережені зміни.",
			"OK");
	}

	private async void OnAboutClicked(object? sender, EventArgs e) => await Navigation.PushModalAsync(new AboutPage());

	private async void OnExitClicked(object? sender, EventArgs e) => await ExitService.RequestExitAsync();
}
