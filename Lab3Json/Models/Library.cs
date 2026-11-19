using System.Text.Json.Serialization;

namespace Lab3Json.Models;

/// <summary>Кореневий об'єкт JSON-файлу: бібліотека кафедри зі списком видань.</summary>
public class Library
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "Кафедральна бібліотека";

    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;

    [JsonPropertyName("books")]
    public List<Book> Books { get; set; } = new();
}
