using Lab3Json.Models;

namespace Lab3Json.Services;

/// <summary>
/// Перетворення списку читачів у текст для форми редагування і назад.
/// Один рядок — один читач: «ПІБ; факультет; кафедра; посада».
/// </summary>
public static class ReaderTextConverter
{
    private const char Separator = ';';

    public static string ToText(IEnumerable<Reader> readers) =>
        string.Join(Environment.NewLine,
            readers.Select(r => $"{r.FullName}{Separator} {r.Faculty}{Separator} {r.Department}{Separator} {r.Position}"));

    public static List<Reader> FromText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<Reader>();
        }

        return text.Split('\n', '\r')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Split(Separator).Select(p => p.Trim()).ToArray())
            .Select(parts => new Reader
            {
                FullName = parts.ElementAtOrDefault(0) ?? string.Empty,
                Faculty = parts.ElementAtOrDefault(1) ?? string.Empty,
                Department = parts.ElementAtOrDefault(2) ?? string.Empty,
                Position = parts.ElementAtOrDefault(3) ?? string.Empty
            })
            .ToList();
    }
}
