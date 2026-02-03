using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Core.Models;

namespace QuranDailyApp.MAUI;

public partial class BookmarksPage : ContentPage
{
    private readonly IBookmarkService _bookmarkService;
    private List<AyahDisplay> _bookmarks = [];

    public BookmarksPage(IBookmarkService bookmarkService)
    {
        InitializeComponent();
        _bookmarkService = bookmarkService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadBookmarks();
    }

    private async Task LoadBookmarks()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            BookmarksCollectionView.IsVisible = false;
            EmptyStateLayout.IsVisible = false;

            _bookmarks = await _bookmarkService.GetBookmarkedVersesAsync();
            BookmarksCollectionView.ItemsSource = _bookmarks;

            if (_bookmarks.Count == 0)
            {
                EmptyStateLayout.IsVisible = true;
            }
            else
            {
                BookmarksCollectionView.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to load bookmarks: {ex.Message}", "OK");
            EmptyStateLayout.IsVisible = true;
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
        }
    }

    private static async void OnBookmarkItemTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is AyahDisplay ayah)
        {
            // Navigate to verse viewer page
            var parameters = new Dictionary<string, object>
            {
                ["ayah"] = ayah
            };
            
            await Shell.Current.GoToAsync("//verse", parameters);
        }
    }

    private async void OnRemoveBookmarkClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is AyahDisplay ayah)
        {
            try
            {
                var result = await DisplayAlertAsync("Remove Bookmark", 
                    $"Remove \"{ayah.SurahName}\" verse from bookmarks?", 
                    "Remove", "Cancel");

                if (result)
                {
                    await _bookmarkService.RemoveBookmarkAsync(ayah);
                    await LoadBookmarks();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to remove bookmark: {ex.Message}", "OK");
            }
        }
    }

    private static async void OnGoToDailyVerseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//home");
    }
}