# Contributing to salud-mcp

Thank you for your interest in contributing!

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/<your-username>/SaludMCP.git`
3. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
4. Build: `dotnet build salud-mcp.slnx`
5. Run tests: `dotnet test salud-mcp.slnx`

## How to Contribute

### Reporting Bugs

Open an issue describing:
- What you did
- What you expected
- What actually happened
- The error message (if any)

### Suggesting New Tools

The CIMA API offers more endpoints than are currently implemented. Good candidates:

| Tool | Endpoint | Status |
|---|---|---|
| `search_in_technical_sheet` | `POST buscarEnFichaTecnica` | Help wanted |
| `get_safety_materials` | `GET materiales?nregistro=` | Help wanted |

Open an issue to discuss before implementing.

### Submitting a Pull Request

1. Create a feature branch: `git checkout -b feature/my-tool`
2. Follow the existing code style:
   - All code, comments, and identifiers in **English**
   - JSON property names stay as-is (CIMA API uses Spanish field names)
   - Tools return human-readable markdown strings
   - Handle `404`, `429`, and `TaskCanceledException` explicitly — never let raw exceptions reach the MCP layer
3. Add or update tests in `tests/salud-mcp.Tests/`
4. Ensure `dotnet build` and `dotnet test` pass with 0 errors and 0 warnings
5. Open a pull request with a clear description of the change

## Project Structure

```
src/Clients/       # CimaClient — one method per CIMA endpoint
src/Models/Cima/   # Strongly-typed response models (JSON property names preserved)
src/Tools/         # MCP tool classes — one class per domain
tests/             # xUnit tests
```

## Code Style

- Primary constructor syntax for DI (e.g. `public class MyTools(CimaClient cimaClient)`)
- Collection expressions (`[]` instead of `new List<T>()`)
- Nullable reference types enabled — use `?` and null-coalescing operators
- `async/await` throughout — no `.Result` or `.Wait()`
