using System.Text.Json.Serialization;

namespace QuranDailyApp.Models;

public class Ayah
{
    public double Id { get; set; } // Note: quran.json has decimal IDs like 1.1, 2.1
    public string Content { get; set; } = string.Empty; // Arabic
    public string Transliteration { get; set; } = string.Empty;

    [JsonPropertyName("translation_eng")]
    public string TranslationEng { get; set; } = string.Empty;
}
