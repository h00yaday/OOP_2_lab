namespace Lab3Json.Services;

/// <summary>
/// Стандартні вікна Windows для вибору файлу: «Відкрити» і «Зберегти як».
/// Користувач сам бачить і вибирає папку, куди зберігається файл.
/// </summary>
public static class FileDialogService
{
    private static readonly FilePickerFileType JsonFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        [DevicePlatform.WinUI] = new[] { ".json" },
    });

    /// <summary>Вікно «Відкрити»; повертає шлях або null, якщо скасовано.</summary>
    public static async Task<string?> PickJsonToOpenAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Виберіть JSON-файл",
            FileTypes = JsonFileType
        });
        return result?.FullPath;
    }

    /// <summary>Вікно «Зберегти як»; повертає шлях або null, якщо скасовано.</summary>
    public static async Task<string?> PickJsonToSaveAsync(string suggestedName)
    {
#if WINDOWS
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = Path.GetFileNameWithoutExtension(suggestedName)
        };
        picker.FileTypeChoices.Add("JSON-файл", new List<string> { ".json" });

        // Для застосунку без пакета вікно треба прив'язати до головного вікна програми
        var window = (MauiWinUIWindow)Application.Current!.Windows[0].Handler!.PlatformView!;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, window.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        return file?.Path;
#else
        await Task.CompletedTask;
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), suggestedName);
#endif
    }

    /// <summary>Відкриває Провідник з виділеним файлом.</summary>
    public static void ShowInExplorer(string path)
    {
#if WINDOWS
        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
#endif
    }
}
