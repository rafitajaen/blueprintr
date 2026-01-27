# Blueprintr

> Production-ready C# library project with automated testing, NuGet publishing, and quality gates.

## 🎯 Quick Overview

**Blueprintr** is a collection of reusable C# libraries automatically published to NuGet, designed to provide production-ready boilerplate code for common development patterns.

### Current Libraries

- **[Blueprintr.Endpoints](xref:Blueprintr.Endpoints)** - Endpoint utilities for ASP.NET Core

## 🚀 Quick Start

### Installation

```bash
dotnet add package Blueprintr.Endpoints
```

### Usage

```csharp
using Blueprintr.Endpoints;

var name = "/api/users".GetEndpointName();
// Returns: "api-users"
```

## 📚 Documentation

- **[Full Documentation](docs/index.md)** - Complete guides and tutorials
- **[API Reference](api/index.md)** - Detailed API documentation
- **[Getting Started](docs/getting-started.md)** - Step-by-step setup guide
- **[Contributing](CONTRIBUTING.md)** - Contribution guidelines

## ✨ Key Features

- ✅ Automated testing (45 passing tests)
- ✅ Automatic NuGet publishing
- ✅ Quality gates (warnings = errors)
- ✅ CI/CD with GitHub Actions
- ✅ Documentation website (DocFX)
- ✅ Conventional Commits
- ✅ AGPL-3.0 licensed

## 🔗 Links

- **NuGet Package**: [Blueprintr.Endpoints](https://www.nuget.org/packages/Blueprintr.Endpoints/)
- **GitHub Repository**: [rafitajaen/blueprintr](https://github.com/rafitajaen/blueprintr)
- **Issues**: [Report a bug](https://github.com/rafitajaen/blueprintr/issues)

## 📄 License

This project is licensed under the [AGPL-3.0 License](https://github.com/rafitajaen/blueprintr/blob/main/LICENSE).
