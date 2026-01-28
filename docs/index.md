# Blueprintr Documentation

Welcome to the official documentation for Blueprintr - a collection of production-ready boilerplate code libraries for C# projects.

## What is Blueprintr?

Blueprintr provides reusable, well-tested C# libraries that accelerate development of ASP.NET Core applications. All libraries are automatically published to NuGet with semantic versioning, comprehensive documentation, and quality assurance through CI/CD pipelines.

## Available Libraries

### Blueprintr

[![NuGet](https://img.shields.io/nuget/v/Blueprintr.svg)](https://www.nuget.org/packages/Blueprintr/)

Common utilities and patterns for building endpoints and configuring Entity Framework Core in ASP.NET Core applications.

**Quick Installation:**
```bash
dotnet add package Blueprintr
```

**Quick Usage - Endpoint Utilities:**
```csharp
using Blueprintr;

var endpointName = "/api/users".GetEndpointName();
// Returns: "api-users"
```

**Quick Usage - Entity Framework with PostgreSQL:**
```csharp
using Blueprintr.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with PostgreSQL (includes pooling, snake_case, NodaTime)
builder.AddDbContext<AppDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

var app = builder.Build();

// Apply migrations and seed database (optional seeding in Development)
app.UseDbContext<AppDbContext>();

app.Run();
```

**Features:**
- ✅ Endpoint path formatting utilities
- ✅ Entity Framework Core with PostgreSQL support
- ✅ Connection pooling and performance optimization
- ✅ Automatic migrations and optional seeding
- ✅ Snake case naming conventions for database tables

**API Documentation:** [Blueprintr API Reference](xref:Blueprintr)

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

- **NuGet Package**: https://www.nuget.org/packages/Blueprintr/
- **GitHub Repository**: https://github.com/rafitajaen/blueprintr
- **GitHub Issues**: https://github.com/rafitajaen/blueprintr/issues
- **GitHub Discussions**: https://github.com/rafitajaen/blueprintr/discussions

## Project Structure

```
blueprintr/
├── src/                           # Source code
│   └── Blueprintr/                # NuGet library
├── tests/                         # Test projects
│   └── Blueprintr.Tests/          # NUnit tests
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
