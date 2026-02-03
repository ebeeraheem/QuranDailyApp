using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;
using QuranDailyApp.Core.Services;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using INotificationService = QuranDailyApp.Core.Interfaces.INotificationService;

namespace QuranDailyApp.MAUI.Services;

public partial class MauiNotificationService(QuranService quranService) : INotificationService
{
    private const string NOTIFICATION_ENABLED_KEY = "NotificationEnabled";
    private const string NOTIFICATION_TIME_KEY = "NotificationTime";
    private const int DAILY_NOTIFICATION_ID = 1001;

    public async Task<bool> RequestPermissionAsync()
    {
        try
        {
            return await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error requesting notification permission: {ex.Message}");
            return false;
        }
    }

    public async Task ScheduleDailyNotificationAsync(TimeSpan notificationTime, bool enabled)
    {
        if (!enabled)
        {
            await CancelDailyNotificationAsync();
            return;
        }

        try
        {
            // Cancel existing notifications first
            await CancelDailyNotificationAsync();

            // Get today's verse for the notification
            var todaysVerse = await quranService.GetDailyAyahAsync();
            
            // Create the notification
            var notification = new NotificationRequest
            {
                NotificationId = DAILY_NOTIFICATION_ID,
                Title = "Quran Daily 📖",
                Subtitle = $"{todaysVerse.SurahName} ({todaysVerse.SurahNumber}:{(int)todaysVerse.AyahNumber})",
                Description = GetTruncatedTranslation(todaysVerse.Translation),
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Today.Add(notificationTime),
                    NotifyRepeatInterval = TimeSpan.FromDays(1),
                    RepeatType = NotificationRepeat.Daily
                }
            };

            // Schedule the notification
            await LocalNotificationCenter.Current.Show(notification);

            // Save settings
            SetNotificationEnabled(enabled);
            SetNotificationTime(notificationTime);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error scheduling notification: {ex.Message}");
        }
    }

    public Task CancelDailyNotificationAsync()
    {
        try
        {
            LocalNotificationCenter.Current.Cancel(DAILY_NOTIFICATION_ID);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error canceling notification: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    public bool IsNotificationEnabled()
    {
        return Preferences.Get(NOTIFICATION_ENABLED_KEY, false);
    }

    public TimeSpan GetNotificationTime()
    {
        var timeString = Preferences.Get(NOTIFICATION_TIME_KEY, "08:00:00");
        return TimeSpan.TryParse(timeString, CultureInfo.InvariantCulture, out var time)
            ? time : new TimeSpan(8, 0, 0);
    }

    public void SetNotificationEnabled(bool enabled)
    {
        Preferences.Set(NOTIFICATION_ENABLED_KEY, enabled);
    }

    public void SetNotificationTime(TimeSpan time)
    {
        Preferences.Set(NOTIFICATION_TIME_KEY, time.ToString());
    }

    public bool IsFirstLaunch()
    {
        return !Preferences.ContainsKey(NOTIFICATION_ENABLED_KEY);
    }

    private static string GetTruncatedTranslation(string translation, int maxLength = 100)
    {
        if (string.IsNullOrEmpty(translation))
            return "Tap to read today's verse";

        // Remove any footnote numbers (digits after text)
        var cleanTranslation = FootnoteNumbersRegex().Replace(translation, "").Trim();
        
        if (cleanTranslation.Length <= maxLength)
            return cleanTranslation;

        var truncated = cleanTranslation[..maxLength];
        var lastSpace = truncated.LastIndexOf(' ');
        
        return lastSpace > 0 ? truncated[..lastSpace] + "..." : truncated + "...";
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex FootnoteNumbersRegex();

    // Toast notification methods
    public static async Task ShowToastAsync(string message)
    {
        try
        {
            var toast = Toast.Make(message, ToastDuration.Long);
            await toast.Show();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error showing toast: {ex.Message}");
        }
    }

    public static async Task ShowSuccessToastAsync(string message)
    {
        try
        {
            var toast = Toast.Make($"{message}", ToastDuration.Long);
            await toast.Show();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error showing success toast: {ex.Message}");
        }
    }

    public static async Task ShowErrorToastAsync(string message)
    {
        try
        {
            var toast = Toast.Make($"{message}", ToastDuration.Long);
            await toast.Show();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error showing error toast: {ex.Message}");
        }
    }
}