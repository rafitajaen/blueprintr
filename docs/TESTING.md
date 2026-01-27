# Testing Guide

This document explains the testing strategy and CI/CD pipeline for Blueprint projects.

## Testing Strategy

Blueprint uses **NUnit** as the testing framework with the following principles:

### Test Structure

- **Unit Tests**: Test individual methods and classes in isolation
- **Integration Tests**: Test interactions between components (future)
- **Test Organization**: Each library has its own test project

### Test Project Naming

```
src/Blueprint.LibraryName/          -> Blueprint.LibraryName.csproj
tests/Blueprint.LibraryName.Tests/  -> Blueprint.LibraryName.Tests.csproj
```

### Test File Naming

```
src/Blueprintr.Endpoints/EndpointExtensions.cs
tests/Blueprintr.Endpoints.Tests/EndpointExtensionsTests.cs
```

## Writing Tests

### Basic Test Structure

```csharp
namespace Blueprintr.Endpoints.Tests;

[TestFixture]
public class EndpointExtensionsTests
{
    [Test]
    public void MethodName_Scenario_ExpectedBehavior()
    {
        // Arrange
        var input = "test";

        // Act
        var result = input.SomeMethod();

        // Assert
        Assert.That(result, Is.EqualTo("expected"));
    }
}
```

### Test Naming Convention

Use the pattern: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `GetEndpointName_WithLeadingSlash_RemovesSlash`
- `GetEndpointName_WithNull_ThrowsArgumentNullException`
- `Calculate_NegativeNumber_ThrowsArgumentException`

### Arrange-Act-Assert Pattern

Always follow the AAA pattern:

```csharp
[Test]
public void Add_TwoNumbers_ReturnsSum()
{
    // Arrange - Set up test data and preconditions
    var calculator = new Calculator();
    var a = 5;
    var b = 3;

    // Act - Execute the method being tested
    var result = calculator.Add(a, b);

    // Assert - Verify the result
    Assert.That(result, Is.EqualTo(8));
}
```

### Parametrized Tests

For testing multiple scenarios:

```csharp
[TestCase("/api/users", "api-users")]
[TestCase("/api/products/categories", "api-products-categories")]
[TestCase("/health", "health")]
public void GetEndpointName_VariousPaths_ReturnsExpectedNames(string path, string expected)
{
    // Act
    var result = path.GetEndpointName();

    // Assert
    Assert.That(result, Is.EqualTo(expected));
}
```

### Testing Exceptions

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

### Async Tests

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

## Running Tests

### Locally

```bash
# Run all tests
dotnet test

# Run in Release mode
dotnet test --configuration Release

# Run with verbose output
dotnet test --verbosity detailed

# Run tests for specific project
dotnet test tests/Blueprintr.Endpoints.Tests/Blueprintr.Endpoints.Tests.csproj

# Run tests matching a filter
dotnet test --filter "FullyQualifiedName~GetEndpointName"
```

### In CI/CD

Tests run automatically in two scenarios:

#### 1. On Pull Requests
- Workflow: `ci.yml`
- Triggers: Every push to a PR targeting `main` or `develop`
- Purpose: Verify changes don't break tests

#### 2. Before Publishing
- Workflow: `publish-nuget.yml`
- Triggers: Push to `main` with changes in `src/`
- Purpose: Prevent publishing broken packages

## CI Workflow

### CI Pipeline Steps

The `.github/workflows/ci.yml` workflow:

```yaml
1. Checkout code
2. Setup .NET
3. Restore dependencies
4. Build (Release)
5. Run tests
6. Publish test results
7. Generate code coverage
8. Upload coverage to Codecov
```

### Required Status Checks

GitHub branch protection requires:
- ✅ `CI - Build and Test / build-and-test` must pass

This prevents:
- Merging PRs with failing tests
- Direct pushes to `main` without tests passing

### Workflow Integration

```
Pull Request Created
    ↓
CI Workflow Runs
    ↓
Tests Execute
    ↓
├─ All Pass ✅  → Merge Allowed
└─ Any Fail ❌  → Merge Blocked
```

## Code Coverage

### Generating Coverage

Coverage is automatically generated in CI:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Viewing Coverage

1. **In GitHub Actions**: Check workflow run artifacts
2. **Codecov**: View at codecov.io (if configured)
3. **Locally**: Generate and open report:

```bash
# Install report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Generate HTML report
reportgenerator -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report

# Open report
xdg-open ./coverage/report/index.html  # Linux
open ./coverage/report/index.html      # macOS
start ./coverage/report/index.html     # Windows
```

### Coverage Goals

Aim for:
- **Overall**: 80%+ coverage
- **Core logic**: 90%+ coverage
- **Public APIs**: 100% coverage

## Quality Gates

### Enforced Quality Standards

The following standards are enforced:

#### 1. All Tests Must Pass ✅
- No exceptions
- Failing tests block merge

#### 2. Warnings as Errors ✅
- Configured in `Directory.Build.props`
- `TreatWarningsAsErrors: true`
- Exceptions: NU1510, NU1608, CS1591 (XML comments)

#### 3. Code Must Build ✅
- Release configuration
- No build errors

#### 4. Branch Protection ✅
- See [.github/BRANCH_PROTECTION.md](.github/BRANCH_PROTECTION.md)

## Test Best Practices

### Do's ✅

1. **Test One Thing**: Each test should verify one behavior
2. **Independent Tests**: Tests should not depend on each other
3. **Clear Names**: Use descriptive test names
4. **Fast Tests**: Keep tests fast (< 100ms per test)
5. **Arrange-Act-Assert**: Follow the AAA pattern
6. **Edge Cases**: Test boundary conditions
7. **Null Checks**: Test null inputs
8. **Exception Handling**: Verify exceptions are thrown correctly

### Don'ts ❌

1. **No Test Dependencies**: Don't rely on test execution order
2. **No Hard-Coded Paths**: Use relative paths or test fixtures
3. **No External Dependencies**: Mock external services
4. **No Database Tests**: Use in-memory databases for integration tests
5. **No Thread.Sleep**: Use proper async/await or test frameworks
6. **No Console.WriteLine**: Use test output helpers

### Example: Good Test

```csharp
[Test]
public void Add_PositiveNumbers_ReturnsCorrectSum()
{
    // Arrange
    var calculator = new Calculator();

    // Act
    var result = calculator.Add(2, 3);

    // Assert
    Assert.That(result, Is.EqualTo(5));
}
```

### Example: Bad Test

```csharp
[Test]
public void Test1()  // ❌ Bad name
{
    var c = new Calculator();  // ❌ No clear sections
    var r = c.Add(2, 3);
    Assert.That(r, Is.EqualTo(5));
    var r2 = c.Multiply(2, 3);  // ❌ Testing multiple things
    Assert.That(r2, Is.EqualTo(6));
}
```

## Adding Tests to New Libraries

When creating a new library:

### 1. Create Test Project

```bash
# Create test project
dotnet new nunit -n Blueprint.NewLibrary.Tests -o tests/Blueprint.NewLibrary.Tests -f net10.0

# Remove version numbers from PackageReferences in .csproj
# Add project reference to the library being tested

# Add to solution
dotnet sln add tests/Blueprint.NewLibrary.Tests/Blueprint.NewLibrary.Tests.csproj
```

### 2. Configure Test Project

Edit `tests/Blueprint.NewLibrary.Tests/Blueprint.NewLibrary.Tests.csproj`:

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
    <ProjectReference Include="..\..\src\Blueprint.NewLibrary\Blueprint.NewLibrary.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="NUnit.Framework" />
  </ItemGroup>
</Project>
```

### 3. Write First Test

Create `tests/Blueprint.NewLibrary.Tests/MyClassTests.cs`:

```csharp
namespace Blueprintr.NewLibrary.Tests;

[TestFixture]
public class MyClassTests
{
    [Test]
    public void MyMethod_ValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var instance = new MyClass();

        // Act
        var result = instance.MyMethod("input");

        // Assert
        Assert.That(result, Is.Not.Null);
    }
}
```

### 4. Run Tests

```bash
dotnet test
```

## Continuous Integration Flow

### Complete CI/CD Flow

```
Code Change
    ↓
Commit (Conventional Commits)
    ↓
Push to Branch
    ↓
Create Pull Request
    ↓
CI Workflow Triggers
    ├─ Restore
    ├─ Build (warnings as errors)
    ├─ Test
    └─ Coverage
    ↓
Tests Pass? ─┬─ Yes ✅ → Review & Merge Allowed
             └─ No ❌  → Merge Blocked, Fix Required
    ↓
Merge to Main
    ↓
Publish Workflow Triggers
    ├─ Run CI First (must pass)
    ├─ Build Packages
    ├─ Version with MinVer
    └─ Publish to NuGet
```

## Troubleshooting

### Tests Pass Locally but Fail in CI

**Common causes:**
1. Environment differences (paths, OS-specific)
2. Missing test data files
3. Timing issues in async tests
4. Randomness in tests

**Solutions:**
- Check CI logs for specific error
- Ensure test data is committed
- Use proper async/await patterns
- Avoid randomness or seed random generators

### CI Workflow Not Running

**Check:**
1. Workflow file is in `.github/workflows/`
2. Workflow is enabled in GitHub Actions
3. Branch name matches trigger condition
4. No syntax errors in YAML

### Tests Timeout

**Solutions:**
```yaml
# In ci.yml, add timeout
jobs:
  build-and-test:
    timeout-minutes: 30
```

Or in individual tests:
```csharp
[Test, Timeout(5000)]  // 5 seconds
public void MyTest() { }
```

### Coverage Too Low

**Strategies:**
1. Identify uncovered code in coverage report
2. Add tests for edge cases
3. Test error paths
4. Test boundary conditions

## Resources

- [NUnit Documentation](https://docs.nunit.org/)
- [NUnit Best Practices](https://github.com/nunit/docs/wiki/Best-Practices)
- [Testing Best Practices for .NET](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [GitHub Actions for .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)

## Summary

✅ **All tests must pass** before merge
✅ **CI runs automatically** on PRs
✅ **Warnings treated as errors** (enforced)
✅ **Branch protection** prevents broken code in main
✅ **No deployment** without passing tests
✅ **Code coverage** tracked and reported

This ensures **high code quality** and **reliable releases**.
