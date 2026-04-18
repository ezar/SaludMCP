using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;
using SaludMcp.Clients;

namespace SaludMcp.Tools;

[McpServerToolType]
public class MedicationTools(CimaClient cimaClient)
{
    [McpServerTool, Description("Search medications by name or active ingredient in the Spanish AEMPS database.")]
    public async Task<string> SearchMedication(
        [Description("Search query (medication name or active ingredient)")] string query,
        [Description("Search mode: 'name' to search by medication name, 'active_ingredient' to search by active ingredient")] string searchBy = "name",
        [Description("Return only commercially available medications")] bool onlyCommercial = true)
    {
        try
        {
            var results = searchBy == "active_ingredient"
                ? await cimaClient.SearchByActiveIngredientAsync(query, onlyCommercial)
                : await cimaClient.SearchByNameAsync(query, onlyCommercial);

            if (results.Results.Count == 0)
                return $"No medications found for query: **{query}**";

            var sb = new StringBuilder();
            sb.AppendLine($"## Medications matching \"{query}\"");
            sb.AppendLine($"Showing {results.Results.Count} of {results.TotalRows} results\n");

            foreach (var med in results.Results)
            {
                sb.AppendLine($"### {med.Name}");
                sb.AppendLine($"- **Registry number**: {med.RegistryNumber}");
                sb.AppendLine($"- **Active ingredients**: {med.ActiveIngredientsSummary}");
                sb.AppendLine($"- **Laboratory**: {med.Laboratory}");
                sb.AppendLine($"- **Requires prescription**: {(med.RequiresPrescription ? "Yes" : "No")}");
                sb.AppendLine($"- **Commercially available**: {(med.IsCommercial ? "Yes" : "No")}");
                sb.AppendLine($"- **Has supply problems**: {(med.HasSupplyProblems ? "Yes" : "No")}");
                sb.AppendLine($"- **Has safety alerts**: {(med.HasSafetyAlerts ? "Yes" : "No")}");
                sb.AppendLine();
            }

            if (results.TotalRows > results.Results.Count)
                sb.AppendLine($"> There are {results.TotalRows - results.Results.Count} more results. Refine your search for more specific results.");

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
            return $"Error searching medications: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get full details of a medication by registry number or national code.")]
    public async Task<string> GetMedication(
        [Description("The medication identifier value")] string identifier,
        [Description("Identifier type: 'registry_number' (nregistro) or 'national_code' (cn)")] string identifierType = "registry_number")
    {
        try
        {
            var med = identifierType == "national_code"
                ? await cimaClient.GetByNationalCodeAsync(identifier)
                : await cimaClient.GetByRegistryNumberAsync(identifier);

            if (med is null)
                return $"Medication not found for {identifierType}: **{identifier}**";

            var sb = new StringBuilder();
            sb.AppendLine($"## {med.Name}");
            sb.AppendLine();
            sb.AppendLine("### Identification");
            sb.AppendLine($"- **Registry number**: {med.RegistryNumber}");
            sb.AppendLine($"- **Laboratory**: {med.Laboratory}");
            sb.AppendLine($"- **Authorization date**: {CimaClient.FormatEpoch(med.Status?.AuthorizationDate)}");
            sb.AppendLine();

            sb.AppendLine("### Classification");
            sb.AppendLine($"- **Requires prescription**: {(med.RequiresPrescription ? "Yes" : "No")}");
            sb.AppendLine($"- **Affects driving**: {(med.AffectsDriving ? "Yes" : "No")}");
            sb.AppendLine($"- **Black triangle (▼)**: {(med.HasBlackTriangle ? "Yes" : "No")}");
            sb.AppendLine($"- **Orphan drug**: {(med.IsOrphan ? "Yes" : "No")}");
            sb.AppendLine($"- **Biosimilar**: {(med.IsBiosimilar ? "Yes" : "No")}");
            sb.AppendLine($"- **Commercially available**: {(med.IsCommercial ? "Yes" : "No")}");
            sb.AppendLine();

            sb.AppendLine("### Alerts");
            sb.AppendLine($"- **Active supply problems**: {(med.HasSupplyProblems ? "Yes — use get_supply_problems to see details" : "No")}");
            sb.AppendLine($"- **Safety alerts**: {(med.HasSafetyAlerts ? "Yes — use get_safety_alerts to see details" : "No")}");
            sb.AppendLine();

            if (med.ActiveIngredients.Count > 0)
            {
                sb.AppendLine("### Active Ingredients");
                foreach (var ingredient in med.ActiveIngredients)
                    sb.AppendLine($"- {ingredient.Name} {ingredient.Quantity} {ingredient.Unit}".Trim());
                sb.AppendLine();
            }

            if (med.AtcCodes.Count > 0)
            {
                sb.AppendLine("### ATC Codes");
                foreach (var atc in med.AtcCodes)
                    sb.AppendLine($"- {atc.Code}: {atc.Name}");
                sb.AppendLine();
            }

            if (med.PharmaceuticalForm is not null)
            {
                sb.AppendLine("### Pharmaceutical Form");
                sb.AppendLine($"- {med.PharmaceuticalForm.Name}");
                sb.AppendLine();
            }

            if (med.AdministrationRoutes.Count > 0)
            {
                sb.AppendLine("### Administration Routes");
                foreach (var route in med.AdministrationRoutes)
                    sb.AppendLine($"- {route.Name}");
                sb.AppendLine();
            }

            if (med.Presentations.Count > 0)
            {
                sb.AppendLine("### Presentations");
                foreach (var presentation in med.Presentations)
                {
                    var commercialStatus = presentation.IsCommercial ? "available" : "not available";
                    var supplyWarning = presentation.HasSupplyProblems ? " ⚠️ supply problem" : string.Empty;
                    sb.AppendLine($"- **CN {presentation.NationalCode}**: {presentation.Name} ({commercialStatus}){supplyWarning}");
                }
                sb.AppendLine();
            }

            if (med.Documents.Count > 0)
            {
                sb.AppendLine("### Documents");
                foreach (var doc in med.Documents)
                {
                    var docType = doc.Type switch
                    {
                        1 => "Technical data sheet",
                        2 => "Patient leaflet",
                        3 => "Public assessment report",
                        _ => $"Document type {doc.Type}"
                    };
                    sb.AppendLine($"- [{docType}]({doc.Url})");
                }
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
            return $"Error retrieving medication: {ex.Message}";
        }
    }
}
