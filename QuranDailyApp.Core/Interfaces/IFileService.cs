namespace QuranDailyApp.Core.Interfaces;

public interface IFileService
{
    Task<string> ReadTextAsync(string fileName);
}