using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Core.Models;
using System.Text.Json;

namespace QuranDailyApp.MAUI.Services;

public class MauiBookmarkService : IBookmarkService
{
    private const string BOOKMARKS_KEY = "BookmarkedVerses";
    private List<AyahDisplay>? _cachedBookmarks;

    public async Task<List<AyahDisplay>> GetBookmarkedVersesAsync()
    {
        if (_cachedBookmarks != null)
            return _cachedBookmarks;

        try
        {
            var bookmarksJson = Preferences.Get(BOOKMARKS_KEY, string.Empty);
            
            if (string.IsNullOrEmpty(bookmarksJson))
            {
                _cachedBookmarks = [];
                return _cachedBookmarks;
            }

            _cachedBookmarks = JsonSerializer.Deserialize<List<AyahDisplay>>(bookmarksJson) ?? [];
            return _cachedBookmarks;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading bookmarks: {ex.Message}");
            _cachedBookmarks = [];
            return _cachedBookmarks;
        }
    }

    public async Task<bool> IsBookmarkedAsync(AyahDisplay ayah)
    {
        var bookmarks = await GetBookmarkedVersesAsync();
        return bookmarks.Any(b => b.SurahNumber == ayah.SurahNumber && 
                                 Math.Abs(b.AyahNumber - ayah.AyahNumber) < 0.001);
    }

    public async Task BookmarkVerseAsync(AyahDisplay ayah)
    {
        var bookmarks = await GetBookmarkedVersesAsync();
        
        // Check if already bookmarked
        var exists = bookmarks.Any(b => b.SurahNumber == ayah.SurahNumber && 
                                       Math.Abs(b.AyahNumber - ayah.AyahNumber) < 0.001);
        
        if (!exists)
        {
            bookmarks.Insert(0, ayah); // Add to beginning (most recent first)
            await SaveBookmarksAsync(bookmarks);
        }
    }

    public async Task RemoveBookmarkAsync(AyahDisplay ayah)
    {
        var bookmarks = await GetBookmarkedVersesAsync();
        var toRemove = bookmarks.FirstOrDefault(b => b.SurahNumber == ayah.SurahNumber && 
                                                    Math.Abs(b.AyahNumber - ayah.AyahNumber) < 0.001);
        
        if (toRemove != null)
        {
            bookmarks.Remove(toRemove);
            await SaveBookmarksAsync(bookmarks);
        }
    }

    public async Task<int> GetBookmarkCountAsync()
    {
        var bookmarks = await GetBookmarkedVersesAsync();
        return bookmarks.Count;
    }

    private async Task SaveBookmarksAsync(List<AyahDisplay> bookmarks)
    {
        try
        {
            var json = JsonSerializer.Serialize(bookmarks);
            Preferences.Set(BOOKMARKS_KEY, json);
            _cachedBookmarks = bookmarks;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving bookmarks: {ex.Message}");
        }
    }
}