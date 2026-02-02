using QuranDailyApp.Core.Models;
using QuranDailyApp.Core.Services;
using QuranDailyApp.Core.Interfaces;

namespace QuranDailyApp.MAUI;

public partial class MainPage : ContentPage
{
    private readonly QuranService _quranService;
    private readonly INotificationService _notificationService;
    private AyahDisplay? _currentAyah;
    private bool _isLoading = false;

    public MainPage(QuranService quranService, INotificationService notificationService)
    {
        InitializeComponent();
        _quranService = quranService;
        _notificationService = notificationService;
        
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        // Handle first-launch notification setup
        await InitializeFirstLaunchNotifications();
        
        // Load the daily verse
        await LoadDailyAyah();
        UpdateDate();
    }

    private async Task InitializeFirstLaunchNotifications()
    {
        try
        {
            // Check if this is the first launch
            if (_notificationService.IsFirstLaunch())
            {
                // First launch - request permission and set up default
                var hasPermission = await _notificationService.RequestPermissionAsync();
                
                if (hasPermission)
                {
                    // Set up default 8 AM daily notifications
                    var defaultTime = new TimeSpan(8, 0, 0);
                    await _notificationService.ScheduleDailyNotificationAsync(defaultTime, true);
                    
                    // Show a friendly message to user
                    await DisplayAlert("Daily Reminders Set! 🔔", 
                        "You'll receive daily Quran verse notifications at 8:00 AM.\n\nYou can change the time or turn them off in Settings.", 
                        "Got it!");
                }
                else
                {
                    // User denied permission - still save preference as disabled
                    _notificationService.SetNotificationEnabled(false);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing notifications: {ex.Message}");
        }
    }

    private void UpdateDate()
    {
        DateLabel.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    }

    private async Task LoadDailyAyah()
    {
        await PerformActionWithLoading(async () =>
        {
            _currentAyah = await _quranService.GetDailyAyahAsync();
            UpdateVerseDisplay();
        });
    }

    private async Task LoadRandomAyah()
    {
        await PerformActionWithLoading(async () =>
        {
            _currentAyah = await _quranService.GetRandomAyahAsync();
            UpdateVerseDisplay();
        });
    }

    private async Task LoadNextAyah()
    {
        if (_currentAyah == null) return;

        await PerformActionWithLoading(async () =>
        {
            _currentAyah = await _quranService.GetNextAyah(_currentAyah);
            UpdateVerseDisplay();
        });
    }

    private async Task LoadPreviousAyah()
    {
        if (_currentAyah == null) return;

        await PerformActionWithLoading(async () =>
        {
            _currentAyah = await _quranService.GetPreviousAyah(_currentAyah);
            UpdateVerseDisplay();
        });
    }

    private async Task PerformActionWithLoading(Func<Task> action)
    {
        if (_isLoading) return;

        _isLoading = true;
        LoadingIndicator.IsVisible = true;
        VerseCard.IsVisible = false;
        NavigationGrid.IsVisible = false;

        // Disable buttons
        TodayButton.IsEnabled = false;
        PreviousButton.IsEnabled = false;
        RandomButton.IsEnabled = false;
        NextButton.IsEnabled = false;

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load verse: {ex.Message}", "OK");
        }
        finally
        {
            _isLoading = false;
            LoadingIndicator.IsVisible = false;

            // Re-enable buttons
            TodayButton.IsEnabled = true;
            PreviousButton.IsEnabled = true;
            RandomButton.IsEnabled = true;
            NextButton.IsEnabled = true;
        }
    }

    private void UpdateVerseDisplay()
    {
        if (_currentAyah == null) return;

        // Update verse information
        SurahNameLabel.Text = _currentAyah.SurahName;
        SurahReferenceLabel.Text = $"{_currentAyah.SurahNumber}:{(int)_currentAyah.AyahNumber}";
        ArabicLabel.Text = _currentAyah.Arabic;
        TransliterationLabel.Text = _currentAyah.Transliteration;
        TranslationLabel.Text = _currentAyah.Translation;

        // Show verse card and navigation
        VerseCard.IsVisible = true;
        NavigationGrid.IsVisible = true;

        // Hide transliteration if empty
        TransliterationLabel.IsVisible = !string.IsNullOrWhiteSpace(_currentAyah.Transliteration);
    }

    // Event Handlers
    private async void OnTodayClicked(object sender, EventArgs e)
    {
        await LoadDailyAyah();
    }

    private async void OnRandomClicked(object sender, EventArgs e)
    {
        await LoadRandomAyah();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await LoadNextAyah();
    }

    private async void OnPreviousClicked(object sender, EventArgs e)
    {
        await LoadPreviousAyah();
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (_currentAyah == null) return;

        var shareText = $"{_currentAyah.Arabic}\n\n" +
                       $"\"{_currentAyah.Translation}\"\n\n" +
                       $"- Quran {_currentAyah.SurahNumber}:{(int)_currentAyah.AyahNumber} ({_currentAyah.SurahName})";

        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = "Quran Daily Verse",
                Text = shareText
            });
        }
        catch
        {
            try
            {
                await Clipboard.Default.SetTextAsync(shareText);
                await DisplayAlert("Copied", "Verse copied to clipboard!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to share verse: {ex.Message}", "OK");
            }
        }
    }
}

