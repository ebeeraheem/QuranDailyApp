namespace QuranDailyApp.Models;

public class AyahDisplay
{
    public string SurahName { get; set; } = string.Empty;
    public int SurahNumber { get; set; }
    public int AyahNumber { get; set; }
    public string Arabic { get; set; } = string.Empty;
    public string Transliteration { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
}