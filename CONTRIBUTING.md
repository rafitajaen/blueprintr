# Contributing Guide

Thank you for your interest in contributing to Blueprintr!

## 🚀 Quick Start

1. Fork the repository
2. Clone your fork: `git clone https://github.com/rafitajaen/blueprintr.git`
3. Create a branch: `git checkout -b feat/my-feature`
4. Make your changes
5. Add tests
6. Commit: `git commit -m "feat: my new feature"`
7. Push: `git push origin feat/my-feature`
8. Open a Pull Request

## 📝 Commit Message Convention

This project follows [Conventional Commits](https://www.conventionalcommits.org/).

### Format
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types
- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **test**: Adding or updating tests
- **refactor**: Code refactoring
- **perf**: Performance improvements
- **ci**: CI/CD changes
- **chore**: Maintenance tasks

### Examples
```bash
feat: add validation middleware
fix: resolve null reference in endpoint handler
docs: update API examples
test: add tests for error scenarios
refactor: simplify endpoint registration
```

### Breaking Changes
```bash
feat!: change endpoint registration API

BREAKING CHANGE: EndpointRegistration method signature changed
```

## 🧪 Testing

### Required
- All new features must include tests
- All tests must pass before submitting PR
- Aim for high code coverage

### Run Tests
```bash
# All tests
dotnet test

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Specific project
dotnet test tests/Blueprintr.Endpoints.Tests/
```

## 📋 Pull Request Process

1. **Ensure tests pass**
   ```bash
   dotnet test
   ```

2. **Update documentation** if needed
   - XML comments for public APIs
   - README.md for significant changes
   - docs/ for new features

3. **Follow code style**
   - EditorConfig is configured
   - Format on save in VS Code
   - Follow existing patterns

4. **Create descriptive PR**
   - Use the PR template
   - Explain what and why
   - Link related issues

5. **Wait for CI**
   - All checks must pass
   - Address review feedback
   - Squash commits if requested

## ✅ Code Standards

- Use C# and .NET conventions
- Enable nullable reference types
- Document public APIs with XML comments
- Keep methods small and focused
- Follow SOLID principles
- No warnings (warnings = errors)

## 📖 Resources

- [Conventional Commits](https://www.conventionalcommits.org/)
- [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [NUnit Documentation](https://docs.nunit.org/)
- [CLAUDE.md](CLAUDE.md) - Complete project guide

## 🆘 Getting Help

- **Questions**: Open a GitHub Discussion
- **Bugs**: Open a GitHub Issue
- **Documentation**: See [CLAUDE.md](CLAUDE.md)

## 🎉 Thank You!

Every contribution, no matter how small, is appreciated!

---

**For complete development guide, see [CLAUDE.md](CLAUDE.md)**
