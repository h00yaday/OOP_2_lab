namespace Lab3Json.Models;

/// <summary>Порядок сортування результатів пошуку.</summary>
public enum SortOrder
{
    ById,
    ByTitle,
    ByYearDescending,
    ByReadersDescending
}

/// <summary>
/// Критерії пошуку, які задає користувач. Порожній (null) критерій не враховується,
/// тож можна шукати за будь-якою комбінацією.
/// </summary>
public class SearchCriteria
{
    /// <summary>Частина прізвища автора.</summary>
    public string? Author { get; set; }

    /// <summary>Вид видання (точний збіг, значення з файлу).</summary>
    public string? Type { get; set; }

    public int? YearFrom { get; set; }

    public int? YearTo { get; set; }

    /// <summary>Частина ПІБ читача.</summary>
    public string? Reader { get; set; }

    /// <summary>Ключове слово в назві або анотації.</summary>
    public string? Keyword { get; set; }

    public SortOrder Sort { get; set; } = SortOrder.ById;

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Author) && string.IsNullOrWhiteSpace(Type) &&
        YearFrom == null && YearTo == null &&
        string.IsNullOrWhiteSpace(Reader) && string.IsNullOrWhiteSpace(Keyword);

    public override string ToString()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(Author)) parts.Add($"автор містить «{Author}»");
        if (!string.IsNullOrWhiteSpace(Type)) parts.Add($"вид = {Type}");
        if (YearFrom != null || YearTo != null) parts.Add($"рік {YearFrom?.ToString() ?? "…"}–{YearTo?.ToString() ?? "…"}");
        if (!string.IsNullOrWhiteSpace(Reader)) parts.Add($"читач містить «{Reader}»");
        if (!string.IsNullOrWhiteSpace(Keyword)) parts.Add($"слово «{Keyword}»");
        return parts.Count == 0 ? "без фільтрів" : string.Join("; ", parts);
    }
}
