using Microsoft.Extensions.Caching.Memory;
using QuranDailyApp.Models;
using System.Text.Json;

namespace QuranDailyApp.Services;

public class QuranService(IMemoryCache cache, HttpClient httpClient, ILogger<QuranService> logger)
{
    private List<AyahDisplay> _allAyahs = [];
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<AyahDisplay>> GetAllAyahsAsync()
    {
        if (_allAyahs.Count != 0)
            return _allAyahs;

        if (!cache.TryGetValue(nameof(QuranData), out QuranData? quranData))
        {
            try
            {
                var json = await httpClient.GetStringAsync("data/quran.json");
                quranData = JsonSerializer.Deserialize<QuranData>(json, _jsonOptions);
                cache.Set(nameof(QuranData), quranData, TimeSpan.FromDays(1)); // Cache for a day
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load Quran data");
                return []; // Empty on error
            }
        }

        if (quranData is null)
            return [];

        _allAyahs = [];
        foreach (var chapter in quranData.Chapters)
        {
            foreach (var ayah in chapter.Verses)
            {
                _allAyahs.Add(new AyahDisplay
                {
                    SurahNumber = chapter.Id,
                    SurahName = chapter.SurahName,
                    AyahNumber = ayah.Id,
                    Arabic = ayah.Content,
                    Transliteration = ayah.Transliteration,
                    Translation = ayah.TranslationEng
                });
            }
        }

        return _allAyahs;
    }

    public async Task<AyahDisplay> GetDailyAyahAsync()
    {
        var allAyahs = await GetAllAyahsAsync();
        if (allAyahs.Count == 0)
            return new AyahDisplay { Arabic = "Error loading Quran data." };

        var dayOfYear = DateTime.Now.DayOfYear;
        var rng = new Random(dayOfYear); // Deterministic seed for same ayah each day
        var index = rng.Next(allAyahs.Count);
        return allAyahs[index];
    }

    public async Task<AyahDisplay> GetRandomAyahAsync()
    {
        var allAyahs = await GetAllAyahsAsync();
        if (allAyahs.Count == 0)
            return new AyahDisplay { Arabic = "Error loading Quran data." };

        var rng = new Random(); // Truly random
        var index = rng.Next(allAyahs.Count);
        return allAyahs[index];
    }
}