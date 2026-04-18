using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;
using SaludMcp.Clients;

namespace SaludMcp.Tools;

[McpServerToolType]
public class EquivalenceTools(CimaClient cimaClient)
{
    [McpServerTool, Description("Find all generic equivalents (same Virtual Medicinal Product) for a medication by its registry number.")]
    public async Task<string> GetGenerics(
        [Description("Registry number (nregistro) of the medication")] string registryNumber)
    {
        try
        {
            var presentations = await cimaClient.GetPresentationsAsync(registryNumber);

            if (presentations.Count == 0)
                return $"No presentations found for registry number: **{registryNumber}**";

            // Group by DCP (substance + dose + form). Falls back to VTM (pure substance) when absent.
            var dcpGroups = presentations
                .GroupBy(p => p.Dcp?.Name ?? p.Vtm?.Name ?? "Unknown")
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine($"## Generic Equivalents for Registry {registryNumber}");
            sb.AppendLine();

            foreach (var group in dcpGroups)
            {
                sb.AppendLine($"### {group.Key}");
                foreach (var presentation in group)
                {
                    var status = presentation.IsCommercial ? "commercially available" : "not commercially available";
                    var supplyWarning = presentation.HasSupplyProblems ? " ⚠️ supply problem" : string.Empty;
                    sb.AppendLine($"- **CN {presentation.NationalCode}** ({presentation.RegistryNumber}): {presentation.Name} — {status}{supplyWarning}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            return "Rate limit reached. Please wait a moment before retrying.";
        }
        catch (TaskCanceledException)
        {
            return "The request timed out. The CIMA API may be temporarily unavailable.";
        }
        catch (Exception ex)
        {
            return $"Error retrieving generics: {ex.Message}";
        }
    }

    [McpServerTool, Description("Find all medications containing a specific active ingredient in the Spanish AEMPS database.")]
    public async Task<string> SearchByActiveIngredient(
        [Description("Active ingredient name (e.g. 'ibuprofeno', 'paracetamol')")] string activeIngredient,
        [Description("Return only commercially available medications")] bool onlyCommercial = true)
    {
        try
        {
            var results = await cimaClient.SearchByActiveIngredientAsync(activeIngredient, onlyCommercial);

            if (results.Results.Count == 0)
                return $"No medications found containing active ingredient: **{activeIngredient}**";

            var sb = new StringBuilder();
            sb.AppendLine($"## Medications Containing \"{activeIngredient}\"");
            sb.AppendLine($"Showing {results.Results.Count} of {results.TotalRows} results\n");

            var grouped = results.Results.GroupBy(m => m.PharmaceuticalForm?.Name ?? "Other");
            foreach (var group in grouped)
            {
                sb.AppendLine($"### {group.Key}");
                foreach (var med in group)
                {
                    var supplyWarning = med.HasSupplyProblems ? " ⚠️" : string.Empty;
                    sb.AppendLine($"- **{med.Name}** ({med.RegistryNumber}) — {med.Laboratory}{supplyWarning}");
                }
                sb.AppendLine();
            }

            if (results.TotalRows > results.Results.Count)
                sb.AppendLine($"> There are {results.TotalRows - results.Results.Count} more results. Use search_medication for more specific searches.");

            return sb.ToString();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            return "Rate limit reached. Please wait a moment before retrying.";
        }
        catch (TaskCanceledException)
        {
            return "The request timed out. The CIMA API may be temporarily unavailable.";
        }
        catch (Exception ex)
        {
            return $"Error searching by active ingredient: {ex.Message}";
        }
    }
}
