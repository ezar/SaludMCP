using System.Text.Json;
using SaludMcp.Models.Cima;

namespace SaludMcp.Clients;

public class CimaClient(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient CreateClient() => httpClientFactory.CreateClient("cima");

    private static DateTimeOffset EpochToDateTimeOffset(long epoch) =>
        DateTimeOffset.FromUnixTimeMilliseconds(epoch);

    /// <summary>Converts a nullable Unix epoch (milliseconds) to a dd/MM/yyyy string, or "N/A" if null.</summary>
    public static string FormatEpoch(long? epoch) =>
        epoch.HasValue ? EpochToDateTimeOffset(epoch.Value).ToString("dd/MM/yyyy") : "N/A";

    public async Task<PaginatedResult<Medication>> SearchByNameAsync(
        string name, bool onlyCommercial = true, int page = 1)
    {
        var client = CreateClient();
        var commercialFilter = onlyCommercial ? "&comerc=1" : string.Empty;
        var url = $"medicamentos?nombre={Uri.EscapeDataString(name)}{commercialFilter}&pagina={page}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return new PaginatedResult<Medication>();

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<PaginatedResult<Medication>>(content, JsonOptions)
               ?? new PaginatedResult<Medication>();
    }

    public async Task<PaginatedResult<Medication>> SearchByActiveIngredientAsync(
        string activeIngredient, bool onlyCommercial = true, int page = 1)
    {
        var client = CreateClient();
        var commercialFilter = onlyCommercial ? "&comerc=1" : string.Empty;
        var url = $"medicamentos?practiv1={Uri.EscapeDataString(activeIngredient)}{commercialFilter}&pagina={page}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return new PaginatedResult<Medication>();

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<PaginatedResult<Medication>>(content, JsonOptions)
               ?? new PaginatedResult<Medication>();
    }

    public async Task<Medication?> GetByRegistryNumberAsync(string registryNumber)
    {
        var client = CreateClient();
        var url = $"medicamento?nregistro={Uri.EscapeDataString(registryNumber)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<Medication>(content, JsonOptions);
    }

    public async Task<Medication?> GetByNationalCodeAsync(string nationalCode)
    {
        var client = CreateClient();
        var url = $"medicamento?cn={Uri.EscapeDataString(nationalCode)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<Medication>(content, JsonOptions);
    }

    public async Task<List<Presentation>> GetPresentationsAsync(string registryNumber)
    {
        var client = CreateClient();
        var url = $"presentaciones?nregistro={Uri.EscapeDataString(registryNumber)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return [];

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var paged = await JsonSerializer.DeserializeAsync<PaginatedResult<Presentation>>(content, JsonOptions);
        return paged?.Results ?? [];
    }

    public async Task<List<SupplyProblem>> GetAllSupplyProblemsAsync()
    {
        var client = CreateClient();
        var response = await client.GetAsync("psuministro");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return [];

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var paged = await JsonSerializer.DeserializeAsync<PaginatedResult<SupplyProblem>>(content, JsonOptions);
        return paged?.Results ?? [];
    }

    public async Task<List<SupplyProblem>> GetSupplyProblemsByNationalCodeAsync(string nationalCode)
    {
        var client = CreateClient();
        var url = $"psuministro/{Uri.EscapeDataString(nationalCode)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return [];

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var paged = await JsonSerializer.DeserializeAsync<PaginatedResult<SupplyProblem>>(content, JsonOptions);
        return paged?.Results ?? [];
    }

    public async Task<List<SafetyAlert>> GetSafetyAlertsAsync(string registryNumber)
    {
        var client = CreateClient();
        var url = $"notas?nregistro={Uri.EscapeDataString(registryNumber)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return [];

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<SafetyAlert>>(content, JsonOptions) ?? [];
    }

    public async Task<SafetyMaterials?> GetSafetyMaterialsAsync(string registryNumber)
    {
        var client = CreateClient();
        var url = $"materiales?nregistro={Uri.EscapeDataString(registryNumber)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<SafetyMaterials>(content, JsonOptions);
    }

    public async Task<List<MedicationChange>> GetRecentChangesAsync(DateOnly since)
    {
        var client = CreateClient();
        var dateStr = since.ToString("dd/MM/yyyy");
        var url = $"registroCambios?fecha={Uri.EscapeDataString(dateStr)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return [];

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var paged = await JsonSerializer.DeserializeAsync<PaginatedResult<MedicationChange>>(content, JsonOptions);
        return paged?.Results ?? [];
    }

    public async Task<List<Presentation>> GetVmppByActiveIngredientAsync(string activeIngredient)
    {
        var client = CreateClient();
        var url = $"vmpp?practiv1={Uri.EscapeDataString(activeIngredient)}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return [];

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<Presentation>>(content, JsonOptions) ?? [];
    }
}
