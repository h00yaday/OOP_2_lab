using System.Text.Json.Serialization;

namespace Lab3Json.Models;

/// <summary>Читач, який взяв видання (вкладений об'єкт у JSON).</summary>
public class Reader
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("faculty")]
    public string Faculty { get; set; } = string.Empty;

    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    public Reader Clone() => (Reader)MemberwiseClone();

    public override string ToString() => $"{FullName} ({Position}, {Faculty}, кафедра {Department})";
}
