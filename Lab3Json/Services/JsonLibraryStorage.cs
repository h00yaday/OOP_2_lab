using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lab3Json.Models;

namespace Lab3Json.Services;

/// <summary>
/// Серіалізація / десеріалізація бібліотеки у формат JSON (System.Text.Json).
/// Лише читає і пише файл — нічого не знає про інтерфейс.
/// </summary>
public static class JsonLibraryStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,                                   // «гарний» JSON з відступами
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,  // кирилиця і апостроф як є, без і
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // pages: null не пишемо
        ReadCommentHandling = JsonCommentHandling.Skip,         // коментарі у файлі дозволені
        AllowTrailingCommas = true
    };

    /// <summary>Десеріалізація: JSON-файл → об'єкт Library.</summary>
    /// <exception cref="JsonException">Файл не є коректним JSON.</exception>
    public static async Task<Library> LoadAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var library = await JsonSerializer.DeserializeAsync<Library>(stream, Options)
                      ?? throw new JsonException("Файл порожній.");
        library.Books ??= new List<Book>();
        return library;
    }

    /// <summary>Серіалізація: об'єкт Library → JSON-файл.</summary>
    public static async Task SaveAsync(Library library, string path)
    {
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, library, Options);
    }
}
