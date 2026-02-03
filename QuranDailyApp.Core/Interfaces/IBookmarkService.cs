using QuranDailyApp.Core.Models;

namespace QuranDailyApp.Core.Interfaces;

public interface IBookmarkService
{
    Task<List<AyahDisplay>> GetBookmarkedVersesAsync();
    Task<bool> IsBookmarkedAsync(AyahDisplay ayah);
    Task BookmarkVerseAsync(AyahDisplay ayah);
    Task RemoveBookmarkAsync(AyahDisplay ayah);
    Task<int> GetBookmarkCountAsync();
}