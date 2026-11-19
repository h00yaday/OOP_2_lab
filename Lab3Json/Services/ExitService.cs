namespace Lab3Json.Services;

/// <summary>Вихід з програми з підтвердженням «Так» / «Ні» (і попередженням про незбережені зміни).</summary>
public static class ExitService
{
    private static bool _exitConfirmed;
    private static bool _dialogOpen;

    /// <summary>Чи вже підтверджено вихід (щоб не питати вдруге при закритті вікна).</summary>
    public static bool ExitConfirmed => _exitConfirmed;

    /// <summary>Сторінка повідомляє, чи є незбережені зміни.</summary>
    public static Func<bool>? HasUnsavedChanges { get; set; }

    public static async Task RequestExitAsync()
    {
        if (_dialogOpen)
        {
            return;
        }

        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page == null)
        {
            return;
        }

        var message = HasUnsavedChanges?.Invoke() == true
            ? "Є незбережені зміни, вони будуть втрачені.\nЧи дійсно ви хочете завершити роботу з програмою?"
            : "Чи дійсно ви хочете завершити роботу з програмою?";

        _dialogOpen = true;
        try
        {
            if (await page.DisplayAlertAsync("Підтвердження", message, "Так", "Ні"))
            {
                _exitConfirmed = true;
                Application.Current!.Quit();
            }
        }
        finally
        {
            _dialogOpen = false;
        }
    }
}
