# Upgrade Plan: net8.0 → net10.0

## Prerequisites

Install the .NET 10 SDK (currently only `8.0.415` is installed):

```bash
# Download from https://dot.net/download or via brew
brew install --cask dotnet-sdk  # or download the installer
dotnet --list-sdks              # confirm 10.x appears
```

---

## Step 1 — `api` project

**File:** `aspnetcore/src/api/api.csproj`

### 1.1 Bump TargetFramework

```xml
<TargetFramework>net10.0</TargetFramework>
```

### 1.2 Fix stale DocumentationFile paths (still reference net5.0)

Change both `DocumentationFile` entries from:
```xml
<DocumentationFile>bin\Debug\net5.0\api.xml</DocumentationFile>
<DocumentationFile>bin\Release\net5.0\api.xml</DocumentationFile>
```
To:
```xml
<DocumentationFile>bin\Debug\net10.0\api.xml</DocumentationFile>
<DocumentationFile>bin\Release\net10.0\api.xml</DocumentationFile>
```

### 1.3 Update NuGet packages

```bash
cd aspnetcore
dotnet add src/api/api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.11
dotnet add src/api/api.csproj package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore --version 10.0.11
dotnet add src/api/api.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.11
dotnet add src/api/api.csproj package Microsoft.EntityFrameworkCore.Design --version 10.0.11
dotnet add src/api/api.csproj package Microsoft.Extensions.Configuration --version 10.0.11
dotnet add src/api/api.csproj package Serilog --version 4.4.0
dotnet add src/api/api.csproj package Serilog.AspNetCore --version 10.0.0
dotnet add src/api/api.csproj package Serilog.Settings.Configuration --version 10.0.1
dotnet add src/api/api.csproj package Serilog.Sinks.Http --version 9.2.1
dotnet add src/api/api.csproj package Dapper --version 2.1.79
dotnet add src/api/api.csproj package OpenAI --version 2.13.0
dotnet add src/api/api.csproj package Swashbuckle.AspNetCore --version 10.2.3
```

### 1.4 Packages requiring manual migration

#### `IdentityModel.AspNetCore` 4.3.0 — KEPT (incorrectly flagged as unused)

`IdentityModel.Client` is imported in `Startup.cs` and `AddClientAccessTokenManagement` / `AddClientAccessTokenHttpClient` extension methods are actively used. The package targets `netstandard2.0` and is compatible with net10.0 — no action required.

#### `NEST` 7.17.5 — DEFERRED (tech debt)

Keeping NEST 7.17.5 for now. It targets `netstandard2.0` and is compatible with net10.0. Migration to `Elastic.Clients.Elasticsearch` is tracked as a follow-up.

#### `Swashbuckle.AspNetCore` → replaced with `Microsoft.AspNetCore.OpenApi`

Replaced Swashbuckle with ASP.NET Core's built-in OpenAPI support (`Microsoft.AspNetCore.OpenApi` 10.0.11).

Changes made in `Startup.cs`:
- Removed `using Microsoft.OpenApi.Models`, `using System.Reflection`, `using System.IO`
- Replaced `services.AddSwaggerGen(...)` with `services.AddOpenApi()`
- Replaced `app.UseSwagger(); app.UseSwaggerUI();` with `endpoints.MapOpenApi()` inside `UseEndpoints`

### 1.5 Build and verify

```bash
dotnet restore src/api/api.csproj
dotnet build src/api/api.csproj
```

---

## Step 2 — `api.Tests` project ✅

**File:** `aspnetcore/src/api.Tests/api.Tests.csproj`

*Completed. Build succeeded; 309/309 tests pass.*

### 2.1 Bump TargetFramework

```xml
<TargetFramework>net10.0</TargetFramework>
```

### 2.2 Update NuGet packages

```bash
dotnet add src/api.Tests/api.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory --version 10.0.11
dotnet add src/api.Tests/api.Tests.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.11
dotnet add src/api.Tests/api.Tests.csproj package Microsoft.NET.Test.Sdk --version 18.9.0
dotnet add src/api.Tests/api.Tests.csproj package xunit --version 2.9.3
dotnet add src/api.Tests/api.Tests.csproj package xunit.runner.visualstudio --version 4.0.0
dotnet add src/api.Tests/api.Tests.csproj package coverlet.collector --version 10.0.1
```

> **Note:** `xunit.runner.visualstudio` 4.0.0 is a major version bump — verify test discovery still works in VS Code after updating.

### 2.3 Build and test

```bash
dotnet restore src/api.Tests/api.Tests.csproj
dotnet build src/api.Tests/api.Tests.csproj
dotnet test src/api.Tests/api.Tests.csproj
```

---

## Step 3 — Dockerfile ✅

**File:** `aspnetcore/openshift/api/rahti2/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
...
FROM mcr.microsoft.com/dotnet/aspnet:10.0
```

---

## Step 4 — GitHub Actions workflow ✅

**File:** `.github/workflows/dotnet.yml`

```yaml
- uses: actions/setup-dotnet@v4   # bump from v3
  with:
    dotnet-version: 10.0.x        # was 8.0.x
```

---

## Step 5 — Optional: modernize hosting model

`Startup.cs` uses the pre-.NET 6 hosting pattern. Migrating to `WebApplication.CreateBuilder` in `Program.cs` is not required for net10.0 to compile, but is the current standard and simplifies the codebase.

This is a non-trivial refactor and should be done in a separate PR after the framework upgrade is stable.

---

## Validation checklist

- [x] .NET 10 SDK installed (`10.0.400`)
- [x] `api` TargetFramework = `net10.0`
- [x] `api.Tests` TargetFramework = `net10.0`
- [x] `IdentityModel.AspNetCore` kept (was incorrectly flagged as unused — actively used)
- [x] `NEST` deferred — kept 7.17.5 for net10.0 compatibility, migration tracked as follow-up
- [x] Swashbuckle replaced with `Microsoft.AspNetCore.OpenApi`
- [x] `dotnet build aspnetcore/mydata.sln` succeeds with no errors
- [x] `dotnet test aspnetcore/mydata.sln` passes (309/309)
- [x] Dockerfile base images updated to `10.0`
- [x] GitHub Actions workflow uses `dotnet-version: 10.0.x`
- [ ] CI pipeline passes
