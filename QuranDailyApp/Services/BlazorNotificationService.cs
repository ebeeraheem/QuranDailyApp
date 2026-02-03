using QuranDailyApp.Core.Interfaces;

namespace QuranDailyApp.Services;

public class BlazorNotificationService : INotificationService
{
    // Blazor Web app doesn't support local notifications, so these are no-op implementations
    public Task<bool> RequestPermissionAsync() => Task.FromResult(false);
    
    public Task ScheduleDailyNotificationAsync(TimeSpan notificationTime, bool enabled) => Task.CompletedTask;
    
    public Task CancelDailyNotificationAsync() => Task.CompletedTask;
    
    public bool IsNotificationEnabled() => false;
    
    public TimeSpan GetNotificationTime() => new(8, 0, 0);
    
    public void SetNotificationTime(TimeSpan time) { }
    
    public void SetNotificationEnabled(bool enabled) { }
    
    public bool IsFirstLaunch() => false;

    // Toast methods - no-op implementations for Blazor
    public Task ShowToastAsync(string message) => Task.CompletedTask;

    public Task ShowSuccessToastAsync(string message) => Task.CompletedTask;

    public Task ShowErrorToastAsync(string message) => Task.CompletedTask;
}