namespace QuranDailyApp.Models;

public class Chapter
{
    public int Id { get; set; }
    public string SurahName { get; set; } = string.Empty;
    public string SurahNameAr { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TotalVerses { get; set; }
    public List<Ayah> Verses { get; set; } = [];
}
