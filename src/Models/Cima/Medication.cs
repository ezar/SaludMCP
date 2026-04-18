using System.Text.Json.Serialization;

namespace SaludMcp.Models.Cima;

public class Medication
{
    [JsonPropertyName("nregistro")]
    public string? RegistryNumber { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    /// <summary>Comma-separated active ingredients summary string returned by the search endpoint.</summary>
    [JsonPropertyName("pactivos")]
    public string? ActiveIngredientsSummary { get; set; }

    [JsonPropertyName("labtitular")]
    public string? Laboratory { get; set; }

    [JsonPropertyName("receta")]
    public bool RequiresPrescription { get; set; }

    [JsonPropertyName("conduc")]
    public bool AffectsDriving { get; set; }

    /// <summary>Black triangle (▼) — additional monitoring required by the EMA.</summary>
    [JsonPropertyName("triangulo")]
    public bool HasBlackTriangle { get; set; }

    [JsonPropertyName("huerfano")]
    public bool IsOrphan { get; set; }

    [JsonPropertyName("biosimilar")]
    public bool IsBiosimilar { get; set; }

    [JsonPropertyName("psum")]
    public bool HasSupplyProblems { get; set; }

    [JsonPropertyName("notas")]
    public bool HasSafetyAlerts { get; set; }

    [JsonPropertyName("comerc")]
    public bool IsCommercial { get; set; }

    [JsonPropertyName("estado")]
    public AuthorizationStatus? Status { get; set; }

    [JsonPropertyName("docs")]
    public List<Document> Documents { get; set; } = [];

    [JsonPropertyName("atcs")]
    public List<AtcCode> AtcCodes { get; set; } = [];

    [JsonPropertyName("principiosActivos")]
    public List<ActiveIngredient> ActiveIngredients { get; set; } = [];

    [JsonPropertyName("viasAdministracion")]
    public List<AdministrationRoute> AdministrationRoutes { get; set; } = [];

    [JsonPropertyName("formaFarmaceutica")]
    public PharmaceuticalForm? PharmaceuticalForm { get; set; }

    [JsonPropertyName("presentaciones")]
    public List<Presentation> Presentations { get; set; } = [];
}
