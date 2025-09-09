using System.Text.Json.Serialization;

namespace QuranDailyApp.Models;

public class QuranMetadata
{
    [JsonPropertyName("total_surahs")]
    public int TotalSurahs { get; set; } = 114;

    [JsonPropertyName("total_meccan_surahs")]
    public int TotalMeccanSurahs { get; set; } = 86;

    [JsonPropertyName("total_medinan_surahs")]
    public int TotalMedinanSurahs { get; set; } = 28;

    [JsonPropertyName("total_verses")]
    public int TotalVerses { get; set; } = 6236;
}
