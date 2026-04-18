# salud-mcp

[![Build](https://github.com/ezar/SaludMCP/actions/workflows/ci.yml/badge.svg)](https://github.com/ezar/SaludMCP/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/download/dotnet/10.0)

An open-source **MCP (Model Context Protocol) server** for the Spanish public health system, built on .NET 10.

Exposes tools that allow any LLM to query real-time medication data from the official **AEMPS** (Agencia Española de Medicamentos y Productos Sanitarios) **CIMA REST API**.

- ✅ No API key required
- ✅ No registration
- ✅ 100% free public data
- ✅ Real-time official data

---

## Available Tools

| Tool | Description |
|---|---|
| `search_medication` | Search medications by name or active ingredient |
| `get_medication` | Get full detail of a medication by registry number or national code |
| `get_supply_problems` | Get current supply problems / shortages (all or for a specific CN) |
| `get_safety_alerts` | Get safety alerts and notes for a medication |
| `get_recent_changes` | Get medications added, removed, or modified since a given date |
| `get_generics` | Find all generic equivalents for a medication |
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

The MCP endpoint will be available at: **`http://localhost:5000/mcp`**

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

Once connected to Claude (or any MCP-compatible LLM), you can ask natural language questions like:

- *"Is there a supply shortage for ibuprofen 400mg?"*
- *"What are the safety alerts for fentanyl patches?"*
- *"Find all generics for Adiro 100mg"*
- *"What medications contain paracetamol and are available without prescription?"*
- *"What new medications were authorized in the last 30 days?"*
- *"Does metformin affect driving ability?"*

---

## Architecture

```
salud-mcp/
├── src/
│   ├── Clients/
│   │   └── CimaClient.cs          # Named HttpClient wrapper for the CIMA REST API
│   ├── Models/Cima/
│   │   ├── Common.cs              # Shared types (AuthorizationStatus, AtcCode, etc.)
│   │   ├── Medication.cs
│   │   ├── Presentation.cs
│   │   ├── SupplyProblem.cs
│   │   ├── SafetyAlert.cs
│   │   └── MedicationChange.cs
│   ├── Tools/
│   │   ├── MedicationTools.cs     # search_medication, get_medication
│   │   ├── SafetyTools.cs         # get_supply_problems, get_safety_alerts, get_recent_changes
│   │   └── EquivalenceTools.cs    # get_generics, search_by_active_ingredient
│   └── Program.cs                 # ASP.NET Core + MCP Streamable HTTP bootstrap
└── tests/
    └── salud-mcp.Tests/
```

**Transport**: [MCP Streamable HTTP](https://modelcontextprotocol.io/docs/concepts/transports) via `ModelContextProtocol.AspNetCore` v1.2.0

**Data source**: [CIMA REST API](https://www.aemps.gob.es/apps/cima/docs/CIMA_REST_API.pdf) — official AEMPS public API

---

## Development

```bash
# Build
dotnet build salud-mcp.slnx

# Test
dotnet test salud-mcp.slnx

# Run
dotnet run --project src/

# Inspect end-to-end
npx @modelcontextprotocol/inspector http://localhost:5000/mcp
```

---

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) and open an issue or pull request.

**Pending tools (help wanted):**
- `search_in_technical_sheet` — search text within medication technical data sheets (`POST buscarEnFichaTecnica`)
- `get_safety_materials` — get patient/professional safety material documents (`GET materiales`)

---

## License

This project is licensed under the MIT License — see [LICENSE](LICENSE) for details.

Data provided by [AEMPS CIMA](https://cima.aemps.es/) under their public access terms.
