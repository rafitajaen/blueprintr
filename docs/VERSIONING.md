# Versioning Guide

This project uses **MinVer** for automatic semantic versioning based on Git tags.

## How MinVer Works

MinVer calculates the package version by analyzing Git history:

1. **Without tags**: `0.0.0-alpha.0.{commits}`
2. **With tag**: The tag version plus additional commits
3. **On exact tag**: The exact tag version

## Creating Versions

### First Version

```bash
# When ready to publish the first version
git tag 1.0.0
git push origin 1.0.0
```

### Patch Versions (Bug Fixes)

For bug fixes that don't change the API:

```bash
git tag 1.0.1
git push origin 1.0.1
```

### Minor Versions (New Features)

For new features that maintain backward compatibility:

```bash
git tag 1.1.0
git push origin 1.1.0
```

### Major Versions (Breaking Changes)

For changes that break compatibility:

```bash
git tag 2.0.0
git push origin 2.0.0
```

### Pre-Release Versions

For beta, alpha, RC, etc versions:

```bash
# Beta
git tag 1.0.0-beta.1
git push origin 1.0.0-beta.1

# Release Candidate
git tag 1.0.0-rc.1
git push origin 1.0.0-rc.1
```

## Check Current Version

To see what version will be generated without committing:

```bash
dotnet minver
```

For a specific project:

```bash
dotnet minver -t src/Blueprintr/Blueprintr.csproj
```

## Recommended Workflow

### Active Development (Without Tags)

During development, each commit generates a pre-release version:
- `0.0.0-alpha.0.5` (5 commits from start)
- `0.0.0-alpha.0.6` (6 commits)
- etc.

These versions are published to NuGet automatically but are clearly identifiable as unstable.

### Publishing Stable Version

When code is ready for production:

1. **Ensure everything is committed**
   ```bash
   git status
   ```

2. **Create tag with desired version**
   ```bash
   git tag 1.0.0
   ```

3. **Push the tag**
   ```bash
   git push origin 1.0.0
   ```

4. **GitHub Actions will publish automatically**

### Continue Development After a Release

After creating a tag:
- Next commit will generate `1.0.1-alpha.0.1`
- This indicates development towards version 1.0.1

When ready, you can:
- Create tag `1.0.1` for a patch version
- Or jump to `1.1.0` if you added features

## Multiple Libraries

If you have multiple libraries and want independent versioning:

### Option 1: Shared Tags (Recommended to start)

All libraries share the same version:
```bash
git tag 1.0.0
```

### Option 2: Per-Project Tags

You can use prefixes in tags for independent versioning:

```bash
# For Blueprintr
git tag endpoints/1.0.0

# For another library
git tag otherlibrary/1.0.0
```

Then configure MinVer in each .csproj:
```xml
<MinVerTagPrefix>endpoints/</MinVerTagPrefix>
```

## Troubleshooting

### Version Doesn't Change

- Ensure tag exists: `git tag -l`
- Verify tag was pushed: `git ls-remote --tags origin`
- Workflow needs `fetch-depth: 0` to see full history

### Unexpected Version

```bash
# See what MinVer is seeing
dotnet minver -v d
```

### Clean and Rebuild

```bash
git clean -xfd
dotnet restore
dotnet build
```

## Resources

- [MinVer GitHub](https://github.com/adamralph/minver)
- [Semantic Versioning](https://semver.org/)
- [Git Tags Documentation](https://git-scm.com/book/en/v2/Git-Basics-Tagging)
