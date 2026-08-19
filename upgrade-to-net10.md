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

#### `IdentityModel.AspNetCore` 4.3.0 — ARCHIVED, not resolvable

This package is no longer maintained. Replace with one of:
- `Duende.AccessTokenManagement` — drop-in for token management
- `IdentityModel` v7+ — lower-level primitives

```bash
dotnet remove src/api/api.csproj package IdentityModel.AspNetCore
dotnet add src/api/api.csproj package Duende.AccessTokenManagement --version <latest>
```

Update any `using IdentityModel.AspNetCore.*` namespaces and DI registrations accordingly.

#### `NEST` 7.17.5 — DEPRECATED

NEST v7 targets Elasticsearch 7.x and is deprecated. The replacement is the official `Elastic.Clients.Elasticsearch` package.

Options:
- **Migrate now:** Replace `NEST` with `Elastic.Clients.Elasticsearch` (breaking API changes — plan a separate PR)
- **Defer:** Verify NEST 7.17.5 compiles on net10.0 and treat migration as follow-up tech debt

```bash
# To migrate:
dotnet remove src/api/api.csproj package NEST
dotnet add src/api/api.csproj package Elastic.Clients.Elasticsearch --version <latest>
```

#### `Swashbuckle.AspNetCore` 6 → 10 — MAJOR version

Middleware registration changed significantly between v6 and v10. Alternatively, replace Swashbuckle entirely with ASP.NET Core's built-in OpenAPI support (available since .NET 9, no extra package needed):

```bash
dotnet remove src/api/api.csproj package Swashbuckle.AspNetCore
dotnet add src/api/api.csproj package Microsoft.AspNetCore.OpenApi --version 10.0.11
```

In `Program.cs` / `Startup.cs`, replace:
```csharp
// Before (Swashbuckle)
services.AddSwaggerGen();
app.UseSwagger();
app.UseSwaggerUI();

// After (built-in)
services.AddOpenApi();
app.MapOpenApi();
```

### 1.5 Build and verify

```bash
dotnet restore src/api/api.csproj
dotnet build src/api/api.csproj
```

---

## Step 2 — `api.Tests` project

**File:** `aspnetcore/src/api.Tests/api.Tests.csproj`

*Execute only after Step 1 builds successfully.*

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

## Step 3 — Dockerfile

**File:** `aspnetcore/openshift/api/rahti2/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
...
FROM mcr.microsoft.com/dotnet/aspnet:10.0
```

---

## Step 4 — GitHub Actions workflow

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

- [ ] .NET 10 SDK installed
- [ ] `api` TargetFramework = `net10.0`
- [ ] `api.Tests` TargetFramework = `net10.0`
- [ ] `IdentityModel.AspNetCore` replaced
- [ ] `NEST` compatibility confirmed or migrated
- [ ] Swashbuckle upgraded or replaced with built-in OpenAPI
- [ ] `dotnet build aspnetcore/mydata.sln` succeeds with no errors
- [ ] `dotnet test aspnetcore/mydata.sln` passes
- [ ] Dockerfile base images updated to `10.0`
- [ ] GitHub Actions workflow uses `dotnet-version: 10.0.x`
- [ ] CI pipeline passes
