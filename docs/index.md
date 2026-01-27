# Blueprintr Documentation

Welcome to the official documentation for Blueprintr - a collection of production-ready boilerplate code libraries for C# projects.

## What is Blueprintr?

Blueprintr provides reusable, well-tested C# libraries that accelerate development of ASP.NET Core applications. All libraries are automatically published to NuGet with semantic versioning, comprehensive documentation, and quality assurance through CI/CD pipelines.

## Available Libraries

### Blueprintr.Endpoints

[![NuGet](https://img.shields.io/nuget/v/Blueprintr.Endpoints.svg)](https://www.nuget.org/packages/Blueprintr.Endpoints/)

Common utilities and patterns for building endpoints in ASP.NET Core applications.

**Quick Installation:**
```bash
dotnet add package Blueprintr.Endpoints
```

**Quick Usage:**
```csharp
using Blueprintr.Endpoints;

var endpointName = "/api/users".GetEndpointName();
// Returns: "api-users"
```

**API Documentation:** [Blueprintr.Endpoints API Reference](xref:Blueprintr.Endpoints)

## Documentation Overview

| Guide | Description |
|-------|-------------|
| **[Getting Started](getting-started.md)** | Installation, usage examples, and quick start guide |
| **[Development Guide](development-guide.md)** | Testing, versioning, documentation, and development workflow |
| **[Configuration Guide](configuration-guide.md)** | GitHub setup, NuGet publishing, CI/CD, and troubleshooting |
| **[API Reference](api/index.md)** | Complete API documentation generated from source code |

## Key Features

| Feature | Description |
|---------|-------------|
| **Automated NuGet Publishing** | Packages are automatically published on every push to main |
| **Semantic Versioning** | MinVer automatically calculates versions from git tags |
| **Quality Gates** | All warnings treated as errors, tests must pass before merge |
| **CI/CD Pipeline** | GitHub Actions for building, testing, and publishing |
| **Documentation Website** | DocFX-generated documentation with API reference |
| **Conventional Commits** | Standardized commit messages for clear history |
| **Trusted Publishing** | Secure, keyless NuGet authentication via OIDC |

## Quick Links

- **NuGet Package**: https://www.nuget.org/packages/Blueprintr.Endpoints/
- **GitHub Repository**: https://github.com/rafitajaen/blueprintr
- **GitHub Issues**: https://github.com/rafitajaen/blueprintr/issues
- **GitHub Discussions**: https://github.com/rafitajaen/blueprintr/discussions

## Project Structure

```
blueprintr/
├── src/                           # Source code
│   └── Blueprintr.Endpoints/      # NuGet library
├── tests/                         # Test projects
│   └── Blueprintr.Endpoints.Tests/ # NUnit tests
├── docs/                          # Documentation
├── .github/workflows/             # CI/CD pipelines
│   ├── ci.yml                     # Build + Test
│   ├── publish-nuget.yml          # NuGet publishing
│   └── documentation.yml          # Docs generation
└── CLAUDE.md                      # Project overview
```

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET 10.0 |
| Testing | NUnit |
| Documentation | DocFX |
| CI/CD | GitHub Actions |
| Package Registry | NuGet.org |
| Versioning | MinVer |
| License | AGPL-3.0-or-later |

## Getting Help

- **Bug Reports**: Open an issue on [GitHub Issues](https://github.com/rafitajaen/blueprintr/issues)
- **Questions**: Start a discussion on [GitHub Discussions](https://github.com/rafitajaen/blueprintr/discussions)
- **Documentation Issues**: Found an error? Create a PR or issue
- **Contributing**: See [Contributing Guidelines](../CONTRIBUTING.md)

## License

Blueprintr is licensed under the [GNU Affero General Public License v3.0 (AGPL-3.0-or-later)](https://github.com/rafitajaen/blueprintr/blob/main/LICENSE).

**Key Points:**
- Commercial use is allowed
- Modifications must be shared under the same license
- Network use (SaaS) requires source code disclosure
