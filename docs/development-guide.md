# Development Guide

This comprehensive guide covers everything you need to know for developing, testing, versioning, and documenting Blueprintr libraries.

## Table of Contents

1. [Testing](#testing)
   - [Testing Strategy](#testing-strategy)
   - [Writing Tests](#writing-tests)
   - [Running Tests](#running-tests)
   - [Code Coverage](#code-coverage)
   - [Quality Gates](#quality-gates)
   - [Test Best Practices](#test-best-practices)
2. [Versioning](#versioning)
   - [How MinVer Works](#how-minver-works)
   - [Version Calculation](#version-calculation)
   - [Creating Versions](#creating-versions)
   - [Pre-release Workflow](#pre-release-workflow)
   - [Stable Release Process](#stable-release-process)
3. [Documentation](#documentation)
   - [DocFX Overview](#docfx-overview)
   - [Writing XML Documentation](#writing-xml-documentation)
   - [Building Documentation Locally](#building-documentation-locally)
   - [Documentation Best Practices](#documentation-best-practices)
4. [Development Workflow](#development-workflow)
   - [Feature Development](#feature-development)
   - [Adding New Libraries](#adding-new-libraries)
   - [Commit Message Format](#commit-message-format)
5. [Troubleshooting](#troubleshooting)

---

## Testing

### Testing Strategy

Blueprintr uses **NUnit** as the testing framework with strict quality standards.

#### Test Organization

| Test Type | Purpose | Location |
|-----------|---------|----------|
| Unit Tests | Test individual methods and classes in isolation | `tests/{Library}.Tests/` |
| Integration Tests | Test interactions between components | `tests/{Library}.Tests/` |
| Parametrized Tests | Test multiple scenarios efficiently | Within test classes |

#### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Test Project | `{Library}.Tests` | `Blueprintr.Tests` |
| Test Class | `{ClassName}Tests` | `EndpointExtensionsTests` |
| Test Method | `MethodName_Scenario_ExpectedBehavior` | `GetEndpointName_WithLeadingSlash_RemovesSlash` |

### Writing Tests

#### Basic Test Structure

Follow the **Arrange-Act-Assert (AAA)** pattern:

```csharp
namespace Blueprintr.Tests;

[TestFixture]
public class EndpointExtensionsTests
{
    [Test]
    public void GetEndpointName_WithValidPath_ReturnsFormattedName()
    {
        // Arrange - Set up test data and preconditions
        var path = "/api/users";

        // Act - Execute the method being tested
        var result = path.GetEndpointName();

        // Assert - Verify the result
        Assert.That(result, Is.EqualTo("api-users"));
    }
}
```

#### Parametrized Tests

Test multiple scenarios efficiently:

```csharp
[TestCase("/api/users", "api-users")]
[TestCase("/api/products/categories", "api-products-categories")]
[TestCase("/health", "health")]
[TestCase("", "")]
public void GetEndpointName_VariousPaths_ReturnsExpectedNames(string path, string expected)
{
    // Act
    var result = path.GetEndpointName();

    // Assert
    Assert.That(result, Is.EqualTo(expected));
}
```

#### Testing Exceptions

```csharp
[Test]
public void GetEndpointName_WithNull_ThrowsArgumentNullException()
{
    // Arrange
    string? endpointPath = null;

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => endpointPath!.GetEndpointName());
}
```

#### Async Tests

```csharp
[Test]
public async Task GetDataAsync_ValidId_ReturnsData()
{
    // Arrange
    var service = new DataService();
    var id = 1;

    // Act
    var result = await service.GetDataAsync(id);

    // Assert
    Assert.That(result, Is.Not.Null);
}
```

### Running Tests

#### Local Execution

```bash
# Run all tests
dotnet test

# Run in Release mode (matches CI)
dotnet test --configuration Release

# Run with verbose output
dotnet test --verbosity detailed

# Run specific project
dotnet test tests/Blueprintr.Tests/

# Run tests matching a filter
dotnet test --filter "FullyQualifiedName~GetEndpointName"

# Run with specific timeout
dotnet test --timeout 30000
```

#### CI/CD Execution

Tests run automatically in two scenarios:

| Scenario | Workflow | Trigger |
|----------|----------|---------|
| Pull Requests | `ci.yml` | Push to PR targeting main/develop |
| Before Publishing | `publish-nuget.yml` | Push to main with src/ changes |

### Code Coverage

#### Generating Coverage Locally

```bash
# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Install report generator (first time only)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report

# Open report
xdg-open ./coverage/report/index.html  # Linux
open ./coverage/report/index.html      # macOS
start ./coverage/report/index.html     # Windows
```

#### Coverage Goals

| Scope | Target | Priority |
|-------|--------|----------|
| Overall | 80%+ | Required |
| Core Logic | 90%+ | High |
| Public APIs | 100% | Critical |

### Quality Gates

The following standards are **enforced automatically**:

| Gate | Description | Enforcement |
|------|-------------|-------------|
| Tests Must Pass | All tests must succeed | CI blocks merge on failure |
| Warnings as Errors | Build warnings fail the build | `TreatWarningsAsErrors=true` |
| Code Must Build | Release configuration build must succeed | CI validates |
| Branch Protection | PRs required for main | GitHub settings |

**Suppressed Warnings:**
- `NU1510`, `NU1608` - NuGet version warnings
- `CS1591` - Missing XML documentation (optional for internal code)

### Test Best Practices

#### Do's

| Practice | Example |
|----------|---------|
| Test one thing per test | Single assertion focus |
| Independent tests | No shared mutable state |
| Clear names | `MethodName_Scenario_ExpectedBehavior` |
| Fast tests | < 100ms per test |
| Edge cases | Test boundaries and null values |
| Arrange-Act-Assert | Clear test structure |

#### Don'ts

| Anti-Pattern | Why |
|--------------|-----|
| Test dependencies | Tests must run in any order |
| Hard-coded paths | Use relative paths or fixtures |
| External dependencies | Mock external services |
| Thread.Sleep | Use async/await properly |
| Console.WriteLine | Use test output helpers |
| Randomness without seeding | Tests become non-deterministic |

---

## Versioning

### How MinVer Works

Blueprintr uses **MinVer** for automatic semantic versioning based on Git tags.

**Key Benefits:**
- No manual version management in `.csproj` files
- Versions calculated from Git history
- Pre-release versions auto-increment
- Works seamlessly with CI/CD

### Version Calculation

| Scenario | Version Format | Example |
|----------|----------------|---------|
| No tags, N commits | `0.0.0-alpha.0.N` | `0.0.0-alpha.0.5` |
| After tag `1.0.0`, N commits | `1.0.1-alpha.0.N` | `1.0.1-alpha.0.3` |
| Exactly on tag `1.0.0` | `1.0.0` | `1.0.0` |
| Pre-release tag | `1.0.0-beta.1` | `1.0.0-beta.1` |

### Creating Versions

#### Check Current Version

```bash
# Install MinVer CLI (first time only)
dotnet tool install --global minver-cli

# Check calculated version
minver

# With detailed output
minver -v d
```

#### Semantic Version Rules

| Change Type | From | To | When to Use |
|-------------|------|-----|-------------|
| **Patch** | 1.0.0 | 1.0.1 | Bug fixes, no new features |
| **Minor** | 1.0.0 | 1.1.0 | New features, backwards compatible |
| **Major** | 1.0.0 | 2.0.0 | Breaking API changes |

#### Creating Tags

```bash
# Patch version (bug fixes)
git tag 1.0.1
git push origin 1.0.1

# Minor version (new features)
git tag 1.1.0
git push origin 1.1.0

# Major version (breaking changes)
git tag 2.0.0
git push origin 2.0.0

# Pre-release versions
git tag 1.0.0-beta.1
git push origin 1.0.0-beta.1

git tag 1.0.0-rc.1
git push origin 1.0.0-rc.1
```

### Pre-release Workflow

During active development (no stable tags):

1. Every commit generates `0.0.0-alpha.0.X`
2. X increments with each commit
3. Packages are marked "pre-release" on NuGet
4. Users must check "Include prerelease" to see them

**Advantages:**
- No version management needed
- Automatic publishing on every change
- No version conflicts (each commit = unique version)
- Clear indication of unstable builds

### Stable Release Process

When ready for a stable release:

```bash
# 1. Ensure all changes are committed and pushed
git status  # Should be clean
git push origin main

# 2. Check current calculated version
minver
# Output: 0.0.0-alpha.0.X or 1.0.1-alpha.0.X

# 3. Create semantic version tag
git tag 1.0.0

# 4. Push the tag
git push origin 1.0.0
```

**What Happens:**
1. GitHub Actions detects the tag
2. CI workflow runs (build + test)
3. If tests pass:
   - NuGet publish workflow publishes `1.0.0`
   - Documentation workflow publishes docs
4. Package appears on NuGet as stable

**After Release:**
- Next commit generates `1.0.1-alpha.0.1`
- Create `1.0.1` tag for patch, or `1.1.0` for features

---

## Documentation

### DocFX Overview

Blueprintr uses **DocFX** for documentation generation.

**Features:**
- Automatic API documentation from XML comments
- Markdown articles for guides
- Modern, searchable website
- GitHub Pages deployment

### Writing XML Documentation

#### Document All Public APIs

```csharp
namespace Blueprintr;

/// <summary>
/// Extension methods for endpoint path manipulation.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Converts an endpoint path to a standardized name.
    /// </summary>
    /// <param name="endpointPath">The endpoint path to convert (e.g., "/api/users").</param>
    /// <returns>A standardized endpoint name with slashes replaced by hyphens.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="endpointPath"/> is null.</exception>
    /// <example>
    /// <code>
    /// var name = "/api/users".GetEndpointName();
    /// // Returns: "api-users"
    /// </code>
    /// </example>
    /// <remarks>
    /// This method removes leading slashes and replaces internal slashes with hyphens.
    /// Empty strings return empty strings. The original case is preserved.
    /// </remarks>
    /// <seealso cref="StringExtensions"/>
    public static string GetEndpointName(this string endpointPath)
    {
        ArgumentNullException.ThrowIfNull(endpointPath);
        return endpointPath.TrimStart('/').Replace('/', '-');
    }
}
```

#### XML Documentation Tags Reference

| Tag | Purpose | Example |
|-----|---------|---------|
| `<summary>` | Brief description | Required for all public members |
| `<param>` | Parameter description | `<param name="path">The path to convert</param>` |
| `<returns>` | Return value description | `<returns>The formatted name</returns>` |
| `<exception>` | Document exceptions | `<exception cref="ArgumentNullException">When null</exception>` |
| `<example>` | Code examples | Include usage demonstrations |
| `<remarks>` | Additional details | Edge cases, performance notes |
| `<seealso>` | Related members | `<seealso cref="OtherClass"/>` |
| `<typeparam>` | Generic type parameter | For generic methods/classes |

### Building Documentation Locally

```bash
# Install DocFX (first time only)
dotnet tool install -g docfx

# Generate API metadata from XML comments
docfx metadata

# Build the documentation site
docfx build

# Serve locally to preview
docfx serve _site

# Open in browser
# http://localhost:8080
```

### Documentation Best Practices

| Practice | Description |
|----------|-------------|
| Document all public APIs | Every public class, method, property |
| Include examples | Show real usage in `<example>` tags |
| Explain parameters | Describe expected values and constraints |
| Document exceptions | List all exceptions that can be thrown |
| Keep it current | Update docs when changing code |
| Use cross-references | Link related APIs with `<seealso>` |

---

## Development Workflow

### Feature Development

#### Step-by-Step Process

```bash
# 1. Create feature branch (use conventional prefixes)
git checkout -b feat/new-feature

# 2. Make changes in src/
code src/Blueprintr/NewFeature.cs

# 3. Add tests
code tests/Blueprintr.Tests/NewFeatureTests.cs

# 4. Run tests locally
dotnet test

# 5. Commit with conventional format
git commit -m "feat: add new feature"

# 6. Push and create PR
git push origin feat/new-feature
```

#### CI/CD Flow

```
Code Change
    |
Commit (Conventional Commits)
    |
Push to Branch
    |
Create Pull Request
    |
CI Workflow Triggers
    |- Restore
    |- Build (warnings as errors)
    |- Test
    |- Coverage
    |
Tests Pass? -- Yes --> Review & Merge Allowed
         |
         `-- No ---> Merge Blocked, Fix Required
    |
Merge to Main
    |
Publish Workflow Triggers
    |- Run CI First (must pass)
    |- Build Packages
    |- Version with MinVer
    |- Publish to NuGet
```

### Adding New Libraries

#### Create Library Project

```bash
# 1. Create project
cd src
dotnet new classlib -n Blueprintr.NewLibrary -f net10.0

# 2. Configure .csproj (copy from existing library)
# Update: PackageId, Description, Authors
```

#### Create Test Project

```bash
# 1. Create test project
cd tests
dotnet new nunit -n Blueprintr.NewLibrary.Tests -f net10.0

# 2. Configure test project
```

**Test Project `.csproj` Template:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="nunit" />
    <PackageReference Include="NUnit.Analyzers" />
    <PackageReference Include="NUnit3TestAdapter" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Blueprintr.NewLibrary\Blueprintr.NewLibrary.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="NUnit.Framework" />
  </ItemGroup>
</Project>
```

#### Add to Solution

```bash
# Add both projects to solution
cd ..
dotnet sln add src/Blueprintr.NewLibrary/Blueprintr.NewLibrary.csproj
dotnet sln add tests/Blueprintr.NewLibrary.Tests/Blueprintr.NewLibrary.Tests.csproj

# Verify
dotnet build
dotnet test

# Commit
git add .
git commit -m "feat: add Blueprintr.NewLibrary"
```

### Commit Message Format

Follow **Conventional Commits** for clear history and automation:

#### Format

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

#### Types

| Type | Description | Example |
|------|-------------|---------|
| `feat` | New feature | `feat: add validation middleware` |
| `fix` | Bug fix | `fix: resolve null reference error` |
| `docs` | Documentation | `docs: update API examples` |
| `test` | Tests | `test: add edge case tests` |
| `refactor` | Code refactoring | `refactor: simplify endpoint logic` |
| `chore` | Maintenance | `chore: update dependencies` |
| `ci` | CI/CD changes | `ci: add code coverage step` |
| `perf` | Performance | `perf: optimize string handling` |

---

## Troubleshooting

### Tests Pass Locally but Fail in CI

**Common Causes:**

| Cause | Solution |
|-------|----------|
| Environment differences | Use `Path.Combine()` instead of string concatenation |
| Case sensitivity | Linux is case-sensitive; match exact filenames |
| Missing test data | Ensure test files are committed |
| Timezone differences | Use `DateTimeOffset` for time-aware tests |
| Async issues | Don't use `.Result` or `.Wait()` |

**Debug Steps:**

```bash
# Match CI environment
dotnet build --configuration Release
dotnet test --configuration Release

# Check .NET version
dotnet --version
```

### Version Doesn't Change

**Diagnostic:**

```bash
# Check MinVer calculation
minver -v d

# Check local tags
git tag -l

# Check remote tags
git ls-remote --tags origin

# Ensure full history fetched
git fetch --unshallow
```

**Common Issues:**

| Issue | Solution |
|-------|----------|
| Tag not pushed | `git push origin 1.0.0` |
| Shallow clone | Use `fetch-depth: 0` in CI |
| Tag on wrong branch | Verify tag is reachable from current branch |

### Build Warnings Treated as Errors

**Solution 1: Fix the Warning** (Recommended)

Address the underlying code issue.

**Solution 2: Suppress Specific Warning**

In `Directory.Build.props`:
```xml
<WarningsNotAsErrors>NU1510;NU1608;CS1591;CS1234</WarningsNotAsErrors>
```

**Solution 3: Suppress in Code**

```csharp
#pragma warning disable CS1591 // Missing XML comment
public void MyMethod() { }
#pragma warning restore CS1591
```

### CI Workflow Not Running

**Checklist:**

1. Workflow file exists in `.github/workflows/`
2. Workflow is enabled in GitHub Actions
3. Branch name matches trigger condition
4. No YAML syntax errors

**Validate YAML:**

```bash
# Install yamllint
pip install yamllint

# Check workflow
yamllint .github/workflows/ci.yml
```

### Documentation Not Building

**Checklist:**

1. XML documentation enabled in `.csproj`:
   ```xml
   <GenerateDocumentationFile>true</GenerateDocumentationFile>
   ```

2. DocFX installed:
   ```bash
   dotnet tool install -g docfx
   ```

3. Build documentation:
   ```bash
   docfx metadata
   docfx build
   ```

4. Check for missing XML comments on public APIs

---

## Quick Reference

### Common Commands

| Action | Command |
|--------|---------|
| Build | `dotnet build` |
| Build Release | `dotnet build --configuration Release` |
| Test | `dotnet test` |
| Test with Coverage | `dotnet test --collect:"XPlat Code Coverage"` |
| Check Version | `minver` |
| Create Tag | `git tag 1.0.0 && git push origin 1.0.0` |
| Build Docs | `docfx metadata && docfx build` |
| Serve Docs | `docfx serve _site` |
| Clean | `dotnet clean` |
| Restore | `dotnet restore` |

### File Locations

| File | Purpose |
|------|---------|
| `src/` | Library source code |
| `tests/` | Test projects |
| `docs/` | Documentation source |
| `_site/` | Generated documentation (gitignored) |
| `Directory.Build.props` | Shared build settings |
| `Directory.Packages.props` | Central package versions |
| `docfx.json` | DocFX configuration |

### Package Management

#### Directory.Packages.props Structure

Blueprintr uses **Central Package Management (CPM)** to maintain consistent package versions across all projects. The `Directory.Packages.props` file is organized into logical categories:

| Category | Description | Examples |
|----------|-------------|----------|
| **ASP.NET Core & Base Framework** | Core web framework and authentication | JWT Bearer, OpenAPI, Caching |
| **Entity Framework Core** | ORM and database providers | EF Core, PostgreSQL, migrations |
| **Validation & Serialization** | Data validation and serialization | FluentValidation, NodaTime, ULID |
| **Caching & Performance** | Caching strategies and optimizations | Redis, FusionCache |
| **Health Checks** | Application health monitoring | PostgreSQL, Redis health checks |
| **Messaging & Event Bus** | Message queue and event-driven patterns | MassTransit, RabbitMQ |
| **Observability: Logging** | Structured logging infrastructure | Serilog, Console, Seq sinks |
| **Observability: OpenTelemetry** | Distributed tracing and metrics | OTLP exporter, instrumentation |
| **API Documentation** | API documentation and interactive UI | Swagger, Scalar |
| **Email & Templates** | Email sending and template rendering | MailKit, MJML, Razor |
| **Testing** | Testing frameworks and tools | NUnit, NSubstitute, Selenium |
| **.NET Aspire** | Cloud-native orchestration | Aspire hosting, resilience |
| **Utilities** | Helper libraries | Humanizer, Markdig, file handling |
| **Code Quality & Analysis** | Static analysis and linting | SonarAnalyzer |
| **Build & Versioning** | Build automation and versioning | MinVer, SourceLink |

#### Adding New Packages

```bash
# Add package reference to Directory.Packages.props
<PackageVersion Include="NewPackage" Version="1.0.0" />

# Reference in project (version omitted)
dotnet add package NewPackage

# In .csproj (no version attribute)
<PackageReference Include="NewPackage" />
```

#### Best Practices

- **Versions**: All versions centralized in `Directory.Packages.props`
- **Organization**: Keep packages sorted within their category
- **Updates**: Update versions centrally for consistency across projects
- **Documentation**: Comment unusual version choices or pinned versions

---

## Resources

### Official Documentation

- [NUnit Documentation](https://docs.nunit.org/)
- [MinVer GitHub](https://github.com/adamralph/minver)
- [DocFX Official Site](https://dotnet.github.io/docfx/)
- [Semantic Versioning](https://semver.org/)
- [Conventional Commits](https://www.conventionalcommits.org/)

### Microsoft Resources

- [.NET Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [XML Documentation Comments](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)
- [GitHub Actions for .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)

---

## Summary

| Area | Key Points |
|------|------------|
| **Testing** | NUnit framework, 80%+ coverage, AAA pattern, CI enforcement |
| **Versioning** | MinVer auto-calculates, tags create stable releases |
| **Documentation** | DocFX generates from XML comments, deploy to GitHub Pages |
| **Workflow** | Feature branches, conventional commits, PR-based merges |

**Quality is Automated:**
- All tests must pass before merge
- Warnings are treated as errors
- CI runs on every PR
- No deployment without passing tests
