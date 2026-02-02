using QuranDailyApp.Core.Interfaces;

namespace QuranDailyApp.Services;

public class BlazorFileService(HttpClient httpClient) : IFileService
{
    public async Task<string> ReadTextAsync(string fileName)
    {
        try
        {
            // For Blazor, read from wwwroot via HTTP
            return await httpClient.GetStringAsync($"data/{fileName}");
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"Could not read file: data/{fileName}", ex);
        }
    }
}