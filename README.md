# Blueprintr

[![NuGet](https://img.shields.io/nuget/v/Blueprintr.svg)](https://www.nuget.org/packages/Blueprintr/)
[![Build Status](https://github.com/rafitajaen/blueprintr/workflows/CI%20-%20Build%20and%20Test/badge.svg)](https://github.com/rafitajaen/blueprintr/actions)
[![License: AGPL-3.0](https://img.shields.io/badge/License-AGPL%203.0-blue.svg)](LICENSE)

Collection of boilerplate code libraries for C# projects. Reusable libraries automatically published to NuGet.

## 📚 Available Libraries

### Blueprintr
Common utilities and patterns for building endpoints and configuring Entity Framework Core in ASP.NET Core applications.

```bash
dotnet add package Blueprintr
```

**Endpoint utilities:**
```csharp
using Blueprintr;

var name = "/api/users".GetEndpointName();
// Returns: "api-users"
```

**Entity Framework with PostgreSQL:**
```csharp
using Blueprintr.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with PostgreSQL
builder.AddDbContext<AppDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

var app = builder.Build();
app.UseDbContext<AppDbContext>();
app.Run();
```

## 🚀 Quick Start

See **[CLAUDE.md](CLAUDE.md)** for complete documentation including:
- Installation and setup
- Development workflow
- Testing guidelines
- Publishing process
- CI/CD pipeline
- Troubleshooting

## 🛠️ Development

```bash
# Build
dotnet build

# Test
dotnet test

# Package
dotnet pack --configuration Release
```

## 📦 Publishing Releases

### Development (Alpha versions)
Push to main - auto-publishes pre-release versions to NuGet:
```bash
git push origin main
# Publishes: 0.1.1-alpha.0.X to NuGet (no GitHub Release)
```

### Stable Release
Create and push a tag - publishes to NuGet **AND** creates GitHub Release:
```bash
git tag 0.1.0
git push origin 0.1.0
# Publishes: 0.1.0 to NuGet + creates GitHub Release
```

**See [CLAUDE.md](CLAUDE.md#-publishing-versions)** for complete versioning guide.

## 📖 Documentation

- **[CLAUDE.md](CLAUDE.md)** - Complete project guide
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Contribution guidelines
- **[API Documentation](https://rafitajaen.github.io/blueprintr/)** - Generated from code
- **[docs/](docs/)** - Additional documentation:
  - [Getting Started](docs/getting-started.md) - Installation and usage
  - [Development Guide](docs/development-guide.md) - Testing, versioning, documentation
  - [Configuration Guide](docs/configuration-guide.md) - GitHub, NuGet, CI/CD setup

## 🎯 Features

- ✅ Automated testing (NUnit)
- ✅ Automatic NuGet publishing
- ✅ Quality gates (warnings as errors)
- ✅ CI/CD with GitHub Actions
- ✅ Documentation website (DocFX)
- ✅ Conventional Commits
- ✅ VS Code configured

## 🤝 Contributing

1. Fork the repository
2. Create a branch: `git checkout -b feat/new-feature`
3. Make changes and add tests
4. Commit: `git commit -m "feat: add new feature"`
5. Push: `git push origin feat/new-feature`
6. Open a Pull Request

**Read [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.**

## 📋 Project Structure

```
blueprintr/
├── src/                    # NuGet libraries
├── tests/                  # Test projects
├── docs/                   # Documentation
├── .github/workflows/      # CI/CD pipelines
└── CLAUDE.md              # Complete guide
```

## 🔐 License

GNU Affero General Public License v3.0 (AGPL-3.0-or-later)

See [LICENSE](LICENSE) file for details.

## 📞 Support

- 📝 [GitHub Issues](https://github.com/rafitajaen/blueprintr/issues) - Bug reports & features
- 📖 [GitHub Discussions](https://github.com/rafitajaen/blueprintr/discussions) - Questions & ideas
- 📚 [Documentation](https://rafitajaen.github.io/blueprintr/) - API reference

---

**For complete setup and usage instructions, see [CLAUDE.md](CLAUDE.md)**
