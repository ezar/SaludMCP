using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;
using SaludMcp.Clients;

namespace SaludMcp.Tools;

[McpServerToolType]
public class SafetyTools(CimaClient cimaClient)
{
    [McpServerTool, Description("Get current medication supply problems (shortages) from the Spanish AEMPS database. Returns all active problems if no national code is provided.")]
    public async Task<string> GetSupplyProblems(
        [Description("National code (CN) of a specific medication. Leave empty to get all active supply problems.")] string? nationalCode = null)
    {
        try
        {
            var problems = string.IsNullOrWhiteSpace(nationalCode)
                ? await cimaClient.GetAllSupplyProblemsAsync()
                : await cimaClient.GetSupplyProblemsByNationalCodeAsync(nationalCode);

            if (problems.Count == 0)
            {
                return string.IsNullOrWhiteSpace(nationalCode)
                    ? "No active supply problems found."
                    : $"No supply problems found for national code: **{nationalCode}**";
            }

            var sb = new StringBuilder();
            var title = string.IsNullOrWhiteSpace(nationalCode)
                ? "All Active Supply Problems"
                : $"Supply Problems for CN {nationalCode}";

            sb.AppendLine($"## {title}");
            sb.AppendLine($"Found **{problems.Count}** supply problem(s)\n");

            foreach (var problem in problems)
            {
                sb.AppendLine($"### {problem.Name}");
                sb.AppendLine($"- **National code (CN)**: {problem.NationalCode}");
                sb.AppendLine($"- **Start date**: {CimaClient.FormatEpoch(problem.StartDate)}");
                sb.AppendLine($"- **Expected end date**: {CimaClient.FormatEpoch(problem.EndDate)}");
                sb.AppendLine($"- **Active**: {(problem.IsActive ? "Yes" : "No")}");
                if (!string.IsNullOrWhiteSpace(problem.Observations))
                    sb.AppendLine($"- **Notes**: {problem.Observations}");
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
            return $"Error retrieving supply problems: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get safety alerts and notes for a medication in the Spanish AEMPS database.")]
    public async Task<string> GetSafetyAlerts(
        [Description("Registry number (nregistro) of the medication")] string registryNumber)
    {
        try
        {
            var alerts = await cimaClient.GetSafetyAlertsAsync(registryNumber);

            if (alerts.Count == 0)
                return $"No safety alerts found for registry number: **{registryNumber}**";

            var sb = new StringBuilder();
            sb.AppendLine($"## Safety Alerts for Registry {registryNumber}");
            sb.AppendLine($"Found **{alerts.Count}** alert(s)\n");

            foreach (var alert in alerts)
            {
                var alertType = alert.Type switch
                {
                    1 => "Dear Healthcare Professional Letter",
                    2 => "Drug Safety Update",
                    3 => "Press Release",
                    _ => $"Alert type {alert.Type}"
                };

                sb.AppendLine($"### {alert.Subject}");
                sb.AppendLine($"- **Type**: {alertType}");
                sb.AppendLine($"- **Reference**: {alert.Reference ?? alert.Number}");
                sb.AppendLine($"- **Date**: {CimaClient.FormatEpoch(alert.Date)}");
                if (!string.IsNullOrWhiteSpace(alert.Url))
                    sb.AppendLine($"- **Document**: [{alert.Url}]({alert.Url})");
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
            return $"Error retrieving safety alerts: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get medications that have been added, removed, or modified since a given date in the Spanish AEMPS database.")]
    public async Task<string> GetRecentChanges(
        [Description("Start date in ISO format (yyyy-MM-dd), e.g. '2024-01-01'")] string since,
        [Description("Filter by change type: 'all', 'new', 'removed', or 'modified'")] string changeType = "all")
    {
        try
        {
            if (!DateOnly.TryParse(since, out var sinceDate))
                return $"Invalid date format: **{since}**. Use ISO format yyyy-MM-dd (e.g. 2024-01-01).";

            var changes = await cimaClient.GetRecentChangesAsync(sinceDate);

            if (changeType != "all")
            {
                var typeId = changeType switch
                {
                    "new"      => 1,
                    "removed"  => 2,
                    "modified" => 3,
                    _          => -1
                };
                if (typeId > 0)
                    changes = changes.Where(c => c.ChangeType == typeId).ToList();
            }

            if (changes.Count == 0)
                return $"No changes found since **{since}**" + (changeType != "all" ? $" for type: {changeType}" : "") + ".";

            var sb = new StringBuilder();
            sb.AppendLine($"## Medication Changes Since {since}");
            sb.AppendLine($"Found **{changes.Count}** change(s)\n");

            foreach (var change in changes)
            {
                var changeTypeLabel = change.ChangeType switch
                {
                    1 => "New authorization",
                    2 => "Removed",
                    3 => "Modified",
                    _ => $"Change type {change.ChangeType}"
                };

                sb.AppendLine($"### {change.RegistryNumber}");
                sb.AppendLine($"- **Change type**: {changeTypeLabel}");
                sb.AppendLine($"- **Date**: {CimaClient.FormatEpoch(change.Date)}");
                if (change.Changes.Count > 0)
                    sb.AppendLine($"- **Affected fields**: {string.Join(", ", change.Changes)}");
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
            return $"Error retrieving recent changes: {ex.Message}";
        }
    }
}
