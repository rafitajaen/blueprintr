# Getting Started

This comprehensive guide will help you get started with Blueprintr libraries, from installation through advanced usage patterns.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Installation](#installation)
3. [Basic Usage](#basic-usage)
4. [Code Examples](#code-examples)
5. [Best Practices](#best-practices)
6. [Troubleshooting](#troubleshooting)
7. [Next Steps](#next-steps)

---

## Prerequisites

Before using Blueprintr libraries, ensure you have the following installed:

### Required

| Requirement | Version | How to Check | Download |
|-------------|---------|--------------|----------|
| .NET SDK | 10.0 or later | `dotnet --version` | [Download .NET](https://dotnet.microsoft.com/download) |
| C# Project | Console, Web, Library, etc. | N/A | N/A |

### Optional (for Development)

| Tool | Purpose | Installation |
|------|---------|--------------|
| Visual Studio | IDE with NuGet GUI | [Download VS](https://visualstudio.microsoft.com/) |
| Visual Studio Code | Lightweight editor | [Download VS Code](https://code.visualstudio.com/) |
| JetBrains Rider | Cross-platform IDE | [Download Rider](https://www.jetbrains.com/rider/) |

### Verify Installation

```bash
# Check .NET SDK version
dotnet --version

# Expected output: 10.0.x or higher
```

---

## Installation

There are multiple ways to install Blueprintr packages. Choose the method that best fits your workflow.

### Method 1: .NET CLI (Recommended)

The simplest and most universal method:

```bash
# Navigate to your project directory
cd /path/to/your/project

# Install Blueprintr.Endpoints
dotnet add package Blueprintr.Endpoints
```

**For specific versions:**
```bash
# Install a specific stable version
dotnet add package Blueprintr.Endpoints --version 1.0.0

# Install the latest pre-release version
dotnet add package Blueprintr.Endpoints --prerelease
```

### Method 2: Package Manager Console (Visual Studio)

If you're using Visual Studio:

```powershell
# Install latest stable version
Install-Package Blueprintr.Endpoints

# Install specific version
Install-Package Blueprintr.Endpoints -Version 1.0.0

# Install pre-release version
Install-Package Blueprintr.Endpoints -Prerelease
```

### Method 3: Visual Studio NuGet GUI

1. Right-click on your project in Solution Explorer
2. Select **"Manage NuGet Packages"**
3. Click the **"Browse"** tab
4. Search for **"Blueprintr.Endpoints"**
5. Select the package and click **"Install"**
6. Review and accept the license

**Note:** Check **"Include prerelease"** to see alpha/beta versions.

### Method 4: Manual .csproj Edit

Add directly to your project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Blueprintr.Endpoints" Version="1.0.0" />
  </ItemGroup>
</Project>
```

Then restore packages:
```bash
dotnet restore
```

### Verify Installation

After installation, verify the package is correctly installed:

```bash
# List installed packages
dotnet list package

# Expected output includes:
# > Blueprintr.Endpoints    1.0.0
```

---

## Basic Usage

### Blueprintr.Endpoints

The Blueprintr.Endpoints library provides utilities for working with endpoint paths in ASP.NET Core applications.

#### Import the Namespace

```csharp
using Blueprintr.Endpoints;
```

#### GetEndpointName Extension Method

Converts an endpoint path to a standardized, URL-friendly name:

```csharp
using Blueprintr.Endpoints;

public class MyService
{
    public string FormatEndpointName(string path)
    {
        // Convert "/api/users/profile" to "api-users-profile"
        return path.GetEndpointName();
    }
}
```

**Transformation Rules:**
- Removes leading slashes (`/api` becomes `api`)
- Replaces internal slashes with hyphens (`/api/users` becomes `api-users`)
- Preserves other characters
- Case is preserved

---

## Code Examples

### Example 1: Basic Endpoint Name Conversion

```csharp
using Blueprintr.Endpoints;

// Single conversion
var name = "/api/users".GetEndpointName();
Console.WriteLine(name);  // Output: api-users
```

### Example 2: Multiple Endpoint Processing

```csharp
using Blueprintr.Endpoints;

var endpoints = new[]
{
    "/api/users",
    "/api/products/categories",
    "/api/orders/history/recent",
    "/health",
    "/metrics"
};

foreach (var endpoint in endpoints)
{
    var name = endpoint.GetEndpointName();
    Console.WriteLine($"{endpoint,-35} -> {name}");
}

// Output:
// /api/users                          -> api-users
// /api/products/categories            -> api-products-categories
// /api/orders/history/recent          -> api-orders-history-recent
// /health                             -> health
// /metrics                            -> metrics
```

### Example 3: ASP.NET Core Minimal API Integration

```csharp
using Blueprintr.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Use GetEndpointName for endpoint metadata
app.MapGet("/api/users", () => Results.Ok(new[] { "Alice", "Bob" }))
   .WithName("/api/users".GetEndpointName())
   .WithTags("Users");

app.MapGet("/api/products", () => Results.Ok(new[] { "Widget", "Gadget" }))
   .WithName("/api/products".GetEndpointName())
   .WithTags("Products");

app.Run();
```

### Example 4: Controller-Based API Integration

```csharp
using Blueprintr.Endpoints;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var endpointName = "/api/users".GetEndpointName();
        _logger.LogInformation("Endpoint {EndpointName} called", endpointName);

        return Ok(new[] { "User1", "User2" });
    }
}
```

### Example 5: Dynamic Route Registration

```csharp
using Blueprintr.Endpoints;

public static class EndpointRegistration
{
    public static void RegisterEndpoints(WebApplication app, string[] routes)
    {
        foreach (var route in routes)
        {
            var endpointName = route.GetEndpointName();

            app.MapGet(route, () => Results.Ok(new { route, name = endpointName }))
               .WithName(endpointName);
        }
    }
}

// Usage:
var routes = new[] { "/api/users", "/api/products", "/api/orders" };
EndpointRegistration.RegisterEndpoints(app, routes);
```

### Example 6: OpenAPI/Swagger Operation IDs

```csharp
using Blueprintr.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Use consistent operation IDs
app.MapGet("/api/users/{id}", (int id) => Results.Ok(new { id, name = "User" }))
   .WithName($"{"/api/users".GetEndpointName()}-get-by-id")
   .WithOpenApi();

app.Run();
```

---

## Best Practices

### 1. Consistent Naming

Use `GetEndpointName()` consistently across your application for all endpoint-related naming:

```csharp
// Good: Consistent naming
var endpoints = new Dictionary<string, Delegate>
{
    { "/api/users".GetEndpointName(), GetUsers },
    { "/api/products".GetEndpointName(), GetProducts },
};

// Avoid: Mixing manual and automated naming
```

### 2. Centralize Endpoint Definitions

Define endpoints in a central location for maintainability:

```csharp
public static class Routes
{
    public const string Users = "/api/users";
    public const string Products = "/api/products";
    public const string Orders = "/api/orders";

    public static string GetName(string route) => route.GetEndpointName();
}

// Usage:
app.MapGet(Routes.Users, GetUsers)
   .WithName(Routes.GetName(Routes.Users));
```

### 3. Logging with Endpoint Names

Include endpoint names in logs for easier debugging:

```csharp
public class LoggingMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value ?? "";
        var endpointName = path.GetEndpointName();

        _logger.LogInformation(
            "Request started: {EndpointName} at {Path}",
            endpointName,
            path
        );

        await next(context);
    }
}
```

### 4. Unit Testing

Test your endpoint name transformations:

```csharp
using Blueprintr.Endpoints;
using NUnit.Framework;

[TestFixture]
public class EndpointNamingTests
{
    [TestCase("/api/users", "api-users")]
    [TestCase("/api/v1/products", "api-v1-products")]
    [TestCase("/health", "health")]
    public void GetEndpointName_VariousPaths_ReturnsExpected(string input, string expected)
    {
        var result = input.GetEndpointName();
        Assert.That(result, Is.EqualTo(expected));
    }
}
```

---

## Troubleshooting

### Package Not Found

**Symptom:** Package search returns no results or installation fails.

**Solutions:**

1. **Verify package name:**
   ```bash
   # Correct
   dotnet add package Blueprintr.Endpoints

   # Incorrect
   dotnet add package Blueprintr  # Old package name
   ```

2. **Clear NuGet cache:**
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   ```

3. **Check NuGet source:**
   ```bash
   dotnet nuget list source
   # Should include: https://api.nuget.org/v3/index.json
   ```

4. **Add NuGet source if missing:**
   ```bash
   dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
   ```

### Build Errors After Installation

**Symptom:** Project fails to build after adding the package.

**Solutions:**

1. **Check .NET version compatibility:**
   ```bash
   dotnet --version
   # Must be 10.0.x or higher
   ```

2. **Restore packages:**
   ```bash
   dotnet restore
   ```

3. **Clean and rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```

4. **Update package to latest:**
   ```bash
   dotnet add package Blueprintr.Endpoints --version "*"
   ```

### Extension Method Not Found

**Symptom:** `GetEndpointName()` method not recognized.

**Solutions:**

1. **Add the using directive:**
   ```csharp
   using Blueprintr.Endpoints;
   ```

2. **Verify installation:**
   ```bash
   dotnet list package | grep Blueprintr
   ```

3. **Rebuild the project:**
   ```bash
   dotnet build
   ```

### Version Conflicts

**Symptom:** Dependency version conflicts during restore.

**Solutions:**

1. **Check installed versions:**
   ```bash
   dotnet list package --include-transitive
   ```

2. **Update all packages:**
   ```bash
   dotnet outdated  # If dotnet-outdated tool is installed
   ```

3. **Force specific version:**
   ```xml
   <PackageReference Include="Blueprintr.Endpoints" Version="[1.0.0]" />
   ```

---

## Next Steps

Now that you have Blueprintr installed and understand the basics, explore these resources:

### Explore More

- **[API Reference](api/index.md)** - Complete API documentation with all methods and parameters
- **[Development Guide](development-guide.md)** - Learn about testing, versioning, and contributing
- **[Configuration Guide](configuration-guide.md)** - Set up CI/CD and NuGet publishing

### Contribute

Interested in contributing? Here's how:

1. **Report Issues**: Found a bug? [Open an issue](https://github.com/rafitajaen/blueprintr/issues)
2. **Request Features**: Have an idea? [Start a discussion](https://github.com/rafitajaen/blueprintr/discussions)
3. **Submit Code**: See [Contributing Guidelines](../CONTRIBUTING.md)

### Stay Updated

- **Watch the Repository**: Get notified of new releases
- **Check NuGet**: New versions appear at https://www.nuget.org/packages/Blueprintr.Endpoints/
- **Read Release Notes**: Each release includes detailed change information

---

## Quick Reference

| Action | Command |
|--------|---------|
| Install package | `dotnet add package Blueprintr.Endpoints` |
| Install specific version | `dotnet add package Blueprintr.Endpoints --version 1.0.0` |
| Install prerelease | `dotnet add package Blueprintr.Endpoints --prerelease` |
| List packages | `dotnet list package` |
| Update package | `dotnet add package Blueprintr.Endpoints` |
| Remove package | `dotnet remove package Blueprintr.Endpoints` |
| Clear cache | `dotnet nuget locals all --clear` |

---

**Questions?** Open an issue on [GitHub](https://github.com/rafitajaen/blueprintr/issues) or start a [discussion](https://github.com/rafitajaen/blueprintr/discussions).
