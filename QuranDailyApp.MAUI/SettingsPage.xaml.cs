using QuranDailyApp.Core.Interfaces;

namespace QuranDailyApp.MAUI;

public partial class SettingsPage : ContentPage
{
    private readonly INotificationService _notificationService;

    public SettingsPage(INotificationService notificationService)
    {
        InitializeComponent();
        _notificationService = notificationService;
        LoadSettings();
    }

    private async void LoadSettings()
    {
        LoadCurrentTheme();
        // TODO: Add notification settings UI
        await InitializeNotifications();
    }

    private async Task InitializeNotifications()
    {
        // Request permission on app start
        var hasPermission = await _notificationService.RequestPermissionAsync();
        
        // Set up default daily notification at 8 AM if not already configured
        if (hasPermission && !_notificationService.IsNotificationEnabled())
        {
            var defaultTime = new TimeSpan(8, 0, 0); // 8:00 AM
            await _notificationService.ScheduleDailyNotificationAsync(defaultTime, true);
        }
    }

    private void LoadCurrentTheme()
    {
        var theme = Preferences.Get("Theme", "System");
        
        switch (theme)
        {
            case "Light":
                LightThemeRadio.IsChecked = true;
                break;
            case "Dark":
                DarkThemeRadio.IsChecked = true;
                break;
            default:
                SystemThemeRadio.IsChecked = true;
                break;
        }
    }

    private void OnThemeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        var radioButton = sender as RadioButton;
        string theme = "";

        if (radioButton == LightThemeRadio)
        {
            theme = "Light";
            Application.Current!.UserAppTheme = AppTheme.Light;
        }
        else if (radioButton == DarkThemeRadio)
        {
            theme = "Dark";
            Application.Current!.UserAppTheme = AppTheme.Dark;
        }
        else if (radioButton == SystemThemeRadio)
        {
            theme = "System";
            Application.Current!.UserAppTheme = AppTheme.Unspecified;
        }

        Preferences.Set("Theme", theme);
    }
}