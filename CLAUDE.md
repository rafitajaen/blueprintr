# Blueprintr - C# Boilerplate Libraries

> Production-ready C# library project with automated testing, NuGet publishing, and quality gates.

## 🎯 Quick Overview

**What is it?** Collection of reusable C# libraries automatically published to NuGet.

**Current Libraries:**
- `Blueprintr` - Endpoint utilities for ASP.NET Core

**Key Features:**
- ✅ Automated testing (12 passing tests)
- ✅ Automatic NuGet publishing
- ✅ Quality gates (warnings = errors)
- ✅ CI/CD with GitHub Actions
- ✅ Documentation website (DocFX)
- ✅ Conventional Commits
- ✅ AGPL-3.0 licensed

## 🚀 Quick Start

### Installation
```bash
dotnet add package Blueprintr
```

### Usage
```csharp
using Blueprintr;

var name = "/api/users".GetEndpointName();
// Returns: "api-users"
```

## 📁 Project Structure

```
blueprintr/
├── src/
│   └── Blueprintr/           # NuGet library
├── tests/
│   └── Blueprintr.Tests/     # NUnit tests (12 tests ✅)
├── docs/                               # DocFX documentation
├── .github/workflows/                  # CI/CD pipelines
│   ├── ci.yml                         # PR validation (tests only)
│   └── deploy.yml                     # Auto-deploy: Tests → NuGet → Docs
├── .vscode/                           # VS Code config
├── Blueprintr.slnx                    # Solution file
└── CLAUDE.md                          # This file
```

## 🛠️ Development Setup (First Time)

### 1. Prerequisites
```bash
# Required
.NET 10.0 SDK
Git

# Optional (but recommended)
VS Code
DocFX: dotnet tool install -g docfx
MinVer CLI: dotnet tool install -g minver-cli
```

### 2. Configure GitHub Secrets
```bash
# Get API key from: https://www.nuget.org/account/apikeys
# Then: GitHub → Settings → Secrets → Actions
# Create: NUGET_API_KEY = your-api-key
```

### 3. Update Repository URLs
```bash
# Replace YOUR_USERNAME with your GitHub username
find . -type f \( -name "*.md" -o -name "*.csproj" -o -name "*.json" \) \
  -not -path "*/legacy/*" -not -path "*/.git/*" \
  -exec sed -i 's/YOUR_USERNAME/your-actual-username/g' {} +
```

### 4. Configure Branch Protection
```
GitHub → Settings → Branches → Add rule
Branch pattern: main
✅ Require pull request before merging
✅ Require status checks: "Build and Test"
✅ Do not allow bypassing
```

## 💻 Daily Development Workflow

### Create Feature
```bash
# 1. Create branch (use conventional commit prefixes)
git checkout -b feat/new-feature

# 2. Make changes + add tests
code src/Blueprintr/NewFeature.cs
code tests/Blueprintr.Tests/NewFeatureTests.cs

# 3. Test locally
dotnet test

# 4. Commit (Conventional Commits)
git commit -m "feat: add new feature"

# 5. Push & create PR
git push origin feat/new-feature
```

### Commit Message Format
```bash
feat:      # New feature
fix:       # Bug fix
docs:      # Documentation
test:      # Tests
refactor:  # Code refactoring
chore:     # Maintenance
ci:        # CI/CD changes

# Examples:
git commit -m "feat: add validation middleware"
git commit -m "fix: resolve null reference error"
git commit -m "docs: update API examples"
```

## 📦 Publishing Versions

### Automatic Pre-release (Alpha Versions)
Every push to `main` branch that modifies files in `src/`:

```bash
git push origin main
```

**What happens automatically:**
- ✅ Runs all tests
- ✅ Publishes to NuGet as `0.x.x-alpha.0.Y` (pre-release)
- ✅ Updates documentation on GitHub Pages
- ❌ **NO GitHub Release created** (alpha versions don't create releases)

**Example:** Pushing changes publishes `Blueprintr 0.1.1-alpha.0.6` to NuGet.

---

### Stable Release (Production Versions)
To publish a **stable version** and create a **GitHub Release**, create and push a tag:

```bash
# Create a semantic version tag
git tag 0.1.0

# Push the tag to GitHub
git push origin 0.1.0
```

**What happens automatically:**
- ✅ Runs all tests
- ✅ Publishes to NuGet as `0.1.0` (stable version)
- ✅ Updates documentation on GitHub Pages
- ✅ **Creates GitHub Release** with:
  - Release notes (auto-generated from commits)
  - NuGet package files (.nupkg and .snupkg)
  - Installation instructions
  - Links to documentation

**Example:** Pushing tag `0.1.0` publishes `Blueprintr 0.1.0` to NuGet AND creates a GitHub Release.

---

### Version Examples

```bash
# Production versions (create GitHub Releases)
git tag 0.1.0          # First stable release
git tag 0.1.1          # Patch (bug fixes)
git tag 0.2.0          # Minor (new features)
git tag 1.0.0          # Major (breaking changes)

# Pre-release versions (create GitHub Releases)
git tag 1.0.0-beta.1   # Beta pre-release
git tag 1.0.0-rc.1     # Release candidate

# Development versions (NO GitHub Releases)
# Just push to main - auto-publishes as alpha
git push origin main   # Creates 0.x.x-alpha.0.Y
```

---

### When to Use Each

| Scenario | Action | NuGet Version | GitHub Release |
|----------|--------|---------------|----------------|
| Development/testing | Push to main | `0.1.1-alpha.0.6` | ❌ No |
| Bug fix | Create tag `0.1.1` | `0.1.1` | ✅ Yes |
| New features | Create tag `0.2.0` | `0.2.0` | ✅ Yes |
| Breaking changes | Create tag `1.0.0` | `1.0.0` | ✅ Yes |
| Beta testing | Create tag `1.0.0-beta.1` | `1.0.0-beta.1` | ✅ Yes |

---

### First Stable Release (Step-by-Step)

When you're ready to publish your first stable version:

1. **Ensure all changes are committed and tests pass:**
   ```bash
   dotnet test
   git status  # Should be clean
   ```

2. **Create a version tag following semantic versioning:**
   ```bash
   git tag 0.1.0
   ```

3. **Push the tag to trigger the release workflow:**
   ```bash
   git push origin 0.1.0
   ```

4. **Monitor the workflow:**
   ```bash
   gh run watch
   ```

5. **Verify the release:**
   - NuGet: https://www.nuget.org/packages/Blueprintr/
   - GitHub Releases: https://github.com/rafitajaen/blueprintr/releases
   - Documentation: https://rafitajaen.github.io/blueprintr/

**Done!** Your package is now published and available for everyone to install.

## 🧪 Testing

### Run Tests
```bash
# All tests
dotnet test

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Specific project
dotnet test tests/Blueprintr.Tests/
```

### Quality Gates
- ✅ All tests must pass (12/12)
- ✅ Warnings treated as errors
- ✅ Build must succeed
- ✅ CI must pass before merge
- ✅ No direct push to main

## 🔄 CI/CD Pipeline

### What Happens on PR?
```
1. Push to feature branch
2. Create Pull Request
3. CI Workflow (ci.yml) runs:
   - Restore dependencies
   - Build (Release)
   - Run tests
   - Generate coverage
4. Tests pass? → Merge allowed ✅
   Tests fail? → Merge blocked ❌
```

### What Happens on Merge to Main?
```
1. Merge to main (or direct push)
2. Deploy Workflow (deploy.yml) runs EVERYTHING automatically:

   Step 1: Build & Test
   - Restore dependencies
   - Build (Release)
   - Run all tests
   - Generate coverage reports

   Step 2: Publish to NuGet (if src/ changed)
   - Detect changed projects
   - Version with MinVer (from git tags)
   - Pack NuGet packages
   - Publish to NuGet.org

   Step 3: Deploy Documentation
   - Generate API docs with DocFX
   - Deploy to GitHub Pages

All in one workflow! 🚀
```

## 📚 Adding New Library

```bash
# 1. Create project
cd src
dotnet new classlib -n Blueprintr.NewLibrary -f net10.0

# 2. Copy config from Blueprintr.csproj
#    Update: PackageId, Description, Authors

# 3. Add XML documentation to public APIs
/// <summary>
/// Description of your class
/// </summary>

# 4. Create tests
cd ../tests
dotnet new nunit -n Blueprintr.NewLibrary.Tests -f net10.0

# 5. Add to solution
cd ..
dotnet sln add src/Blueprintr.NewLibrary/Blueprintr.NewLibrary.csproj
dotnet sln add tests/Blueprintr.NewLibrary.Tests/Blueprintr.NewLibrary.Tests.csproj

# 6. Commit and push
git add .
git commit -m "feat: add Blueprintr.NewLibrary"
git push origin main
```

## 📖 Documentation

### Generate Docs Locally
```bash
# Build documentation
docfx metadata
docfx build

# Serve locally
docfx serve _site
# Open: http://localhost:8080
```

### Documentation Updates
- XML comments in code → API docs (automatic)
- Markdown files in `docs/` → Articles
- Push tag → Docs update on GitHub Pages

### GitHub Pages
After first tag push:
- Docs available at: `https://rafitajaen.github.io/blueprintr/`

## 🛠️ Useful Commands

```bash
# Build
dotnet build
dotnet build --configuration Release

# Test
dotnet test
dotnet test --verbosity detailed

# Package
dotnet pack --configuration Release

# Clean
dotnet clean

# Restore
dotnet restore

# Check version
dotnet minver

# Run all CI steps locally
dotnet restore && \
  dotnet build --configuration Release --no-restore && \
  dotnet test --configuration Release --no-build
```

## 🎯 VS Code Tasks

Press `Ctrl+Shift+B` (or `Cmd+Shift+B` on Mac) to access:
- **build** - Default build
- **test** - Run tests
- **build-release** - Release build
- **test-with-coverage** - Tests with coverage
- **pack** - Create NuGet package
- **clean** - Clean outputs

## 📋 Configuration Files

| File | Purpose |
|------|---------|
| `Directory.Build.props` | Shared settings (warnings as errors) |
| `Directory.Packages.props` | Central package versions |
| `Blueprintr.slnx` | Solution file |
| `docfx.json` | Documentation config |
| `.editorconfig` | Code style rules |
| `.gitignore` | Ignore patterns |

## 🔐 License: AGPL-3.0

This project uses **GNU Affero General Public License v3.0**.

**Key Points:**
- ✅ Commercial use allowed
- ✅ Can modify and distribute
- ⚠️ **Must share source code** when distributing
- ⚠️ **Network use = distribution** (SaaS must share source)
- ⚠️ Modifications must be AGPL-3.0

**Why AGPL?** To ensure improvements are shared back to the community, including for SaaS applications.

## 🐛 Troubleshooting

### Build Fails
```bash
dotnet clean && dotnet restore && dotnet build
```

### Tests Fail
```bash
dotnet test --verbosity detailed
```

### CI Not Running
- Check `.github/workflows/` files exist
- Verify workflow is enabled in GitHub Actions
- Check branch name matches trigger

### Can't Merge PR
- Verify all tests pass
- Check branch protection is configured
- Ensure CI status check passed

### Publishing Fails
- Check `NUGET_API_KEY` is configured
- Verify API key hasn't expired
- Check package version doesn't exist

## 📊 Project Stats

```
Target Framework: .NET 10.0
License: AGPL-3.0-or-later
Tests: 12 passing
Test Framework: NUnit
CI/CD: GitHub Actions
Documentation: DocFX
Versioning: MinVer (Git tags)
```

## 🔗 Important Links

- **NuGet Package**: https://www.nuget.org/packages/Blueprintr/
- **Documentation**: https://rafitajaen.github.io/blueprintr/
- **GitHub Repo**: https://github.com/rafitajaen/blueprintr
- **Issues**: https://github.com/rafitajaen/blueprintr/issues

## 💡 Key Concepts

### Conventional Commits
Standard commit message format for automated changelogs and versioning.
Format: `type(scope): description`

### MinVer
Automatic semantic versioning from Git tags. No manual version management.

### Central Package Management
All package versions in `Directory.Packages.props`. Consistent across projects.

### DocFX
Generates documentation from XML comments and markdown files.

### Quality Gates
Automated checks that prevent broken code from reaching main branch.

## 🎓 Learning Path

**New to the project?**
1. Read this file (CLAUDE.md)
2. Run `dotnet build` and `dotnet test`
3. Make a small change and create a PR
4. See CI in action

**Setting up for the first time?**
1. Configure `NUGET_API_KEY`
2. Update `YOUR_USERNAME`
3. Set up branch protection
4. Make first commit

**Ready to contribute?**
1. Check issues for tasks
2. Follow Conventional Commits
3. Add tests for changes
4. Create PR when ready

## 📞 Getting Help

- **Issues**: Open a GitHub issue
- **Discussions**: Use GitHub Discussions
- **Documentation**: Check docs/ folder
- **Examples**: See tests/ folder

## 🚨 Common Mistakes to Avoid

❌ Don't commit without tests
❌ Don't bypass CI checks
❌ Don't use incorrect commit format
❌ Don't commit sensitive files (.env, keys)
❌ Don't push directly to main

✅ Always add tests
✅ Wait for CI to pass
✅ Use Conventional Commits
✅ Check .gitignore is working
✅ Create PRs for changes

## 🎉 Quick Win Checklist

For a successful first contribution:
- [ ] Fork and clone repo
- [ ] Run `dotnet build` successfully
- [ ] Run `dotnet test` - all pass
- [ ] Create feature branch
- [ ] Make small change
- [ ] Add test for change
- [ ] Commit with conventional format
- [ ] Push and create PR
- [ ] Watch CI pass ✅
- [ ] Merge when approved

---

**Ready to start?** Run `dotnet build && dotnet test` to verify everything works! 🚀

**Questions?** Open a GitHub issue or discussion.

**Want to contribute?** Create a PR with tests and conventional commits.
