using System.Text.Json.Serialization;

namespace SaludMcp.Models.Cima;

public class SupplyProblem
{
    [JsonPropertyName("cn")]
    public string? NationalCode { get; set; }

    /// <summary>Supply problem type code as defined by AEMPS.</summary>
    [JsonPropertyName("tipoProblemaSuministro")]
    public int? ProblemType { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    /// <summary>Problem start date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("fini")]
    public long? StartDate { get; set; }

    /// <summary>Expected resolution date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("ffin")]
    public long? EndDate { get; set; }

    [JsonPropertyName("observ")]
    public string? Observations { get; set; }

    [JsonPropertyName("activo")]
    public bool IsActive { get; set; }
}
