using System.Text.Json.Serialization;

namespace QuranDailyApp.Models;

public class Chapter
{
    public int Id { get; set; }

    [JsonPropertyName("surah_name")]
    public string SurahName { get; set; } = string.Empty;

    [JsonPropertyName("surah_name_ar")]
    public string SurahNameAr { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("total_verses")]
    public int TotalVerses { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, Ayah> Verses { get; set; } = [];
}
