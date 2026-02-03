using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Core.Models;
using System.Diagnostics;

namespace QuranDailyApp.MAUI;

[QueryProperty("Ayah", "ayah")]
public partial class VerseViewerPage : ContentPage
{
    private readonly IBookmarkService _bookmarkService;
    private readonly INotificationService _notificationService;
    private AyahDisplay? _currentAyah;

    public VerseViewerPage(IBookmarkService bookmarkService, INotificationService notificationService)
    {
        InitializeComponent();
        _bookmarkService = bookmarkService;
        _notificationService = notificationService;
    }

    public AyahDisplay Ayah
    {
        get => _currentAyah ?? new AyahDisplay();

        set
        {
            _currentAyah = value;
            UpdateVerseDisplay();
        }
    }

    private async void UpdateVerseDisplay()
    {
        if (_currentAyah == null) return;

        // Update verse information
        SurahNameLabel.Text = _currentAyah.SurahName;
        SurahReferenceLabel.Text = $"{_currentAyah.SurahNumber}:{(int)_currentAyah.AyahNumber}";
        ArabicLabel.Text = _currentAyah.Arabic;
        TransliterationLabel.Text = _currentAyah.Transliteration;
        TranslationLabel.Text = _currentAyah.Translation;

        // Update bookmark button appearance
        await UpdateBookmarkButton();

        // Hide transliteration if empty
        TransliterationLabel.IsVisible = !string.IsNullOrWhiteSpace(_currentAyah.Transliteration);
        
        // Update page title
        Title = $"{_currentAyah.SurahName} - Verse {(int)_currentAyah.AyahNumber}";
    }

    private async Task UpdateBookmarkButton()
    {
        if (_currentAyah == null) return;
        
        var isBookmarked = await _bookmarkService.IsBookmarkedAsync(_currentAyah);
        BookmarkButton.Text = isBookmarked ? "🔖" : "🏷️";
        BookmarkButton.TextColor = isBookmarked ? Colors.Red : Colors.Blue;
    }

    private async void OnBookmarkClicked(object sender, EventArgs e)
    {
        if (_currentAyah == null) return;

        try
        {
            var isBookmarked = await _bookmarkService.IsBookmarkedAsync(_currentAyah);
            
            if (isBookmarked)
            {
                await _bookmarkService.RemoveBookmarkAsync(_currentAyah);
                await _notificationService.ShowToastAsync("Bookmark removed");
            }
            else
            {
                await _bookmarkService.BookmarkVerseAsync(_currentAyah);
                await _notificationService.ShowSuccessToastAsync("Verse bookmarked!");
            }

            await UpdateBookmarkButton();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not bookmark verse: {ex.Message}");
            await _notificationService.ShowErrorToastAsync("Could not bookmark verse. Please try again.");
        }
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
                await _notificationService.ShowSuccessToastAsync("Verse copied to clipboard!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sharing failed: {ex.Message}");
                await _notificationService.ShowErrorToastAsync("Unable to share verse. Please try again.");
            }
        }
    }
}