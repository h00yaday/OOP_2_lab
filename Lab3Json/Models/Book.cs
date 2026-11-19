using System.Text;
using System.Text.Json.Serialization;

namespace Lab3Json.Models;

/// <summary>
/// Видання кафедральної бібліотеки — один елемент списку "books" у JSON.
/// Атрибути [JsonPropertyName] задають імена полів у файлі,
/// [JsonIgnore] — обчислювані властивості, які у файл не пишуться.
/// </summary>
public class Book
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; } = new();

    [JsonPropertyName("annotation")]
    public string Annotation { get; set; } = string.Empty;

    /// <summary>Кваліфікаційна ознака видання: підручник, посібник, монографія…</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = string.Empty;

    /// <summary>Кількість сторінок; може бути невідомою (null — у файл не пишеться).</summary>
    [JsonPropertyName("pages")]
    public int? Pages { get; set; }

    [JsonPropertyName("readers")]
    public List<Reader> Readers { get; set; } = new();

    [JsonIgnore]
    public string AuthorsText => string.Join(", ", Authors);

    [JsonIgnore]
    public int ReadersCount => Readers.Count;

    /// <summary>Глибока копія — форма редагування працює з копією, тому «Скасувати» нічого не змінює.</summary>
    public Book Clone()
    {
        var copy = (Book)MemberwiseClone();
        copy.Authors = new List<string>(Authors);
        copy.Readers = Readers.Select(r => r.Clone()).ToList();
        return copy;
    }

    /// <summary>Детальна інформація для панелі під таблицею.</summary>
    [JsonIgnore]
    public string DetailsText
    {
        get
        {
            var sb = new StringBuilder();
            sb.AppendLine($"«{Title}» — {AuthorsText}");
            sb.AppendLine($"{Type}, {Publisher}, {Year} р.{(Pages.HasValue ? $", {Pages} с." : string.Empty)}");
            if (!string.IsNullOrWhiteSpace(Annotation))
            {
                sb.AppendLine($"Анотація: {Annotation}");
            }

            sb.AppendLine(Readers.Count == 0 ? "Читачів немає." : "Читачі:");
            foreach (var reader in Readers)
            {
                sb.AppendLine($"   • {reader}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
