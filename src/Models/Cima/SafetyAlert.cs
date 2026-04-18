using System.Text.Json.Serialization;

namespace SaludMcp.Models.Cima;

public class SafetyAlert
{
    /// <summary>1 = Dear Healthcare Professional Letter, 2 = Drug Safety Update, 3 = Press Release.</summary>
    [JsonPropertyName("tipo")]
    public int Type { get; set; }

    [JsonPropertyName("num")]
    public string? Number { get; set; }

    [JsonPropertyName("referencia")]
    public string? Reference { get; set; }

    [JsonPropertyName("asunto")]
    public string? Subject { get; set; }

    /// <summary>Publication date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("fecha")]
    public long? Date { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
