using Microsoft.Extensions.Logging;
using QuranDailyApp.Core.Services;
using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.MAUI.Services;
using Plugin.LocalNotification;
using INotificationService = QuranDailyApp.Core.Interfaces.INotificationService;

namespace QuranDailyApp.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add caching services
        builder.Services.AddMemoryCache();

        // Add file service for MAUI
        builder.Services.AddScoped<IFileService, MauiFileService>();

        // Add notification service
        builder.Services.AddSingleton<INotificationService, MauiNotificationService>();

        // Add core services
        builder.Services.AddScoped<QuranService>();

        // Register pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
