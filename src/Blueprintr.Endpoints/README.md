# Blueprintr.Endpoints

Common utilities and patterns for building endpoints in ASP.NET Core applications.

## Installation

```bash
dotnet add package Blueprintr.Endpoints
```

## Usage

This library provides extension methods and utilities for working with endpoints in ASP.NET Core.

### Endpoint Name Extension

Convert endpoint paths to standardized names:

```csharp
using Blueprintr.Endpoints;

var endpointPath = "/api/users/profile";
var name = endpointPath.GetEndpointName();
// Returns: "api-users-profile"
```

## API Documentation

For complete API documentation, visit the [Blueprintr Documentation](https://YOUR_USERNAME.github.io/blueprint/).

## Features

- Extension methods for endpoint manipulation
- Utilities for common endpoint patterns
- Fully documented with XML comments
- .NET 10.0 target framework

## Examples

### Basic Usage

```csharp
using Blueprintr.Endpoints;

public class EndpointRegistry
{
    public void RegisterEndpoint(string path)
    {
        var name = path.GetEndpointName();
        Console.WriteLine($"Registering endpoint: {name}");
    }
}
```

## Contributing

Contributions are welcome! Please read the [Contributing Guide](https://github.com/YOUR_USERNAME/blueprintr/blob/main/CONTRIBUTING.md).

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [LICENSE](https://github.com/YOUR_USERNAME/blueprintr/blob/main/LICENSE) file for details.

## Support

- [Documentation](https://YOUR_USERNAME.github.io/blueprint/)
- [GitHub Issues](https://github.com/YOUR_USERNAME/blueprintr/issues)
- [GitHub Discussions](https://github.com/YOUR_USERNAME/blueprintr/discussions)
