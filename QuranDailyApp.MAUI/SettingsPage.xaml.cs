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
        await InitializeAndLoadNotificationSettings();
    }

    private async Task InitializeAndLoadNotificationSettings()
    {
        // Request permission on first load
        var hasPermission = await _notificationService.RequestPermissionAsync();
        
        // If no permission and notifications aren't configured, set up defaults
        if (hasPermission && !_notificationService.IsNotificationEnabled())
        {
            var defaultTime = new TimeSpan(8, 0, 0); // 8:00 AM
            await _notificationService.ScheduleDailyNotificationAsync(defaultTime, true);
        }
        
        // Load current settings for UI (when UI controls are added)
        LoadNotificationSettingsForUI();
    }
    
    private void LoadNotificationSettingsForUI()
    {
        // Load current settings for UI controls (when XAML controls are added)
        var enabled = _notificationService.IsNotificationEnabled();
        var time = _notificationService.GetNotificationTime();
        
        System.Diagnostics.Debug.WriteLine($"Notification Settings - Enabled: {enabled}, Time: {time}");
        
        NotificationSwitch.IsToggled = enabled;
        NotificationTimePicker.Time = time;
        NotificationTimeGrid.IsVisible = enabled;
        PermissionStatusLabel.IsVisible = false;
    }

    private async Task ToggleNotifications(bool enabled)
    {
        if (enabled)
        {
            var hasPermission = await _notificationService.RequestPermissionAsync();
            if (!hasPermission)
            {
                await DisplayAlert("Permission Required", 
                    "To receive daily notifications, please enable notification permissions in your device settings.", 
                    "OK");
                
                NotificationSwitch.IsToggled = false;
                PermissionStatusLabel.Text = "?? Notification permission required. Please enable in device settings.";
                PermissionStatusLabel.IsVisible = true;
                return;
            }
            
            PermissionStatusLabel.IsVisible = false;

            var currentTime = _notificationService.GetNotificationTime();
            await _notificationService.ScheduleDailyNotificationAsync(currentTime, true);

            NotificationTimeGrid.IsVisible = true;
        }
        else
        {
            await _notificationService.CancelDailyNotificationAsync();
            _notificationService.SetNotificationEnabled(false);
            
            NotificationTimeGrid.IsVisible = false;
            PermissionStatusLabel.IsVisible = false;
        }
    }
    
    private async Task UpdateNotificationTime(TimeSpan newTime)
    {
        if (_notificationService.IsNotificationEnabled())
        {
            await _notificationService.ScheduleDailyNotificationAsync(newTime, true);
        }
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

    // Notification event handlers (to be connected when XAML controls are added)
    private async void OnNotificationToggled(object sender, ToggledEventArgs e)
    {
        await ToggleNotifications(e.Value);
    }

    private async void OnNotificationTimeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == TimePicker.TimeProperty.PropertyName && sender is TimePicker timePicker)
        {
            await UpdateNotificationTime(timePicker.Time ?? TimeSpan.Zero);
        }
    }
}