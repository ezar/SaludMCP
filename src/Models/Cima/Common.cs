using System.Text.Json.Serialization;

namespace SaludMcp.Models.Cima;

public class AuthorizationStatus
{
    /// <summary>Authorization date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("aut")]
    public long? AuthorizationDate { get; set; }

    /// <summary>Last revision date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("rev")]
    public long? RevisionDate { get; set; }
}

public class AtcCode
{
    [JsonPropertyName("codigo")]
    public string? Code { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    /// <summary>ATC hierarchy level (3–5).</summary>
    [JsonPropertyName("nivel")]
    public int Level { get; set; }
}

public class ActiveIngredient
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("codigo")]
    public string? Code { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    [JsonPropertyName("cantidad")]
    public string? Quantity { get; set; }

    [JsonPropertyName("unidad")]
    public string? Unit { get; set; }
}

public class AdministrationRoute
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }
}

public class PharmaceuticalForm
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }
}

public class Document
{
    /// <summary>1 = Technical data sheet, 2 = Patient leaflet, 3 = Public assessment report.</summary>
    [JsonPropertyName("tipo")]
    public int Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("urlHtml")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("secc")]
    public bool HasSections { get; set; }

    [JsonPropertyName("fecha")]
    public long? Date { get; set; }
}

/// <summary>Shared shape for coded references (vtm, dcp, dcpf) — id + description name.</summary>
public class CodedReference
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("nombre")]
    public string? Name { get; set; }
}

/// <summary>A single safety material document (patient or professional).</summary>
public class SafetyMaterialDocument
{
    [JsonPropertyName("nombre")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Publication date as Unix epoch milliseconds.</summary>
    [JsonPropertyName("fecha")]
    public long? Date { get; set; }
}

/// <summary>Response from GET materiales — lists of documents split by audience.</summary>
public class SafetyMaterials
{
    [JsonPropertyName("listaDocsPaciente")]
    public List<SafetyMaterialDocument> PatientDocuments { get; set; } = [];

    [JsonPropertyName("listaDocsProfesional")]
    public List<SafetyMaterialDocument> ProfessionalDocuments { get; set; } = [];
}

public class PaginatedResult<T>
{
    [JsonPropertyName("totalFilas")]
    public int TotalRows { get; set; }

    [JsonPropertyName("pagina")]
    public int Page { get; set; }

    [JsonPropertyName("tamanioPagina")]
    public int PageSize { get; set; }

    [JsonPropertyName("resultados")]
    public List<T> Results { get; set; } = [];
}
