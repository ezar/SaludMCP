using System.Text.Json.Serialization;

namespace SaludMcp.Models.Cima;

public class Presentation
{
    [JsonPropertyName("cn")]
    public string? NationalCode { get; set; }

    [JsonPropertyName("nregistro")]
    public string? RegistryNumber { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    [JsonPropertyName("pactivos")]
    public string? ActiveIngredientsSummary { get; set; }

    [JsonPropertyName("estado")]
    public AuthorizationStatus? Status { get; set; }

    [JsonPropertyName("comerc")]
    public bool IsCommercial { get; set; }

    [JsonPropertyName("psum")]
    public bool HasSupplyProblems { get; set; }

    /// <summary>
    /// Virtual Therapeutic Moiety — identifies the pure active substance,
    /// shared across all generic equivalents of the same molecule.
    /// </summary>
    [JsonPropertyName("vtm")]
    public CodedReference? Vtm { get; set; }

    /// <summary>
    /// Denominación Común del Producto Farmacéutico — identifies the substance
    /// at a specific dose and pharmaceutical form. Use this to group generic equivalents.
    /// </summary>
    [JsonPropertyName("dcp")]
    public CodedReference? Dcp { get; set; }

    /// <summary>
    /// Denominación Común del Producto Farmacéutico de Fabricación — identifies
    /// the substance at a specific dose, form, and pack size.
    /// </summary>
    [JsonPropertyName("dcpf")]
    public CodedReference? Dcpf { get; set; }
}
