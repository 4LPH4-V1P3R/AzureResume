# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Azure-hosted resume website with a visitor counter. Two-part architecture:

- **Frontend** (`AzureResumeFrontEnd/`): Static HTML/CSS/JS resume page hosted on Azure. Sections are collapsible (click to expand). Calls the backend API to display a visitor count.
- **Backend** (`AzureResumeBackEnd/api/`): .NET 8 Azure Functions v4 (isolated worker model) with a single HTTP-triggered function (`GetResumeCounter`) that reads/increments a visitor counter in Azure Cosmos DB.

## Build & Run Commands

### Backend (Azure Function)

All commands run from `AzureResumeBackEnd/api/`:

```bash
# Build
dotnet build

# Clean + build
dotnet clean && dotnet build

# Publish (Release)
dotnet clean --configuration Release && dotnet publish --configuration Release

# Run locally (requires Azure Functions Core Tools)
cd bin/Debug/net8.0 && func host start
```

The function runs at `http://localhost:7071/api/GetResumeCounter`.

### Frontend

Static files — open `AzureResumeFrontEnd/index.html` directly or serve with any HTTP server. The API URL in `main.js` (`functionApi` variable) must be updated to point to the deployed function URL for production.

## Key Architecture Details

- **CosmosDB integration**: `GetResumeCounter.cs` creates a `CosmosClient` using the `AzureConnectionString` environment variable. It reads/increments a counter document (id: `"1"`) in database `AzureResume`, container `Counter`.
- **Counter model**: `Counter.cs` — simple POCO with `id` and `count` fields, serialized with `System.Text.Json`.
- **Isolated worker model**: Uses `Microsoft.Azure.Functions.Worker` (not the in-process model). Entry point is `Program.cs` with `FunctionsApplication.CreateBuilder`.
- **Configuration**: `local.settings.json` holds `AzureConnectionString` for CosmosDB and is gitignored. `FUNCTIONS_WORKER_RUNTIME` is set to `dotnet-isolated`.
- **Background image**: The CSS references `Background3.jpg` in the frontend directory.

## Prerequisites

- .NET 8 SDK
- Azure Functions Core Tools v4
- VS Code extensions: `ms-azuretools.vscode-azurefunctions`, `ms-dotnettools.csharp`
