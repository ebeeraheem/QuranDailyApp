namespace QuranDailyApp.Core.Interfaces;

public interface INotificationService
{
    Task<bool> RequestPermissionAsync();
    Task ScheduleDailyNotificationAsync(TimeSpan notificationTime, bool enabled);
    Task CancelDailyNotificationAsync();
    bool IsNotificationEnabled();
    TimeSpan GetNotificationTime();
    void SetNotificationTime(TimeSpan time);
    void SetNotificationEnabled(bool enabled);
    bool IsFirstLaunch();
    
    // Toast notification methods
    Task ShowToastAsync(string message);
    Task ShowSuccessToastAsync(string message);
    Task ShowErrorToastAsync(string message);
}