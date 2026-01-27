# Blueprintr Documentation

Welcome to the Blueprintr documentation!

Blueprintr is a collection of boilerplate code libraries for C# projects. These reusable libraries are automatically published to NuGet, making it easy to add common functionality to your projects.

## Available Libraries

### Blueprintr.Endpoints
[![NuGet](https://img.shields.io/nuget/v/Blueprintr.Endpoints.svg)](https://www.nuget.org/packages/Blueprintr.Endpoints/)

Common utilities and patterns for building endpoints in ASP.NET Core applications.

**Installation:**
```bash
dotnet add package Blueprintr.Endpoints
```

**API Documentation:** [Blueprintr.Endpoints API](xref:Blueprintr.Endpoints)

## Getting Started

### Quick Start

1. **Install the package:**
   ```bash
   dotnet add package Blueprintr.Endpoints
   ```

2. **Use in your code:**
   ```csharp
   using Blueprintr.Endpoints;

   var endpointName = "/api/users".GetEndpointName();
   // Returns: "api-users"
   ```

3. **Explore the API:**
   - Browse the [API Reference](api/index.md)
   - Check out [examples and guides](getting-started.md)

## Documentation Sections

- **[Getting Started](getting-started.md)** - Step-by-step guides for each library
- **[API Reference](api/index.md)** - Complete API documentation
- **[Workflow Guide](xref:../WORKFLOW.md)** - Development and publishing workflow
- **[Contributing](xref:../CONTRIBUTING.md)** - How to contribute to Blueprintr

## Features

- ✅ Automatic NuGet publishing
- ✅ Semantic versioning with MinVer
- ✅ Multi-project support
- ✅ .NET 10.0 target framework
- ✅ Comprehensive documentation
- ✅ CI/CD with GitHub Actions

## Support

- 📝 [GitHub Issues](https://github.com/rafitajaen/blueprintr/issues) - Report bugs or request features
- 📖 [GitHub Discussions](https://github.com/rafitajaen/blueprintr/discussions) - Ask questions
- 🔗 [GitHub Repository](https://github.com/rafitajaen/blueprintr) - View source code

## License

Blueprintr is licensed under the [AGPL-3.0 License](https://github.com/rafitajaen/blueprintr/blob/main/LICENSE).
