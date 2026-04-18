# salud-mcp

MCP (Model Context Protocol) server for the Spanish public health system, built on .NET 10.
Exposes tools that allow any LLM to query real-time medication data from the official AEMPS (Agencia Española de Medicamentos y Productos Sanitarios) CIMA REST API.

## Project Goal

Build an open source MCP server that enables LLMs to answer questions about Spanish medications, safety alerts, supply problems, and generic equivalents — using only official, real-time public data.

No API key required. No registration. 100% free public data.

---

## Tech Stack

- **Runtime**: .NET 10 / C#
- **Project SDK**: `Microsoft.NET.Sdk.Web` (required for ASP.NET Core / Streamable HTTP transport)
- **MCP SDK**: `ModelContextProtocol.AspNetCore` v1.2.0 (official Anthropic C# SDK — ASP.NET Core variant)
- **Transport**: Streamable HTTP via `WithHttpTransport()` + `app.MapMcp()`
- **HTTP Client**: `HttpClient` (named, via `IHttpClientFactory`)
- **Serialization**: `System.Text.Json`
- **Testing**: xUnit

> **Do NOT use** `ModelContextProtocol` (base package) — it only supports stdio transport.
> **Do NOT use** `Microsoft.NET.Sdk` — it lacks the ASP.NET Core pipeline needed for HTTP transport.

---

## CIMA REST API

Base URL: `https://cima.aemps.es/cima/rest/`

All responses are JSON encoded in UTF-8. No authentication required.
Full documentation: https://www.aemps.gob.es/apps/cima/docs/CIMA_REST_API.pdf

### Key Endpoints

| Endpoint | Description |
|---|---|
| `GET medicamentos?nombre={name}` | Search medications by name |
| `GET medicamentos?practiv1={name}` | Search by active ingredient |
| `GET medicamento?nregistro={id}` | Get full medication detail by registry number |
| `GET medicamento?cn={cn}` | Get full medication detail by national code |
| `GET presentaciones?nregistro={id}` | List all presentations for a medication |
| `GET psuministro` | Get all current supply problems |
| `GET psuministro/{cn}` | Get supply problems for a specific national code |
| `GET notas?nregistro={id}` | Get safety notes for a medication |
| `GET materiales?nregistro={id}` | Get safety informational materials |
| `GET registroCambios?fecha={dd/mm/yyyy}` | Get medications changed since a date |
| `GET vmpp?practiv1={name}` | Get clinical descriptions by active ingredient |
| `POST buscarEnFichaTecnica` | Search text within technical data sheets |

### Key Data Models

**Medicamento** (full detail)
```json
{
  "nregistro": "string",
  "nombre": "string",
  "pactivos": "string",
  "labtitular": "string",
  "receta": true,
  "conduc": false,
  "triangulo": false,
  "huerfano": false,
  "biosimilar": false,
  "psum": false,
  "notas": false,
  "comerc": true,
  "estado": { "aut": "date" },
  "docs": [],
  "atcs": [],
  "principiosActivos": [],
  "viasAdministracion": [],
  "formaFarmaceutica": {},
  "presentaciones": []
}
```

**ProblemaSuministro**
```json
{
  "cn": "string",
  "nombre": "string",
  "fini": "date",
  "ffin": "date",
  "observ": "string",
  "activo": true
}
```

**Nota** (safety alert)
```json
{
  "tipo": 1,
  "num": "string",
  "ref": "string",
  "asunto": "string",
  "fecha": "date",
  "url": "string"
}
```

**RegistroCambios**
```json
{
  "nregistro": "string",
  "fecha": "date",
  "tipoCambio": 1,
  "cambios": ["estado", "ft", "prosp"]
}
```

---

## Project Structure

```
salud-mcp/
├── src/
│   ├── Clients/
│   │   └── CimaClient.cs          # Named HttpClient wrapper for the CIMA API
│   ├── Models/
│   │   └── Cima/
│   │       ├── Medicamento.cs
│   │       ├── Presentacion.cs
│   │       ├── ProblemaSuministro.cs
│   │       ├── Nota.cs
│   │       ├── RegistroCambios.cs
│   │       └── Common.cs          # Shared types: Estado, Atc, PrincipioActivo,
│   │                              #   ViaAdministracion, FormaFarmaceutica,
│   │                              #   Documento, ResultadoPaginado<T>
│   ├── Tools/
│   │   ├── MedicamentoTools.cs    # search_medication, get_medication
│   │   ├── SeguridadTools.cs      # get_supply_problems, get_safety_alerts, get_recent_changes
│   │   └── EquivalenciaTools.cs   # get_generics, search_by_active_ingredient
│   ├── Program.cs
│   └── salud-mcp.csproj
├── tests/
│   └── salud-mcp.Tests/
│       ├── Clients/
│       │   └── CimaClientTests.cs
│       ├── Tools/
│       │   └── MedicamentoToolsTests.cs
│       └── salud-mcp.Tests.csproj
├── CLAUDE.md
├── README.md
└── salud-mcp.slnx               # Solution file (XML format, .slnx)
```

---

## MCP Tools — Implemented

### Priority 1 — Core ✅

#### `search_medication`
Search medications by name or active ingredient.
- Input: `query` (string), `searchBy` (`"name"` | `"active_ingredient"`), `onlyCommercial` (bool, default true)
- Output: List of matching medications with basic info (name, registry number, lab, prescription required, supply problems)
- CIMA: `GET medicamentos?nombre=` or `GET medicamentos?practiv1=`

#### `get_medication`
Get full detail of a medication.
- Input: `identifier` (string), `identifierType` (`"registry_number"` | `"national_code"`)
- Output: Full medication data including active ingredients, presentations, ATC codes, documents
- CIMA: `GET medicamento?nregistro=` or `GET medicamento?cn=`

#### `get_supply_problems`
Get current medication supply problems (shortages).
- Input: `nationalCode` (string, optional — omit to get all active problems)
- Output: List of supply problems with start/end date and observations
- CIMA: `GET psuministro` or `GET psuministro/{cn}`

#### `get_safety_alerts`
Get safety notes and alerts for a medication.
- Input: `registryNumber` (string)
- Output: List of safety notes with subject, date, and URL to full document
- CIMA: `GET notas?nregistro=`

### Priority 2 — High value ✅

#### `get_generics`
Get all generic equivalents for a medication (same VMP — Virtual Medicinal Product).
- Input: `registryNumber` (string)
- Output: Presentations grouped by VMP, indicating commercialization status
- CIMA: `GET presentaciones?nregistro=`

#### `search_by_active_ingredient`
Find all medications containing a specific active ingredient.
- Input: `activeIngredient` (string), `onlyCommercial` (bool, default true)
- Output: Medications grouped by pharmaceutical form
- CIMA: `GET medicamentos?practiv1=`

#### `get_recent_changes`
Get medications added, removed, or modified since a given date.
- Input: `since` (ISO date string `yyyy-MM-dd`), `changeType` (`"all"` | `"new"` | `"removed"` | `"modified"`, default `"all"`)
- Output: List of changes with registry number, date, type, and affected fields
- CIMA: `GET registroCambios?fecha=`

### Priority 3 — Advanced (pending)

#### `search_in_technical_sheet`
Search text within medication technical data sheets.
- Input: `searchTerms` (list of objects with `section`, `text`, `contains`)
- Output: Medications matching the criteria
- CIMA: `POST buscarEnFichaTecnica`

#### `get_safety_materials`
Get informational safety materials for patients and professionals.
- Input: `registryNumber` (string)
- Output: Documents split by audience (patient / professional) with URLs
- CIMA: `GET materiales?nregistro=`

---

## Implementation Guidelines

### Project file (salud-mcp.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>SaludMcp</RootNamespace>
    <AssemblyName>salud-mcp</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ModelContextProtocol.AspNetCore" Version="1.2.0" />
  </ItemGroup>
</Project>
```

### Program.cs (Streamable HTTP bootstrap)

```csharp
using SaludMcp.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("cima", client =>
{
    client.BaseAddress = new Uri("https://cima.aemps.es/cima/rest/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("salud-mcp/1.0");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<CimaClient>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()        // Streamable HTTP — NOT WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
app.MapMcp();                   // Registers the /mcp endpoint
await app.RunAsync();
```

The server listens on `http://localhost:5000` by default. The MCP endpoint is at `http://localhost:5000/mcp`.

### CimaClient

- Named client `"cima"` registered via `IHttpClientFactory`
- Base address: `https://cima.aemps.es/cima/rest/`
- `User-Agent: salud-mcp/1.0`
- Timeout: 10 seconds
- Pagination via `pagina` query parameter where applicable
- All methods `async Task<T>`
- Static helper `CimaClient.FormatEpoch(long? epoch)` → `dd/MM/yyyy` string (epochs are Unix ms)

### MCP Tool Registration

```csharp
// Each tool class must be decorated with [McpServerToolType]
[McpServerToolType]
public class MedicamentoTools(CimaClient cimaClient)
{
    // Each tool method is decorated with [McpServerTool]
    [McpServerTool, Description("...")]
    public async Task<string> SearchMedication(
        [Description("...")] string query, ...)
    { ... }
}
```

`WithToolsFromAssembly()` in `Program.cs` discovers all `[McpServerToolType]` classes automatically.

### Error Handling

- Return descriptive error strings that help the LLM understand what went wrong
- Handle explicitly:
  - `HttpStatusCode.NotFound` → return empty result or "not found" message
  - `HttpStatusCode.TooManyRequests` → "Rate limit reached" message
  - `TaskCanceledException` → "Request timed out" message
- Never let raw exceptions bubble up to the MCP layer

### Response Formatting

- All tool responses are human-readable markdown
- Include relevant identifiers (registry number, national code) so the LLM can chain calls
- Dates: convert Unix epoch milliseconds to `dd/MM/yyyy` via `CimaClient.FormatEpoch()`
- Booleans: translate to plain language ("Requires prescription: Yes / No")
- Supply problems and safety alerts: use ⚠️ emoji to flag affected presentations

### Namespaces

| Path | Namespace |
|---|---|
| `src/Models/Cima/` | `SaludMcp.Models.Cima` |
| `src/Clients/` | `SaludMcp.Clients` |
| `src/Tools/` | `SaludMcp.Tools` |
| `tests/` | `SaludMcp.Tests` |

---

## Development Workflow

1. Implement models (`Models/Cima/`)
2. Implement `CimaClient` with all endpoint methods
3. Implement tools in priority order
4. Write unit tests for client and tools
5. Build: `dotnet build salud-mcp.slnx`
6. Test: `dotnet test salud-mcp.slnx`
7. Run: `dotnet run --project src/`
8. Inspect end-to-end: `npx @modelcontextprotocol/inspector http://localhost:5000/mcp`

## NuGet Sources

This machine has a private NuGet feed at `http://192.168.1.39/PrivateNuGet/nuget` that may be unreachable.
When adding packages, always target nuget.org explicitly to avoid restore failures:

```bash
dotnet add package <PackageName> --source https://api.nuget.org/v3/index.json
```

## Environment

No environment variables required. The CIMA API is fully public.

Default Kestrel port: `5000` (HTTP) / `5001` (HTTPS).
To override: set `ASPNETCORE_URLS` or add `appsettings.json`.

---

## README Sections to Write (after implementation)

- What is salud-mcp
- Prerequisites (.NET 10 SDK)
- Installation
- Running the server (`dotnet run --project src/`)
- Usage with Claude Desktop (HTTP transport config)
- Usage with MCP Inspector
- Available tools (with examples)
- Contributing
- License (MIT)
