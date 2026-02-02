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
        // The first-launch setup is now handled in MainPage
        // Here we just load the current settings for the UI
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