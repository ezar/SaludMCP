using System.Text.Json.Serialization;

namespace SaludMcp.Models.Cima;

public class MedicationChange
{
    [JsonPropertyName("nregistro")]
    public string? RegistryNumber { get; set; }

    /// <summary>Change date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("fecha")]
    public long? Date { get; set; }

    /// <summary>1 = New authorization, 2 = Removed, 3 = Modified.</summary>
    [JsonPropertyName("tipoCambio")]
    public int ChangeType { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    [JsonPropertyName("labtitular")]
    public string? Laboratory { get; set; }

    /// <summary>List of affected fields (e.g. "estado", "ft", "prosp", "matinf").</summary>
    [JsonPropertyName("cambio")]
    public List<string> Changes { get; set; } = [];
}
