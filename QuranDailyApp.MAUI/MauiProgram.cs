using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Core.Services;
using QuranDailyApp.MAUI.Services;
using INotificationService = QuranDailyApp.Core.Interfaces.INotificationService;

namespace QuranDailyApp.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add caching services
        builder.Services.AddMemoryCache();

        // Add custom services
        builder.Services.AddScoped<IFileService, MauiFileService>();
        builder.Services.AddSingleton<INotificationService, MauiNotificationService>();
        builder.Services.AddSingleton<IBookmarkService, MauiBookmarkService>();
        builder.Services.AddScoped<QuranService>();

        // Register pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddTransient<BookmarksPage>();
        builder.Services.AddTransient<VerseViewerPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
