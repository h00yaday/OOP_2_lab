using Lab3Json.Models;
using Lab3Json.Services;

namespace Lab3Json.Pages;

/// <summary>
/// Окрема форма для додавання та редагування видання.
/// Відкривається модально; ShowAsync повертає заповнене видання або null («Скасувати»).
/// </summary>
public partial class BookEditPage : ContentPage
{
    private readonly Book _book;
    private readonly TaskCompletionSource<Book?> _result = new();

    private BookEditPage(Book? book)
    {
        InitializeComponent();

        // Редагуємо копію: якщо натиснути «Скасувати», оригінал не зміниться
        _book = book?.Clone() ?? new Book { Year = DateTime.Now.Year };
        HeaderLabel.Text = book == null ? "Нове видання" : $"Редагування видання №{book.Id}";
        FillForm();
    }

    public static async Task<Book?> ShowAsync(INavigation navigation, Book? book)
    {
        var page = new BookEditPage(book);
        await navigation.PushModalAsync(page);
        return await page._result.Task;
    }

    private void FillForm()
    {
        TitleEntry.Text = _book.Title;
        AuthorsEntry.Text = _book.AuthorsText;
        TypeEntry.Text = _book.Type;
        YearEntry.Text = _book.Year.ToString();
        PagesEntry.Text = _book.Pages?.ToString();
        PublisherEntry.Text = _book.Publisher;
        AnnotationEditor.Text = _book.Annotation;
        ReadersEditor.Text = ReaderTextConverter.ToText(_book.Readers);
    }

    /// <summary>Переносить значення з полів форми в об'єкт; повертає помилки формату чисел.</summary>
    private List<string> ReadForm()
    {
        var errors = new List<string>();

        _book.Title = TitleEntry.Text?.Trim() ?? string.Empty;
        _book.Authors = (AuthorsEntry.Text ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        _book.Type = TypeEntry.Text?.Trim() ?? string.Empty;
        _book.Publisher = PublisherEntry.Text?.Trim() ?? string.Empty;
        _book.Annotation = AnnotationEditor.Text?.Trim() ?? string.Empty;
        _book.Readers = ReaderTextConverter.FromText(ReadersEditor.Text);

        if (int.TryParse(YearEntry.Text, out var year))
        {
            _book.Year = year;
        }
        else
        {
            errors.Add("Рік має бути цілим числом.");
        }

        if (string.IsNullOrWhiteSpace(PagesEntry.Text))
        {
            _book.Pages = null;
        }
        else if (int.TryParse(PagesEntry.Text, out var pages))
        {
            _book.Pages = pages;
        }
        else
        {
            errors.Add("Кількість сторінок має бути цілим числом.");
        }

        return errors;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var errors = ReadForm();
        errors.AddRange(BookValidator.Validate(_book));

        if (errors.Count > 0)
        {
            ErrorLabel.Text = string.Join(Environment.NewLine, errors.Distinct());
            ErrorLabel.IsVisible = true;
            return;
        }

        _result.TrySetResult(_book);
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _result.TrySetResult(null);
        await Navigation.PopModalAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _result.TrySetResult(null); // форму закрито іншим способом — як «Скасувати»
    }
}
