using System.Text.Json.Serialization;

namespace QuranDailyApp.Models;

public class QuranData
{
    [JsonPropertyName("total_surahs")]
    public int TotalSurahs { get; set; } = 114;

    [JsonPropertyName("total_meccan_surahs")]
    public int TotalMeccanSurahs { get; set; } = 86;

    [JsonPropertyName("total_medinan_surahs")]
    public int TotalMedinanSurahs { get; set; } = 28;

    [JsonPropertyName("total_verses")]
    public int TotalVerses { get; set; } = 6236;

    [JsonPropertyName("number_of_words")]
    public int NumberOfWords { get; set; } = 77430;

    [JsonPropertyName("number_of_unique_words")]
    public int NumberOfUniqueWords { get; set; } = 18994;

    [JsonPropertyName("number_of_stems")]
    public int NumberOfStems { get; set; } = 12183;

    [JsonPropertyName("number_of_lemmas")]
    public int NumberOfLemmas { get; set; } = 3382;

    [JsonPropertyName("number_of_roots")]
    public int NumberOfRoots { get; set; } = 1685;

    [JsonPropertyName("chapters")]
    public Dictionary<string, Chapter> Chapters { get; set; } = [];
}