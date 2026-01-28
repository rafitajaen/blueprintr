# Getting Started

This comprehensive guide will help you get started with Blueprintr libraries, from installation through advanced usage patterns.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Installation](#installation)
3. [Basic Usage](#basic-usage)
4. [Code Examples](#code-examples)
5. [Best Practices](#best-practices)
6. [Troubleshooting](#troubleshooting)
7. [Next Steps](#next-steps)

---

## Prerequisites

Before using Blueprintr libraries, ensure you have the following installed:

### Required

| Requirement | Version | How to Check | Download |
|-------------|---------|--------------|----------|
| .NET SDK | 10.0 or later | `dotnet --version` | [Download .NET](https://dotnet.microsoft.com/download) |
| C# Project | Console, Web, Library, etc. | N/A | N/A |

### Optional (for Development)

| Tool | Purpose | Installation |
|------|---------|--------------|
| Visual Studio | IDE with NuGet GUI | [Download VS](https://visualstudio.microsoft.com/) |
| Visual Studio Code | Lightweight editor | [Download VS Code](https://code.visualstudio.com/) |
| JetBrains Rider | Cross-platform IDE | [Download Rider](https://www.jetbrains.com/rider/) |

### Verify Installation

```bash
# Check .NET SDK version
dotnet --version

# Expected output: 10.0.x or higher
```

---

## Installation

There are multiple ways to install Blueprintr packages. Choose the method that best fits your workflow.

### Method 1: .NET CLI (Recommended)

The simplest and most universal method:

```bash
# Navigate to your project directory
cd /path/to/your/project

# Install Blueprintr
dotnet add package Blueprintr
```

**For specific versions:**
```bash
# Install a specific stable version
dotnet add package Blueprintr --version 1.0.0

# Install the latest pre-release version
dotnet add package Blueprintr --prerelease
```

### Method 2: Package Manager Console (Visual Studio)

If you're using Visual Studio:

```powershell
# Install latest stable version
Install-Package Blueprintr

# Install specific version
Install-Package Blueprintr -Version 1.0.0

# Install pre-release version
Install-Package Blueprintr -Prerelease
```

### Method 3: Visual Studio NuGet GUI

1. Right-click on your project in Solution Explorer
2. Select **"Manage NuGet Packages"**
3. Click the **"Browse"** tab
4. Search for **"Blueprintr"**
5. Select the package and click **"Install"**
6. Review and accept the license

**Note:** Check **"Include prerelease"** to see alpha/beta versions.

### Method 4: Manual .csproj Edit

Add directly to your project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Blueprintr" Version="1.0.0" />
  </ItemGroup>
</Project>
```

Then restore packages:
```bash
dotnet restore
```

### Verify Installation

After installation, verify the package is correctly installed:

```bash
# List installed packages
dotnet list package

# Expected output includes:
# > Blueprintr    1.0.0
```

---

## Basic Usage

### Blueprintr

The Blueprintr library provides utilities for working with endpoint paths in ASP.NET Core applications.

#### Import the Namespace

```csharp
using Blueprintr;
```

#### GetEndpointName Extension Method

Converts an endpoint path to a standardized, URL-friendly name:

```csharp
using Blueprintr;

public class MyService
{
    public string FormatEndpointName(string path)
    {
        // Convert "/api/users/profile" to "api-users-profile"
        return path.GetEndpointName();
    }
}
```

**Transformation Rules:**
- Removes leading slashes (`/api` becomes `api`)
- Replaces internal slashes with hyphens (`/api/users` becomes `api-users`)
- Preserves other characters
- Case is preserved

#### Entity Framework Core with PostgreSQL

The Blueprintr library provides extension methods for simplified Entity Framework Core configuration with PostgreSQL:

```csharp
using Blueprintr.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with optimized PostgreSQL settings
builder.AddDbContext<AppDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

var app = builder.Build();

// Apply migrations and seed database (in development)
app.UseDbContext<AppDbContext>();

app.Run();
```

**Features included:**
- ✅ **Connection pooling** - Improved performance with `AddDbContextPool`
- ✅ **PostgreSQL support** - Npgsql provider with NodaTime
- ✅ **Snake case naming** - Automatic conversion (UserName → user_name)
- ✅ **Single query optimization** - Better LINQ query performance
- ✅ **Auto-migrations** - Applies pending migrations on startup
- ✅ **Development seeding** - Optional automatic data seeding

**Optional: Implement seeding interface**

```csharp
using Blueprintr.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext, ISeedeableDbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Called before seeding check
    public void Initialize()
    {
        // Create views, triggers, etc.
    }

    // Check if database already has seed data
    public bool IsSeeded()
    {
        return Users.Any();
    }

    // Seed initial data (only in Development)
    public void Seed()
    {
        Users.AddRange(
            new User { Name = "Admin", Email = "admin@example.com" },
            new User { Name = "Test User", Email = "test@example.com" }
        );
        // SaveChanges() called automatically after Seed()
    }
}
```

**Connection String Example:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=myapp;Username=postgres;Password=postgres"
  }
}
```

---

## Code Examples

### Example 1: Basic Endpoint Name Conversion

```csharp
using Blueprintr;

// Single conversion
var name = "/api/users".GetEndpointName();
Console.WriteLine(name);  // Output: api-users
```

### Example 2: Multiple Endpoint Processing

```csharp
using Blueprintr;

var endpoints = new[]
{
    "/api/users",
    "/api/products/categories",
    "/api/orders/history/recent",
    "/health",
    "/metrics"
};

foreach (var endpoint in endpoints)
{
    var name = endpoint.GetEndpointName();
    Console.WriteLine($"{endpoint,-35} -> {name}");
}

// Output:
// /api/users                          -> api-users
// /api/products/categories            -> api-products-categories
// /api/orders/history/recent          -> api-orders-history-recent
// /health                             -> health
// /metrics                            -> metrics
```

### Example 3: ASP.NET Core Minimal API Integration

```csharp
using Blueprintr;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Use GetEndpointName for endpoint metadata
app.MapGet("/api/users", () => Results.Ok(new[] { "Alice", "Bob" }))
   .WithName("/api/users".GetEndpointName())
   .WithTags("Users");

app.MapGet("/api/products", () => Results.Ok(new[] { "Widget", "Gadget" }))
   .WithName("/api/products".GetEndpointName())
   .WithTags("Products");

app.Run();
```

### Example 4: Controller-Based API Integration

```csharp
using Blueprintr;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var endpointName = "/api/users".GetEndpointName();
        _logger.LogInformation("Endpoint {EndpointName} called", endpointName);

        return Ok(new[] { "User1", "User2" });
    }
}
```

### Example 5: Dynamic Route Registration

```csharp
using Blueprintr;

public static class EndpointRegistration
{
    public static void RegisterEndpoints(WebApplication app, string[] routes)
    {
        foreach (var route in routes)
        {
            var endpointName = route.GetEndpointName();

            app.MapGet(route, () => Results.Ok(new { route, name = endpointName }))
               .WithName(endpointName);
        }
    }
}

// Usage:
var routes = new[] { "/api/users", "/api/products", "/api/orders" };
EndpointRegistration.RegisterEndpoints(app, routes);
```

### Example 6: OpenAPI/Swagger Operation IDs

```csharp
using Blueprintr;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Use consistent operation IDs
app.MapGet("/api/users/{id}", (int id) => Results.Ok(new { id, name = "User" }))
   .WithName($"{"/api/users".GetEndpointName()}-get-by-id")
   .WithOpenApi();

app.Run();
```

### Example 7: Entity Framework with PostgreSQL

Complete example showing Entity Framework setup with automatic seeding:

```csharp
using Blueprintr.EntityFramework;
using Microsoft.EntityFrameworkCore;

// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL
builder.AddDbContext<BlogDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

var app = builder.Build();

// Apply migrations and seed
app.UseDbContext<BlogDbContext>();

// Define endpoints
app.MapGet("/api/posts", async (BlogDbContext db) =>
{
    var posts = await db.Posts.ToListAsync();
    return Results.Ok(posts);
});

app.MapPost("/api/posts", async (Post post, BlogDbContext db) =>
{
    db.Posts.Add(post);
    await db.SaveChangesAsync();
    return Results.Created($"/api/posts/{post.Id}", post);
});

app.Run();

// Models
public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// DbContext with seeding
public class BlogDbContext : DbContext, ISeedeableDbContext
{
    public DbSet<Post> Posts { get; set; }

    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    public void Initialize()
    {
        // Custom initialization (runs every startup)
        Console.WriteLine("Database initialized");
    }

    public bool IsSeeded()
    {
        return Posts.Any();
    }

    public void Seed()
    {
        // Seed data (only in Development, only once)
        Posts.AddRange(
            new Post
            {
                Title = "Welcome to Blueprintr",
                Content = "This is a sample blog post created during database seeding."
            },
            new Post
            {
                Title = "Getting Started with EF Core",
                Content = "Learn how to use Entity Framework Core with PostgreSQL."
            }
        );
    }
}
```

**appsettings.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blogdb;Username=postgres;Password=postgres;Include Error Detail=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

**Create and apply migrations:**

```bash
# Install EF Core tools (first time only)
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Migration will be applied automatically on app startup
# Or manually: dotnet ef database update
```

### Example 8: Production-Ready Entity Framework Setup

For production environments, consider disabling automatic migrations:

```csharp
using Blueprintr.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.AddDbContext<AppDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

var app = builder.Build();

// Custom migration strategy for production
if (app.Environment.IsProduction())
{
    // Manual migration deployment recommended
    // Apply migrations only if explicitly requested via environment variable
    if (bool.TryParse(
        Environment.GetEnvironmentVariable("RUN_MIGRATIONS"),
        out var runMigrations) && runMigrations)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}
else
{
    // Development: auto-migrate and seed
    app.UseDbContext<AppDbContext>();
}

app.Run();
```

---

## Best Practices

### 1. Consistent Naming

Use `GetEndpointName()` consistently across your application for all endpoint-related naming:

```csharp
// Good: Consistent naming
var endpoints = new Dictionary<string, Delegate>
{
    { "/api/users".GetEndpointName(), GetUsers },
    { "/api/products".GetEndpointName(), GetProducts },
};

// Avoid: Mixing manual and automated naming
```

### 2. Centralize Endpoint Definitions

Define endpoints in a central location for maintainability:

```csharp
public static class Routes
{
    public const string Users = "/api/users";
    public const string Products = "/api/products";
    public const string Orders = "/api/orders";

    public static string GetName(string route) => route.GetEndpointName();
}

// Usage:
app.MapGet(Routes.Users, GetUsers)
   .WithName(Routes.GetName(Routes.Users));
```

### 3. Logging with Endpoint Names

Include endpoint names in logs for easier debugging:

```csharp
public class LoggingMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value ?? "";
        var endpointName = path.GetEndpointName();

        _logger.LogInformation(
            "Request started: {EndpointName} at {Path}",
            endpointName,
            path
        );

        await next(context);
    }
}
```

### 4. Unit Testing

Test your endpoint name transformations:

```csharp
using Blueprintr;
using NUnit.Framework;

[TestFixture]
public class EndpointNamingTests
{
    [TestCase("/api/users", "api-users")]
    [TestCase("/api/v1/products", "api-v1-products")]
    [TestCase("/health", "health")]
    public void GetEndpointName_VariousPaths_ReturnsExpected(string input, string expected)
    {
        var result = input.GetEndpointName();
        Assert.That(result, Is.EqualTo(expected));
    }
}
```

### 5. Entity Framework Best Practices

**Connection String Security:**

Never hardcode connection strings. Use user secrets in development:

```bash
# Initialize user secrets
dotnet user-secrets init

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=myapp;Username=postgres;Password=postgres"
```

**Production Migration Strategy:**

Avoid automatic migrations in production:

```csharp
// Recommended: Separate migration from app startup
// Deploy migrations using:
// dotnet ef database update --connection "production-connection-string"

// Or use a separate migration job in your CI/CD pipeline
```

**Connection Pooling:**

The `AddDbContext<T>` extension uses connection pooling by default. Monitor pool usage:

```csharp
// Monitor in logs
builder.Services.AddDbContextPool<AppDbContext>(
    options => options.UseNpgsql(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information),
    poolSize: 128  // Default pool size
);
```

**Seeding Strategy:**

Only seed in development. For production data:

```csharp
public void Seed()
{
    // Development seed data only
    if (!Users.Any(u => u.Email == "admin@example.com"))
    {
        Users.Add(new User { /* ... */ });
    }
}

// Production: Use migration data seeding
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.InsertData(
        table: "Users",
        columns: new[] { "Id", "Name", "Email" },
        values: new object[] { 1, "Admin", "admin@example.com" }
    );
}
```

**Performance Optimization:**

```csharp
// Use AsNoTracking for read-only queries
var posts = await db.Posts.AsNoTracking().ToListAsync();

// Use projections to fetch only needed data
var postTitles = await db.Posts
    .Select(p => new { p.Id, p.Title })
    .ToListAsync();

// Batch operations for bulk inserts
db.Posts.AddRange(largeBatchOfPosts);
await db.SaveChangesAsync();
```

---

## Troubleshooting

### Package Not Found

**Symptom:** Package search returns no results or installation fails.

**Solutions:**

1. **Verify package name:**
   ```bash
   # Correct
   dotnet add package Blueprintr

   # Incorrect
   dotnet add package Blueprintr  # Old package name
   ```

2. **Clear NuGet cache:**
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   ```

3. **Check NuGet source:**
   ```bash
   dotnet nuget list source
   # Should include: https://api.nuget.org/v3/index.json
   ```

4. **Add NuGet source if missing:**
   ```bash
   dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
   ```

### Build Errors After Installation

**Symptom:** Project fails to build after adding the package.

**Solutions:**

1. **Check .NET version compatibility:**
   ```bash
   dotnet --version
   # Must be 10.0.x or higher
   ```

2. **Restore packages:**
   ```bash
   dotnet restore
   ```

3. **Clean and rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```

4. **Update package to latest:**
   ```bash
   dotnet add package Blueprintr --version "*"
   ```

### Extension Method Not Found

**Symptom:** `GetEndpointName()` method not recognized.

**Solutions:**

1. **Add the using directive:**
   ```csharp
   using Blueprintr;
   ```

2. **Verify installation:**
   ```bash
   dotnet list package | grep Blueprintr
   ```

3. **Rebuild the project:**
   ```bash
   dotnet build
   ```

### Version Conflicts

**Symptom:** Dependency version conflicts during restore.

**Solutions:**

1. **Check installed versions:**
   ```bash
   dotnet list package --include-transitive
   ```

2. **Update all packages:**
   ```bash
   dotnet outdated  # If dotnet-outdated tool is installed
   ```

3. **Force specific version:**
   ```xml
   <PackageReference Include="Blueprintr" Version="[1.0.0]" />
   ```

---

## Next Steps

Now that you have Blueprintr installed and understand the basics, explore these resources:

### Explore More

- **[API Reference](api/index.md)** - Complete API documentation with all methods and parameters
- **[Development Guide](development-guide.md)** - Learn about testing, versioning, and contributing
- **[Configuration Guide](configuration-guide.md)** - Set up CI/CD and NuGet publishing

### Contribute

Interested in contributing? Here's how:

1. **Report Issues**: Found a bug? [Open an issue](https://github.com/rafitajaen/blueprintr/issues)
2. **Request Features**: Have an idea? [Start a discussion](https://github.com/rafitajaen/blueprintr/discussions)
3. **Submit Code**: See [Contributing Guidelines](../CONTRIBUTING.md)

### Stay Updated

- **Watch the Repository**: Get notified of new releases
- **Check NuGet**: New versions appear at https://www.nuget.org/packages/Blueprintr/
- **Read Release Notes**: Each release includes detailed change information

---

## Quick Reference

| Action | Command |
|--------|---------|
| Install package | `dotnet add package Blueprintr` |
| Install specific version | `dotnet add package Blueprintr --version 1.0.0` |
| Install prerelease | `dotnet add package Blueprintr --prerelease` |
| List packages | `dotnet list package` |
| Update package | `dotnet add package Blueprintr` |
| Remove package | `dotnet remove package Blueprintr` |
| Clear cache | `dotnet nuget locals all --clear` |

---

**Questions?** Open an issue on [GitHub](https://github.com/rafitajaen/blueprintr/issues) or start a [discussion](https://github.com/rafitajaen/blueprintr/discussions).
