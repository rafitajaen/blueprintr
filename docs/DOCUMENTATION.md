# Documentation Setup

This project uses **DocFX** to automatically generate documentation from XML comments in the code and publish it to GitHub Pages.

## What is DocFX?

DocFX is Microsoft's documentation generation tool for .NET projects that:
- Generates API documentation from XML comments
- Supports multiple projects in one documentation site
- Creates a modern, searchable website
- Publishes to GitHub Pages automatically
- Updates documentation with each release

## Why DocFX?

For .NET libraries, DocFX is the best choice because:
1. **Automatic API docs**: Extracts documentation from XML comments
2. **Multi-project support**: Documents all Blueprintr libraries in one place
3. **GitHub integration**: Publishes to GitHub Pages automatically
4. **Modern UI**: Clean, responsive, searchable interface
5. **Official Microsoft tool**: Well-maintained and widely used

## Setup Instructions

### 1. Install DocFX

```bash
dotnet tool install -g docfx
```

### 2. Initialize DocFX in the Project

```bash
# From project root
docfx init -q
```

This creates:
- `docfx.json` - Configuration file
- `docs/` - Documentation source files
- `api/` - API documentation (auto-generated)

### 3. Configure docfx.json

The configuration file should look like this:

```json
{
  "metadata": [
    {
      "src": [
        {
          "files": [
            "src/**/*.csproj"
          ],
          "exclude": [
            "**/bin/**",
            "**/obj/**"
          ]
        }
      ],
      "dest": "api",
      "properties": {
        "TargetFramework": "net10.0"
      }
    }
  ],
  "build": {
    "content": [
      {
        "files": [
          "api/**.yml",
          "api/index.md"
        ]
      },
      {
        "files": [
          "docs/**.md",
          "*.md"
        ],
        "exclude": [
          "obj/**",
          "_site/**"
        ]
      }
    ],
    "resource": [
      {
        "files": [
          "images/**"
        ]
      }
    ],
    "output": "_site",
    "template": [
      "default",
      "modern"
    ],
    "globalMetadata": {
      "_appTitle": "Blueprintr Documentation",
      "_appFooter": "© Blueprintr Team",
      "_enableSearch": true
    }
  }
}
```

### 4. Enable XML Documentation

Update `Directory.Build.props` to generate XML docs:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);CS1591</NoWarn>
</PropertyGroup>
```

### 5. Add XML Comments to Code

Document all public APIs:

```csharp
namespace Blueprintr.Endpoints;

/// <summary>
/// Extension methods for endpoint configuration
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Converts an endpoint path to a standardized name
    /// </summary>
    /// <param name="endpointPath">The endpoint path to convert</param>
    /// <returns>A standardized endpoint name</returns>
    /// <example>
    /// <code>
    /// var name = "/api/users".GetEndpointName();
    /// // Returns: "api-users"
    /// </code>
    /// </example>
    public static string GetEndpointName(this string endpointPath)
    {
        return endpointPath.TrimStart('/').Replace('/', '-');
    }
}
```

### 6. Build Documentation Locally

```bash
# Generate API metadata
docfx metadata

# Build the documentation site
docfx build

# Serve locally to preview
docfx serve _site
```

Open http://localhost:8080 to view.

### 7. GitHub Actions for Auto-Publishing

The workflow automatically:
1. Builds documentation on each release
2. Publishes to GitHub Pages
3. Updates with latest API changes

## Documentation Structure

```
blueprintr/
├── docs/
│   ├── index.md              # Landing page
│   ├── getting-started.md    # Getting started guide
│   ├── tutorials/            # Tutorial articles
│   └── guides/               # How-to guides
├── api/                      # Auto-generated API docs (from XML comments)
├── docfx.json               # DocFX configuration
└── _site/                   # Generated website (gitignored)
```

## Writing Documentation

### Landing Page (docs/index.md)

```markdown
# Blueprintr Libraries

Welcome to Blueprintr documentation!

## Available Libraries

- [Blueprintr.Endpoints](xref:Blueprintr.Endpoints) - Endpoint utilities

## Quick Links

- [Getting Started](getting-started.md)
- [API Reference](api/index.md)
```

### Article Pages

Create markdown files in `docs/`:

```markdown
# Getting Started

## Installation

```bash
dotnet add package Blueprintr.Endpoints
```

## Usage

See [EndpointExtensions](xref:Blueprintr.Endpoints.EndpointExtensions) for details.
```

### Cross-References

Link to API documentation using `xref`:

```markdown
See [EndpointExtensions](xref:Blueprintr.Endpoints.EndpointExtensions.GetEndpointName*)
```

## Publishing to GitHub Pages

### Enable GitHub Pages

1. Go to repository Settings
2. Pages section
3. Source: Deploy from a branch
4. Branch: `gh-pages`
5. Folder: `/ (root)`
6. Save

### Workflow Configuration

The workflow (`.github/workflows/documentation.yml`) will:
1. Trigger on tags (releases)
2. Build DocFX documentation
3. Deploy to `gh-pages` branch
4. Site available at: `https://rafitajaen.github.io/blueprintr/`

## Alternatives to DocFX

If you prefer different tools:

### 1. Docusaurus (React-based)
**Pros**: Modern, very customizable, React ecosystem
**Cons**: No automatic API generation, more setup
**Use when**: You want full control and custom design

### 2. MkDocs (Python-based)
**Pros**: Simple, fast, great for pure markdown
**Cons**: No C# API generation
**Use when**: You want simple markdown documentation only

### 3. Wyam / Statiq (C#-based)
**Pros**: C# tool, flexible, similar to DocFX
**Cons**: Less maintained than DocFX
**Use when**: You want DocFX alternative in C#

### 4. VitePress (Vue-based)
**Pros**: Very fast, modern, Vue ecosystem
**Cons**: No automatic API generation
**Use when**: You want fast, modern markdown docs

**Recommendation**: Stick with DocFX for .NET projects. It's the most integrated solution for C# libraries.

## Best Practices

### 1. Document All Public APIs

```csharp
/// <summary>
/// Clear, concise description of what this does
/// </summary>
/// <param name="parameter">What this parameter is for</param>
/// <returns>What this returns</returns>
/// <exception cref="ArgumentNullException">When thrown</exception>
public void MyMethod(string parameter)
{
    // ...
}
```

### 2. Include Examples

```csharp
/// <example>
/// <code>
/// var result = MyMethod("example");
/// </code>
/// </example>
```

### 3. Link Related APIs

```csharp
/// <seealso cref="RelatedClass"/>
/// <seealso cref="AnotherMethod"/>
```

### 4. Use Remarks for Details

```csharp
/// <summary>
/// Brief description
/// </summary>
/// <remarks>
/// Detailed explanation of behavior, edge cases, performance considerations.
/// </remarks>
```

### 5. Update Documentation with Code

- Change API? Update XML comments
- Add feature? Add guide in docs/
- Breaking change? Update migration guide

## Viewing Documentation

### Local

```bash
docfx serve _site
```

### Online (after publishing)

`https://rafitajaen.github.io/blueprintr/`

## Maintenance

### Updating Documentation

1. Edit XML comments in code
2. Edit markdown files in docs/
3. Commit changes
4. Create tag for release
5. Documentation updates automatically

### Versioning Documentation

DocFX supports version switching:

```json
{
  "build": {
    "globalMetadata": {
      "_appVersion": "1.0.0"
    }
  }
}
```

For advanced version management, see [DocFX versioning docs](https://dotnet.github.io/docfx/).

## Resources

- [DocFX Official Site](https://dotnet.github.io/docfx/)
- [DocFX GitHub](https://github.com/dotnet/docfx)
- [XML Documentation Comments](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)
- [GitHub Pages Documentation](https://docs.github.com/en/pages)
