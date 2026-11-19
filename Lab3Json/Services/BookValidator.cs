using Lab3Json.Models;

namespace Lab3Json.Services;

/// <summary>Перевірка даних видання перед збереженням у список.</summary>
public static class BookValidator
{
    public const int MinYear = 1450;

    /// <summary>Повертає список помилок; порожній список — дані коректні.</summary>
    public static List<string> Validate(Book book)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(book.Title))
        {
            errors.Add("Вкажіть назву видання.");
        }

        if (book.Authors.Count == 0)
        {
            errors.Add("Вкажіть хоча б одного автора.");
        }

        if (string.IsNullOrWhiteSpace(book.Type))
        {
            errors.Add("Вкажіть вид видання.");
        }

        if (book.Year < MinYear || book.Year > DateTime.Now.Year)
        {
            errors.Add($"Рік має бути від {MinYear} до {DateTime.Now.Year}.");
        }

        if (book.Pages is <= 0)
        {
            errors.Add("Кількість сторінок має бути додатною.");
        }

        if (book.Readers.Any(r => string.IsNullOrWhiteSpace(r.FullName)))
        {
            errors.Add("У кожного читача має бути ПІБ.");
        }

        return errors;
    }
}
