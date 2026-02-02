using QuranDailyApp.Core.Interfaces;

namespace QuranDailyApp.MAUI.Services;

public class MauiFileService : IFileService
{
    public async Task<string> ReadTextAsync(string fileName)
    {
        try
        {
            // For MAUI, read from the app package (Resources/Raw folder)
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            // Log the error or handle it as needed
            throw new FileNotFoundException($"Could not read file: {fileName}", ex);
        }
    }
}