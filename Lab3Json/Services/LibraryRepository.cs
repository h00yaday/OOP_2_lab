using Lab3Json.Models;

namespace Lab3Json.Services;

/// <summary>
/// Дані, з якими працює програма: поточна бібліотека, шлях до файлу
/// та ознака незбережених змін. Додавання / редагування / видалення — тут.
/// </summary>
public class LibraryRepository
{
    public Library Library { get; private set; } = new();

    /// <summary>Шлях до відкритого файлу (null — файл ще не збережено).</summary>
    public string? FilePath { get; private set; }

    /// <summary>Чи є зміни, яких ще немає у файлі.</summary>
    public bool HasUnsavedChanges { get; private set; }

    public IReadOnlyList<Book> Books => Library.Books;

    public void CreateNew()
    {
        Library = new Library();
        FilePath = null;
        HasUnsavedChanges = false;
    }

    public async Task OpenAsync(string path)
    {
        Library = await JsonLibraryStorage.LoadAsync(path);
        FilePath = path;
        HasUnsavedChanges = false;
    }

    /// <summary>Зберегти у той самий файл, з якого відкрито.</summary>
    public async Task SaveAsync() => await SaveAsAsync(FilePath ?? throw new InvalidOperationException("Файл ще не вибрано."));

    public async Task SaveAsAsync(string path)
    {
        await JsonLibraryStorage.SaveAsync(Library, path);
        FilePath = path;
        HasUnsavedChanges = false;
    }

    public void Add(Book book)
    {
        book.Id = Library.Books.Count == 0 ? 1 : Library.Books.Max(b => b.Id) + 1;
        Library.Books.Add(book);
        HasUnsavedChanges = true;
    }

    /// <summary>Замінює видання з тим самим Id на відредаговану копію.</summary>
    public void Update(Book edited)
    {
        var index = Library.Books.FindIndex(b => b.Id == edited.Id);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Видання з Id={edited.Id} не знайдено.");
        }

        Library.Books[index] = edited;
        HasUnsavedChanges = true;
    }

    public void Delete(Book book)
    {
        if (Library.Books.RemoveAll(b => b.Id == book.Id) > 0)
        {
            HasUnsavedChanges = true;
        }
    }

    /// <summary>Значення «виду видання», присутні у файлі, — для списку в критеріях пошуку.</summary>
    public List<string> GetBookTypes() =>
        Library.Books.Select(b => b.Type)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .OrderBy(t => t)
            .ToList();
}
