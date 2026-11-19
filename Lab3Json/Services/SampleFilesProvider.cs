namespace Lab3Json.Services;

/// <summary>
/// Копіює вбудований файл-приклад (Resources/Raw/library.json) у папку
/// «Документи\Lab3Json», щоб з ним можна було працювати як зі звичайним файлом
/// і було видно, куди він зберігається.
/// </summary>
public static class SampleFilesProvider
{
    public const string SampleJson = "library.json";

    public static string SampleDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Lab3Json");

    public static async Task<string> GetLocalCopyAsync(string assetName)
    {
        Directory.CreateDirectory(SampleDirectory);
        var targetPath = Path.Combine(SampleDirectory, assetName);

        await using var source = await FileSystem.OpenAppPackageFileAsync(assetName);
        await using var target = File.Create(targetPath);
        await source.CopyToAsync(target);

        return targetPath;
    }
}
