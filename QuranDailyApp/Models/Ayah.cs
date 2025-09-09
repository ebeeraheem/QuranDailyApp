namespace QuranDailyApp.Models;

public class Ayah
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty; // Arabic
    public string Transliteration { get; set; } = string.Empty;
    public string TranslationEng { get; set; } = string.Empty;
}
