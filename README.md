# salud-mcp

An open-source **MCP (Model Context Protocol) server** for the Spanish public health system, built on .NET 10.

Exposes tools that allow any LLM to query real-time medication data from the official **AEMPS** (Agencia Española de Medicamentos y Productos Sanitarios) **CIMA REST API** — no API key, no registration, 100% free public data.

---

## Available Tools

| Tool | Description |
|---|---|
| `search_medication` | Search medications by name or active ingredient |
| `get_medication` | Get full detail of a medication by registry number or national code |
| `get_supply_problems` | Get current supply problems / shortages (all or for a specific CN) |
| `get_safety_alerts` | Get safety alerts and notes for a medication |
| `get_recent_changes` | Get medications added, removed, or modified since a given date |
| `get_generics` | Find all generic equivalents (same pharmaceutical product) |
| `search_by_active_ingredient` | Find all medications containing a specific active ingredient |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

---

## Installation

```bash
git clone https://github.com/ezar/SaludMCP.git
cd SaludMCP
dotnet build salud-mcp.slnx
```

---

## Running the Server

```bash
dotnet run --project src/
```

The MCP endpoint will be available at: `http://localhost:5000/mcp`

---

## Usage with Claude Desktop

Add the following to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "salud-mcp": {
      "type": "http",
      "url": "http://localhost:5000/mcp"
    }
  }
}
```

Start the server before launching Claude Desktop.

---

## Usage with MCP Inspector

```bash
npx @modelcontextprotocol/inspector http://localhost:5000/mcp
```

---

## Example Queries

Once connected to Claude (or any MCP-compatible LLM), you can ask:

- *"Is there a supply shortage for ibuprofen 400mg?"*
- *"What are the safety alerts for fentanyl patches?"*
- *"Find all generics for Adiro 100mg"*
- *"What medications contain paracetamol and are available without prescription?"*
- *"What new medications were authorized in the last 30 days?"*

---

## Architecture

```
src/
├── Clients/CimaClient.cs          # HTTP client for the CIMA REST API
├── Models/Cima/                   # Strongly-typed response models
├── Tools/
│   ├── MedicationTools.cs         # search_medication, get_medication
│   ├── SafetyTools.cs             # get_supply_problems, get_safety_alerts, get_recent_changes
│   └── EquivalenceTools.cs        # get_generics, search_by_active_ingredient
└── Program.cs                     # ASP.NET Core + MCP Streamable HTTP bootstrap
```

- **Transport**: [MCP Streamable HTTP](https://modelcontextprotocol.io/docs/concepts/transports) via `ModelContextProtocol.AspNetCore`
- **Data source**: [CIMA REST API](https://www.aemps.gob.es/apps/cima/docs/CIMA_REST_API.pdf) — official AEMPS public API

---

## Development

```bash
# Build
dotnet build salud-mcp.slnx

# Test
dotnet test salud-mcp.slnx

# Run
dotnet run --project src/
```

---

## Contributing

Contributions are welcome. Please open an issue or pull request on [GitHub](https://github.com/ezar/SaludMCP).

Pending tools (Priority 3):
- `search_in_technical_sheet` — search text within medication technical data sheets
- `get_safety_materials` — get patient/professional safety material documents

---

## License

MIT
