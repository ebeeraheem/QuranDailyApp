using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Core.Models;

namespace QuranDailyApp.Services;

public class BlazorBookmarkService : IBookmarkService
{
    // Blazor Web app uses localStorage for bookmarks
    public Task<List<AyahDisplay>> GetBookmarkedVersesAsync() => Task.FromResult(new List<AyahDisplay>());
    
    public Task<bool> IsBookmarkedAsync(AyahDisplay ayah) => Task.FromResult(false);
    
    public Task BookmarkVerseAsync(AyahDisplay ayah) => Task.CompletedTask;
    
    public Task RemoveBookmarkAsync(AyahDisplay ayah) => Task.CompletedTask;
    
    public Task<int> GetBookmarkCountAsync() => Task.FromResult(0);
}