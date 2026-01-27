# Blueprintr

[![NuGet](https://img.shields.io/nuget/v/Blueprintr.Endpoints.svg)](https://www.nuget.org/packages/Blueprintr.Endpoints/)
[![Build Status](https://github.com/YOUR_USERNAME/blueprintr/workflows/CI%20-%20Build%20and%20Test/badge.svg)](https://github.com/YOUR_USERNAME/blueprintr/actions)
[![License: AGPL-3.0](https://img.shields.io/badge/License-AGPL%203.0-blue.svg)](LICENSE)

Collection of boilerplate code libraries for C# projects. Reusable libraries automatically published to NuGet.

## 📚 Available Libraries

### Blueprintr.Endpoints
Common utilities and patterns for building endpoints in ASP.NET Core applications.

```bash
dotnet add package Blueprintr.Endpoints
```

```csharp
using Blueprintr.Endpoints;

var name = "/api/users".GetEndpointName();
// Returns: "api-users"
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

## 📖 Documentation

- **[CLAUDE.md](CLAUDE.md)** - Complete project guide
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Contribution guidelines
- **[API Documentation](https://YOUR_USERNAME.github.io/blueprintr/)** - Generated from code
- **[docs/](docs/)** - Additional documentation

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

- 📝 [GitHub Issues](https://github.com/YOUR_USERNAME/blueprintr/issues) - Bug reports & features
- 📖 [GitHub Discussions](https://github.com/YOUR_USERNAME/blueprintr/discussions) - Questions & ideas
- 📚 [Documentation](https://YOUR_USERNAME.github.io/blueprintr/) - API reference

---

**For complete setup and usage instructions, see [CLAUDE.md](CLAUDE.md)**
