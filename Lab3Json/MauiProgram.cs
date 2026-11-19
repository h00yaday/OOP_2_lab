using Lab3Json.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Lab3Json;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureLifecycleEvents(events =>
			{
#if WINDOWS
				// Підтвердження виходу і при закритті вікна хрестиком
				events.AddWindows(windows => windows.OnWindowCreated(window =>
				{
					window.AppWindow.Closing += async (_, args) =>
					{
						if (ExitService.ExitConfirmed)
						{
							return;
						}

						args.Cancel = true;
						await ExitService.RequestExitAsync();
					};
				}));
#endif
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
