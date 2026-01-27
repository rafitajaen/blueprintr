# Getting Started

This guide will help you get started with Blueprintr libraries.

## Prerequisites

- .NET 10.0 SDK or later
- A C# project (Console, Web, Library, etc.)

## Installation

### Using .NET CLI

```bash
dotnet add package Blueprintr
```

### Using Package Manager Console

```powershell
Install-Package Blueprintr
```

### Using Visual Studio

1. Right-click on your project
2. Select "Manage NuGet Packages"
3. Search for "Blueprintr"
4. Click "Install"

## Blueprintr

### Overview

Blueprintr provides common utilities and patterns for building endpoints in ASP.NET Core applications.

### Basic Usage

```csharp
using Blueprintr;

public class MyEndpointService
{
    public string FormatEndpointName(string path)
    {
        // Convert "/api/users/profile" to "api-users-profile"
        return path.GetEndpointName();
    }
}
```

### API Reference

For complete API documentation, see [Blueprintr API](xref:Blueprintr).

## Next Steps

- Explore the [API Reference](api/index.md)
- Learn about the [development workflow](xref:../WORKFLOW.md)
- Contribute to the project - see [Contributing Guide](xref:../CONTRIBUTING.md)

## Examples

### Example 1: Endpoint Name Conversion

```csharp
using Blueprintr;

var endpoints = new[]
{
    "/api/users",
    "/api/products/categories",
    "/health"
};

foreach (var endpoint in endpoints)
{
    var name = endpoint.GetEndpointName();
    Console.WriteLine($"{endpoint} -> {name}");
}

// Output:
// /api/users -> api-users
// /api/products/categories -> api-products-categories
// /health -> health
```

## Troubleshooting

### Package Not Found

If you can't find the package:
1. Ensure you're using the correct package name
2. Check NuGet.org to verify the package is published
3. Try clearing your NuGet cache: `dotnet nuget locals all --clear`

### Build Errors

If you encounter build errors:
1. Verify you have .NET 10.0 SDK installed: `dotnet --version`
2. Clean and rebuild: `dotnet clean && dotnet build`
3. Check for compatibility issues with other packages

## Getting Help

- Check the [API documentation](api/index.md)
- Search [GitHub Issues](https://github.com/rafitajaen/blueprintr/issues)
- Ask in [GitHub Discussions](https://github.com/rafitajaen/blueprintr/discussions)
