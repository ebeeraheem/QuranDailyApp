using System.Text.Json.Serialization;

namespace QuranDailyApp.Models;

public class QuranData
{
    [JsonPropertyName("metadata")]
    public QuranMetadata Metadata { get; set; } = new();

    [JsonPropertyName("chapters")]
    public List<Chapter> Chapters { get; set; } = [];
}