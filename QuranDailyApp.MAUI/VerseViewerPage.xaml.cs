using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Core.Models;

namespace QuranDailyApp.MAUI;

[QueryProperty("Ayah", "ayah")]
public partial class VerseViewerPage : ContentPage
{
    private readonly IBookmarkService _bookmarkService;
    private AyahDisplay? _currentAyah;

    public VerseViewerPage(IBookmarkService bookmarkService)
    {
        InitializeComponent();
        _bookmarkService = bookmarkService;
    }

    public AyahDisplay Ayah
    {
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
                await DisplayAlertAsync("Bookmark Removed", "This verse has been removed from your bookmarks.", "OK");
            }
            else
            {
                await _bookmarkService.BookmarkVerseAsync(_currentAyah);
                await DisplayAlertAsync("Verse Bookmarked! 🔖", "This verse has been added to your bookmarks.", "Great!");
            }

            await UpdateBookmarkButton();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Could not bookmark verse: {ex.Message}", "OK");
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
                await DisplayAlertAsync("Copied", "Verse copied to clipboard!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Unable to share verse: {ex.Message}", "OK");
            }
        }
    }
}