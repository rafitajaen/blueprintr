---
name: fastendpoints-best-practices
description: Comprehensive guide to FastEndpoints best practices, patterns, and implementation strategies for building high-performance ASP.NET APIs following the REPR design pattern.
---

# FastEndpoints Best Practices

Complete guide to building maintainable, scalable APIs with FastEndpoints following the REPR (Request-Endpoint-Response) design pattern.

## Core Concepts

### REPR Design Pattern

FastEndpoints promotes the **Request-Endpoint-Response** pattern:
- **Request**: Strongly-typed input DTO
- **Endpoint**: Handler containing configuration and business logic
- **Response**: Strongly-typed output DTO

**Benefits:**
- Minimal boilerplate code
- Clear separation of concerns
- Easy to test and maintain
- Auto-discovery and registration
- Attribute-free endpoint definitions

### Framework Philosophy

- **Secure by default** - Explicit `AllowAnonymous()` required for public endpoints
- **Performance-focused** - Comparable to Minimal APIs, faster than MVC Controllers
- **Strongly-typed** - Full type safety across the request pipeline
- **Testable** - Route-less, strongly-typed integration testing

## Getting Started

### Installation

```bash
dotnet new web -n MyWebApp
cd MyWebApp
dotnet add package FastEndpoints
```

### Program.cs Configuration

```csharp
using FastEndpoints;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();

var app = bld.Build();
app.UseFastEndpoints();
app.Run();
```

## Endpoint Types

FastEndpoints provides four base endpoint classes:

### 1. Endpoint<TRequest, TResponse>
**✅ Use when**: You need strongly-typed request and response

```csharp
public class CreateUserEndpoint : Endpoint<CreateUserRequest, UserResponse>
{
    public override void Configure()
    {
        Post("/api/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var user = new User
        {
            FirstName = req.FirstName,
            LastName = req.LastName
        };

        await Send.OkAsync(new UserResponse
        {
            FullName = $"{req.FirstName} {req.LastName}",
            IsOver18 = req.Age > 18
        }, ct);
    }
}
```

### 2. Endpoint<TRequest>
**✅ Use when**: You need request DTO but flexible response

```csharp
public class ProcessDataEndpoint : Endpoint<ProcessDataRequest>
{
    public override void Configure()
    {
        Post("/api/process");
    }

    public override async Task HandleAsync(ProcessDataRequest req, CancellationToken ct)
    {
        if (req.Data == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(ct);
    }
}
```

### 3. EndpointWithoutRequest<TResponse>
**✅ Use when**: No input needed, structured response

```csharp
public class GetStatusEndpoint : EndpointWithoutRequest<StatusResponse>
{
    public override void Configure()
    {
        Get("/api/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(new StatusResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        }, ct);
    }
}
```

### 4. EndpointWithoutRequest
**✅ Use when**: No DTOs needed

```csharp
public class HealthCheckEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(ct);
    }
}
```

## Endpoint Configuration

### HTTP Methods & Routes

```csharp
public override void Configure()
{
    // Basic HTTP verbs
    Get("/api/users/{id}");
    Post("/api/users");
    Put("/api/users/{id}");
    Patch("/api/users/{id}");
    Delete("/api/users/{id}");

    // Route parameters (access via Route<T>("paramName"))
    Get("/api/markets/{marketId}/positions/{positionId}");
}
```

### Alternative: Attribute Configuration

```csharp
[HttpPost("/api/users")]
[Authorize]
public class CreateUserEndpoint : Endpoint<CreateUserRequest, UserResponse>
{
    // No Configure() override needed
}
```

## Request & Response DTOs

### ✅ GOOD: Separate, focused DTOs

```csharp
// Request DTO
public class CreateMarketRequest
{
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CloseDate { get; set; }
}

// Response DTO
public class MarketResponse
{
    public string Id { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### ❌ BAD: Reusing entities as DTOs

```csharp
// DON'T do this - separates concerns
public class User // Entity used as DTO
{
    public string Id { get; set; }
    public string PasswordHash { get; set; } // Sensitive data!
    public string Email { get; set; }
}
```

## Validation with FluentValidation

### Creating Validators

FastEndpoints includes FluentValidation - no separate package needed.

```csharp
public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class CreateUserValidator : Validator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18)
            .WithMessage("Must be 18 or older")
            .LessThanOrEqualTo(120)
            .WithMessage("Invalid age");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email is required");
    }
}
```

**Note:** Validators are automatically discovered and registered - no manual DI registration needed.

### Validation Response Format

Failed validation returns **400 Bad Request** with structured errors:

```json
{
  "StatusCode": 400,
  "Message": "One or more errors occurred!",
  "Errors": {
    "FullName": ["Name is required", "Name must be at least 2 characters"],
    "Age": ["Must be 18 or older"],
    "Email": ["Valid email is required"]
  }
}
```

### Manual Validation Control

```csharp
public override void Configure()
{
    Post("/api/users");
    DontThrowIfValidationFails(); // Disable automatic throwing
}

public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
{
    if (ValidationFailed)
    {
        // Custom error handling
        foreach (var failure in ValidationFailures)
        {
            Logger.LogWarning($"{failure.PropertyName}: {failure.ErrorMessage}");
        }
        await Send.BadRequestAsync(ct);
        return;
    }

    // Continue processing
}
```

### Business Logic Validation

```csharp
public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
{
    // Check business rules
    if (await _userRepo.EmailExistsAsync(req.Email))
    {
        AddError(r => r.Email, "Email already in use");
    }

    var maxAge = await _settingsRepo.GetMaxAgeAsync();
    if (req.Age > maxAge)
    {
        AddError(r => r.Age, $"Age cannot exceed {maxAge}");
    }

    // Abort if errors exist
    ThrowIfAnyErrors();

    // Or immediately throw with message
    // ThrowError("Creating user failed!");

    // Continue processing...
}
```

### Global Validation Context

Access validation from anywhere (e.g., services, repositories):

```csharp
// Typed context
var ctx = ValidationContext<CreateUserRequest>.Instance;
ctx.AddError(r => r.Email, "Email is invalid");
ctx.ThrowIfAnyErrors();

// Untyped context
var ctx = ValidationContext.Instance;
ctx.AddError("PropertyName", "Error message");
ctx.ThrowIfAnyErrors();
```

### Advanced Validation Options

```csharp
// Enable AbstractValidator support
bld.Services.AddFastEndpoints(o =>
    o.IncludeAbstractValidators = true);

// Specify validator when duplicates exist
public override void Configure()
{
    Post("/api/users");
    Validator<MySpecificValidator>();
}

// Enable DataAnnotations (alternative to FluentValidation)
app.UseFastEndpoints(c =>
    c.Validation.EnableDataAnnotationsSupport = true);
```

## Dependency Injection

### Property Injection (Recommended)

```csharp
public class GetUserEndpoint : Endpoint<GetUserRequest, UserResponse>
{
    // ✅ Automatic property injection
    public IUserRepository UserRepo { get; set; } = null!;
    public ILogger<GetUserEndpoint> Logger { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/users/{id}");
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var user = await UserRepo.GetByIdAsync(req.Id);

        if (user == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Logger.LogInformation("Retrieved user {UserId}", user.Id);

        await Send.OkAsync(new UserResponse
        {
            Id = user.Id,
            Name = user.Name
        }, ct);
    }
}
```

### Constructor Injection

```csharp
public class CreateUserEndpoint : Endpoint<CreateUserRequest, UserResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;

    // ✅ Constructor injection
    public CreateUserEndpoint(IUserRepository userRepo, IEmailService emailService)
    {
        _userRepo = userRepo;
        _emailService = emailService;
    }

    public override void Configure()
    {
        Post("/api/users");
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var user = await _userRepo.CreateAsync(req);
        await _emailService.SendWelcomeEmailAsync(user.Email);

        await Send.OkAsync(new UserResponse { Id = user.Id }, ct);
    }
}
```

### Manual Resolution

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    // Try resolve - returns null if not registered
    var service = TryResolve<IOptionalService>();

    if (service != null)
    {
        await service.DoSomethingAsync();
    }

    // Resolve - throws if not registered
    var requiredService = Resolve<IRequiredService>();
    await requiredService.ProcessAsync();
}
```

### Pre-Resolved Services

Three services are automatically available in all endpoints:

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    // IConfiguration
    var setting = Config["AppSettings:MaxItems"];

    // IWebHostEnvironment
    if (Env.IsDevelopment())
    {
        Logger.LogDebug("Running in development mode");
    }

    // ILogger (automatically typed for current endpoint)
    Logger.LogInformation("Processing request");
}
```

### Scoped Services in Singletons

Validators, mappers, and event handlers are **singletons**. For scoped dependencies:

```csharp
public class MyValidator : Validator<MyRequest>
{
    public MyValidator()
    {
        RuleFor(x => x.UserId)
            .MustAsync(async (userId, ct) =>
            {
                // Create scope for scoped service
                using var scope = CreateScope();
                var userRepo = scope.Resolve<IUserRepository>();
                return await userRepo.ExistsAsync(userId);
            })
            .WithMessage("User does not exist");
    }
}
```

### Keyed Services

```csharp
public class MyEndpoint : Endpoint<MyRequest>
{
    // Property injection with key
    [KeyedService("PrimaryCache")]
    public ICacheService Cache { get; set; } = null!;

    // Constructor injection with key
    public MyEndpoint([FromKeyedServices("Logger")] ILogger logger)
    {
        // ...
    }
}
```

### Source-Generated Registration

```csharp
// Install: dotnet add package FastEndpoints.Generator

[RegisterService<IUserService>(LifeTime.Scoped)]
public class UserService : IUserService
{
    // Implementation
}

// Auto-generates extension method for registration
```

## Security & Authentication

### ⚠️ CRITICAL: Secure by Default

**Endpoints require authentication by default.** Must explicitly allow anonymous access:

```csharp
public override void Configure()
{
    Get("/api/public-data");
    AllowAnonymous(); // ✅ Required for public endpoints
}
```

### JWT Bearer Authentication

```bash
dotnet add package FastEndpoints.Security
```

```csharp
// Program.cs
bld.Services
   .AddAuthenticationJwtBearer(s =>
   {
       s.SigningKey = "your-secret-signing-key-min-32-chars";
       s.TokenSigningStyle = TokenSigningStyle.Symmetric;
   })
   .AddAuthorization()
   .AddFastEndpoints();

app.UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints();
```

### Generating JWT Tokens

```csharp
public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // Validate credentials
        var user = await AuthenticateUserAsync(req.Email, req.Password);

        if (user == null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Generate JWT token
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = "your-secret-signing-key-min-32-chars";
            o.ExpireAt = DateTime.UtcNow.AddDays(1);

            // Add user info
            o.User.Roles.Add("Manager");
            o.User.Claims.Add(("UserId", user.Id));
            o.User.Claims.Add(("Email", user.Email));
        });

        await Send.OkAsync(new LoginResponse
        {
            Token = jwtToken,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        }, ct);
    }
}
```

### Cookie Authentication

```csharp
// Program.cs
bld.Services.AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(60));

// In endpoint
public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
{
    var user = await AuthenticateAsync(req);

    await CookieAuth.SignInAsync(u =>
    {
        u.Roles.Add("Admin");
        u.Claims.Add(("UserId", user.Id));
    });
}
```

### Authorization Patterns

#### Roles-Based

```csharp
public override void Configure()
{
    Post("/api/admin/users");

    // User must have ANY of these roles
    Roles("Admin", "Moderator");
}
```

#### Claims-Based

```csharp
public override void Configure()
{
    Get("/api/user/profile");

    // Require ANY of these claims
    Claims("UserId", "EmployeeId");

    // Require ALL of these claims
    ClaimsAll("Department", "EmployeeLevel");
}
```

#### Permissions-Based

```csharp
public override void Configure()
{
    Delete("/api/articles/{id}");

    // Require ANY permission
    Permissions("Article_Delete", "Article_Admin");

    // Require ALL permissions
    PermissionsAll("Article_Delete", "Audit_Log");
}
```

#### Policy-Based

```csharp
public override void Configure()
{
    Put("/api/sensitive-data");

    Policy(x => x.RequireAssertion(ctx =>
    {
        var userId = ctx.User.FindFirst("UserId")?.Value;
        var department = ctx.User.FindFirst("Department")?.Value;

        return userId != null && department == "IT";
    }));
}
```

#### OAuth2/OIDC Scopes

```csharp
public override void Configure()
{
    Get("/api/resource");

    // Require ANY scope
    Scopes("read:users", "write:users");

    // Require ALL scopes
    ScopesAll("read:users", "admin:users");
}
```

### Multiple Authentication Schemes

```csharp
// Register multiple schemes
bld.Services
   .AddAuthentication()
   .AddJwtBearer()
   .AddCookie();

// Specify scheme per endpoint
public override void Configure()
{
    Get("/api/users");
    AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
}
```

### Access Control Lists (ACL)

```csharp
public override void Configure()
{
    Post("/api/articles");

    // Generate permission constants
    AccessControl("Article_Create", Apply.ToThisEndpoint);
}

// Generates: public const string Article_Create = "7OR";

// Helper methods
var names = Allow.NamesFor(Article_Create, User_Delete);
var codes = Allow.CodesFor(Article_Create, User_Delete);
var all = Allow.AllPermissions();
```

### Token Revocation

```csharp
// Custom revocation checker
public class TokenBlacklistChecker : JwtRevocationMiddleware
{
    protected override async Task<bool> JwtTokenIsValidAsync(
        string token,
        CancellationToken ct)
    {
        var redis = HttpContext.Resolve<IRedisCache>();
        var isBlacklisted = await redis.ExistsAsync($"blacklist:{token}");

        return !isBlacklisted;
    }
}

// Register before auth middleware
app.UseJwtRevocation<TokenBlacklistChecker>()
   .UseAuthentication()
   .UseAuthorization();
```

### CSRF Protection

```csharp
// Program.cs
bld.Services.AddAntiforgery();

app.UseAntiforgeryFE()
   .UseFastEndpoints();

// Endpoint
public override void Configure()
{
    Post("/api/form-submit");
    EnableAntiforgery();
}

// Generate tokens
var antiforgery = HttpContext.Resolve<IAntiforgery>();
var tokens = antiforgery.GetAndStoreTokens(HttpContext);
// Return tokens.RequestToken to client
```

## Pre & Post Processors

### Pre Processors

Execute **before** endpoint handlers. Useful for cross-cutting concerns.

```csharp
public class RequestLogger<TRequest> : IPreProcessor<TRequest>
{
    public Task PreProcessAsync(
        IPreProcessorContext<TRequest> ctx,
        CancellationToken ct)
    {
        var logger = ctx.HttpContext.Resolve<ILogger<TRequest>>();

        logger.LogInformation(
            "Request: {RequestType} Path: {Path}",
            ctx.Request.GetType().Name,
            ctx.HttpContext.Request.Path);

        return Task.CompletedTask;
    }
}

// Attach to endpoint
public override void Configure()
{
    Post("/api/orders");
    PreProcessor<RequestLogger<CreateOrderRequest>>();
}
```

### Short-Circuiting with Pre Processors

```csharp
public class TenantValidator<TRequest> : IPreProcessor<TRequest>
{
    public async Task PreProcessAsync(
        IPreProcessorContext<TRequest> ctx,
        CancellationToken ct)
    {
        var tenantId = ctx.HttpContext.Request.Headers["X-Tenant-Id"];

        if (string.IsNullOrEmpty(tenantId))
        {
            // Short-circuit - handler won't execute
            await ctx.HttpContext.Response.Send.ForbiddenAsync(ct);
            return;
        }

        // Validate tenant
        var tenantService = ctx.HttpContext.Resolve<ITenantService>();
        var isValid = await tenantService.ValidateAsync(tenantId);

        if (!isValid)
        {
            await ctx.HttpContext.Response.Send.ForbiddenAsync(ct);
        }
    }
}
```

**Important:** Check `ctx.HttpContext.ResponseStarted()` in multiple processors to avoid sending duplicate responses.

### Post Processors

Execute **after** endpoint handlers. Useful for logging, auditing, response modification.

```csharp
public class ResponseLogger<TRequest, TResponse>
    : IPostProcessor<TRequest, TResponse>
{
    public Task PostProcessAsync(
        IPostProcessorContext<TRequest, TResponse> ctx,
        CancellationToken ct)
    {
        var logger = ctx.HttpContext.Resolve<ILogger<TResponse>>();

        if (ctx.Response is CreateOrderResponse response)
        {
            logger.LogInformation(
                "Order created: {OrderId} User: {UserId}",
                response.OrderId,
                ctx.HttpContext.User.FindFirst("UserId")?.Value);
        }

        return Task.CompletedTask;
    }
}

// Attach to endpoint
public override void Configure()
{
    Post("/api/orders");
    PostProcessor<ResponseLogger<CreateOrderRequest, CreateOrderResponse>>();
}
```

### Exception Handling in Post Processors

```csharp
public class ExceptionHandler<TRequest, TResponse>
    : IPostProcessor<TRequest, TResponse>
{
    public async Task PostProcessAsync(
        IPostProcessorContext<TRequest, TResponse> ctx,
        CancellationToken ct)
    {
        if (ctx.ExceptionDispatchInfo != null)
        {
            var exception = ctx.ExceptionDispatchInfo.SourceException;
            var logger = ctx.HttpContext.Resolve<ILogger>();

            logger.LogError(exception, "Unhandled exception in endpoint");

            // Mark as handled to prevent re-throwing
            ctx.MarkExceptionAsHandled();

            await ctx.HttpContext.Response.Send.ErrorAsync(
                "An error occurred",
                500,
                ct);
        }
    }
}
```

### Global Processors

Apply processors to **all endpoints**:

```csharp
public class GlobalRequestLogger : IGlobalPreProcessor
{
    public async Task PreProcessAsync(
        IPreProcessorContext ctx,
        CancellationToken ct)
    {
        var logger = ctx.HttpContext.Resolve<ILogger>();

        logger.LogInformation(
            "{Method} {Path} from {IP}",
            ctx.HttpContext.Request.Method,
            ctx.HttpContext.Request.Path,
            ctx.HttpContext.Connection.RemoteIpAddress);
    }
}

// Register globally
app.UseFastEndpoints(c =>
{
    c.Endpoints.Configurator = ep =>
    {
        ep.PreProcessor<GlobalRequestLogger>(Order.Before);
    };
});
```

### Sharing State Between Processors

```csharp
public class MyStateBag
{
    public string TenantId { get; set; } = string.Empty;
    public DateTime RequestStart { get; set; }
}

public class StatePreProcessor : PreProcessor<MyRequest, MyStateBag>
{
    public override Task PreProcessAsync(
        IPreProcessorContext<MyRequest> ctx,
        MyStateBag state,
        CancellationToken ct)
    {
        state.TenantId = ctx.HttpContext.Request.Headers["X-Tenant-Id"]!;
        state.RequestStart = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}

public class MyEndpoint : Endpoint<MyRequest>
{
    public override void Configure()
    {
        Post("/api/data");
        PreProcessor<StatePreProcessor>();
    }

    public override async Task HandleAsync(MyRequest req, CancellationToken ct)
    {
        // Access shared state
        var state = ProcessorState<MyStateBag>();

        Logger.LogInformation("Processing for tenant: {TenantId}", state.TenantId);
    }
}
```

## Model Binding

### Binding Priority Order

FastEndpoints binds from sources in this order (higher priority first):

1. **JSON Body**
2. **Form Fields**
3. **Route Parameters**
4. **Query Parameters**
5. **User Claims** (`[FromClaim]`)
6. **HTTP Headers** (`[FromHeader]`)
7. **Permissions** (`[HasPermission]`)

### Route Parameter Binding

```csharp
public class GetMarketRequest
{
    public string MarketId { get; set; } = string.Empty;
    public string PositionId { get; set; } = string.Empty;
}

public override void Configure()
{
    Get("/api/markets/{MarketId}/positions/{PositionId}");
}

// Request: /api/markets/abc123/positions/xyz789
// MarketId = "abc123", PositionId = "xyz789"

// Without DTO:
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var marketId = Route<string>("MarketId");
    var positionId = Route<string>("PositionId");
}
```

### Query Parameter Binding

```csharp
// Simple scalar values
public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; }
    public int Limit { get; set; } = 10;
}

// Request: ?Query=election&Page=1&Limit=20

// Collections
public class FilterRequest
{
    public List<int> UserIds { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

// Request: ?UserIds=1&UserIds=2&Tags=tech&Tags=ai

// Complex objects with [FromQuery]
public class BookSearchRequest
{
    [FromQuery]
    public Book Book { get; set; } = null!;
}

public class Book
{
    public string Title { get; set; } = string.Empty;
    public List<Author> Authors { get; set; } = new();
}

// Request: ?Title=MyBook&Authors[0].Name=John&Authors[1].Name=Jane
```

### JSON Body Binding

```csharp
// Entire object from JSON body
public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Address Address { get; set; } = null!;
}

// Single property from JSON with [FromBody]
public class UpdateAddressRequest
{
    public string UserId { get; set; } = string.Empty; // From route/query

    [FromBody]
    public Address Address { get; set; } = null!; // From JSON body only
}

// JSON array binding
public class BatchRequest : Endpoint<List<CreateUserRequest>>
{
    // Binds entire JSON array to List<CreateUserRequest>
}
```

### Form Field Binding

```csharp
public override void Configure()
{
    Post("/api/forms");
    AllowFormData(); // Required for form binding
    // or: AllowFormData(urlEncoded: true) for URL-encoded forms
}

// Simple form fields
public class FormRequest
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

// Nested form data with [FromForm]
public class ComplexFormRequest
{
    [FromForm]
    public Book Book { get; set; } = null!;

    public IFormFile CoverImage { get; set; } = null!;
}

// Form fields: Book.Title=value, Book.Authors[0].Name=name, CoverImage=file
```

### Header Binding

```csharp
public class ApiRequest
{
    [FromHeader]
    public string ApiKey { get; set; } = string.Empty;

    [FromHeader(IsRequired = false)]
    public string? ClientVersion { get; set; }

    [FromHeader("X-Tenant-Id")]
    public string TenantId { get; set; } = string.Empty;
}
```

### Claims Binding

```csharp
public class GetProfileRequest
{
    [FromClaim]
    public string UserId { get; set; } = string.Empty;

    [FromClaim(IsRequired = false)]
    public string? Department { get; set; }

    [FromClaim("Email")]
    public string UserEmail { get; set; } = string.Empty;
}

// Supports scalar values, collections, JSON objects, and arrays
```

### Permission Checking

```csharp
public class UpdateArticleRequest
{
    public string ArticleId { get; set; } = string.Empty;

    [HasPermission("Article_Update")]
    public bool CanUpdate { get; set; }

    [HasPermission("Article_Delete")]
    public bool CanDelete { get; set; }
}

// CanUpdate and CanDelete will be true/false based on user permissions
```

### Handling Naming Mismatches

```csharp
// For JSON properties
public class MyRequest
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;
}

// For form/query/route/headers
public class MyRequest
{
    [BindFrom("user_id")]
    public string UserId { get; set; } = string.Empty;
}
```

### Custom Property Types

Types with `TryParse()` method are automatically supported:

```csharp
public class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public static bool TryParse(string? input, out Point? output)
    {
        output = null;

        if (string.IsNullOrEmpty(input))
            return false;

        var parts = input.Split(',');

        if (parts.Length != 2)
            return false;

        if (!double.TryParse(parts[0], out var x) ||
            !double.TryParse(parts[1], out var y))
            return false;

        output = new Point { X = x, Y = y };
        return true;
    }
}

// Now Point can be bound: ?Location=10.5,20.3
public class MapRequest
{
    public Point Location { get; set; } = null!;
}
```

### Custom Value Parsers

```csharp
app.UseFastEndpoints(c =>
{
    c.Binding.ValueParserFor<Guid>(MyParsers.GuidParser);
    c.Binding.ValueParserFor<DateTime>(MyParsers.DateParser);
});

public static class MyParsers
{
    public static ParseResult GuidParser(object? input)
    {
        var success = Guid.TryParse(input?.ToString(), out var result);
        return new ParseResult(success, result);
    }

    public static ParseResult DateParser(object? input)
    {
        var success = DateTime.TryParseExact(
            input?.ToString(),
            "yyyy-MM-dd",
            null,
            DateTimeStyles.None,
            out var result);

        return new ParseResult(success, result);
    }
}
```

### Restricting Binding Sources

```csharp
public class SecureRequest
{
    // Only bind from JSON body, not route/query
    [DontBind(Source.QueryParam | Source.RouteParam)]
    public string ApiKey { get; set; } = string.Empty;

    // Or specify single source
    [FormField]
    public string UserName { get; set; } = string.Empty;
}
```

### Custom Request Binders

```csharp
public class MyBinder : IRequestBinder<MyRequest>
{
    public async ValueTask<MyRequest> BindAsync(
        BinderContext ctx,
        CancellationToken ct)
    {
        return new MyRequest
        {
            Id = ctx.HttpContext.Request.RouteValues["id"]?.ToString()!,
            TenantId = ctx.HttpContext.Request.Headers["X-Tenant-Id"]!,
            UserId = ctx.HttpContext.User.FindFirst("UserId")?.Value!
        };
    }
}

// Register per-endpoint
public override void Configure()
{
    Post("/api/custom");
    RequestBinder(new MyBinder());
}
```

### Global Binding Modifier

```csharp
app.UseFastEndpoints(c =>
{
    c.Binding.Modifier = (req, tReq, ctx, ct) =>
    {
        // Apply modifications to all requests
        if (req is IHasTenant tenant)
        {
            tenant.TenantId = ctx.HttpContext.Request.Headers["X-Tenant-Id"]!;
        }

        if (req is IHasAudit audit)
        {
            audit.RequestedBy = ctx.HttpContext.User.FindFirst("UserId")?.Value!;
            audit.RequestedAt = DateTime.UtcNow;
        }
    };
});
```

### Raw Request Content

```csharp
public class RawRequest : IPlainTextRequest
{
    public string Content { get; set; } = string.Empty;
}

// Endpoint will receive raw body content in Content property
```

## File Handling

### Enable File Uploads

```csharp
public override void Configure()
{
    Post("/api/upload");
    AllowFileUploads(); // ✅ Required for file uploads
}
```

### Basic File Upload

```csharp
public class UploadEndpoint : Endpoint<UploadRequest>
{
    public override void Configure()
    {
        Post("/api/upload");
        AllowFileUploads();
    }

    public override async Task HandleAsync(UploadRequest req, CancellationToken ct)
    {
        // Access uploaded files
        if (Files.Count > 0)
        {
            var file = Files[0];

            var filePath = Path.Combine(
                "uploads",
                $"{Guid.NewGuid()}_{file.FileName}");

            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream, ct);

            await Send.OkAsync(ct);
        }
        else
        {
            await Send.BadRequestAsync(ct);
        }
    }
}
```

### Binding Files to DTOs

```csharp
public class UploadRequest
{
    // Single file
    public IFormFile Avatar { get; set; } = null!;

    // Multiple files
    public List<IFormFile> Documents { get; set; } = new();

    // Collection
    public IFormFileCollection Attachments { get; set; } = null!;
}

// Form field names should match property names
// Collections accept: Documents, Documents[0], Documents[]
```

### Large File Streaming

For memory-efficient large file handling:

```csharp
public override void Configure()
{
    Post("/api/upload/large");

    // Disable auto-buffering
    AllowFileUploads(dontAutoBindFormData: true);

    // Increase limit (50 MB)
    MaxRequestBodySize(50 * 1024 * 1024);
}

public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    await foreach (var section in FormFileSectionsAsync(ct))
    {
        var filePath = Path.Combine("uploads", section.FileName);

        await using var fs = File.Create(filePath);

        // Stream directly to disk with 64KB buffer
        await section.Section.Body.CopyToAsync(fs, 1024 * 64, ct);
    }

    await Send.OkAsync(ct);
}
```

### Sending Files

#### Stream Response

```csharp
public override async Task HandleAsync(DownloadRequest req, CancellationToken ct)
{
    var filePath = Path.Combine("files", req.FileName);

    if (!File.Exists(filePath))
    {
        await Send.NotFoundAsync(ct);
        return;
    }

    var stream = File.OpenRead(filePath);
    var fileInfo = new FileInfo(filePath);

    await Send.StreamAsync(
        stream: stream,
        fileName: req.FileName,
        fileLengthBytes: fileInfo.Length,
        contentType: "application/octet-stream",
        enableRangeProcessing: true, // Support partial downloads
        cancellation: ct);
}
```

#### File Response

```csharp
public override async Task HandleAsync(DownloadRequest req, CancellationToken ct)
{
    var filePath = Path.Combine("files", req.FileName);

    await Send.FileAsync(
        fileInfo: new FileInfo(filePath),
        contentType: "application/pdf",
        cancellation: ct);
}
```

#### Bytes Response

```csharp
public override async Task HandleAsync(GenerateRequest req, CancellationToken ct)
{
    var pdfBytes = await GeneratePdfAsync(req);

    await Send.BytesAsync(
        bytes: pdfBytes,
        fileName: "report.pdf",
        contentType: "application/pdf",
        cancellation: ct);
}
```

## Response Caching

### Header-Based Response Caching

Response caching manipulates downstream cache headers, **not server-side storage**:

```csharp
// Program.cs
bld.Services.AddFastEndpoints()
           .AddResponseCaching();

app.UseResponseCaching()
   .UseFastEndpoints();

// Endpoint
public override void Configure()
{
    Get("/api/markets");

    // Cache for 60 seconds
    ResponseCache(60);

    // Or with options
    ResponseCache(duration: 60,
                  location: ResponseCacheLocation.Any,
                  noStore: false);
}
```

### Output Caching (Server-Side)

For in-memory or Redis caching:

```csharp
// Program.cs
bld.Services.AddOutputCache();

app.UseOutputCache()
   .UseFastEndpoints();

// Endpoint
public override void Configure()
{
    Get("/api/markets/{id}");

    CacheOutput(p => p
        .Expire(TimeSpan.FromSeconds(60))
        .VaryByValue(ctx => new KeyValuePair<string, string>(
            "marketId",
            ctx.Request.RouteValues["id"]?.ToString() ?? string.Empty))
    );
}
```

## Rate Limiting

### ⚠️ Built-in Throttling (Not for Security)

FastEndpoints provides basic endpoint-level throttling:

```csharp
public override void Configure()
{
    Post("/api/orders");

    Throttle(
        hitLimit: 100,              // Max requests
        durationSeconds: 60,        // Time window
        headerName: "X-Client-Id"   // Client identifier header
    );
}
```

**⚠️ Important Limitations:**
- **Not for security/DDoS protection** - easily bypassed
- Per-endpoint only (no global limits)
- Performance overhead
- Malicious clients can set unique headers per request

**✅ Recommended:** Use gateway-level rate limiting (nginx, API Gateway, Cloudflare)

### ASP.NET Core 7+ Rate Limiting

```csharp
// Program.cs
bld.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(60);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });
});

app.UseRateLimiter()
   .UseFastEndpoints();

// Endpoint
public override void Configure()
{
    Post("/api/orders");
    Options(x => x.RequireRateLimiting("fixed"));
}
```

## Error Handling

### Response Methods

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    // Success responses
    await Send.OkAsync(ct);
    await Send.OkAsync(responseDto, ct);
    await Send.CreatedAtAsync<OtherEndpoint>(routeValues, responseDto, ct);

    // Client error responses
    await Send.BadRequestAsync(ct);
    await Send.UnauthorizedAsync(ct);
    await Send.ForbiddenAsync(ct);
    await Send.NotFoundAsync(ct);
    await Send.ConflictAsync(ct);

    // Server error responses
    await Send.ErrorAsync("Error message", 500, ct);

    // Custom status
    await Send.StatusAsync(StatusCodes.Status418ImATeapot, ct);
}
```

### Try-Catch Pattern

```csharp
public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
{
    try
    {
        var order = await _orderService.CreateAsync(req);

        await Send.CreatedAtAsync<GetOrderEndpoint>(
            new { orderId = order.Id },
            new OrderResponse { Id = order.Id },
            ct);
    }
    catch (ValidationException ex)
    {
        foreach (var error in ex.Errors)
        {
            AddError(error.PropertyName, error.ErrorMessage);
        }

        await Send.BadRequestAsync(ct);
    }
    catch (NotFoundException ex)
    {
        await Send.NotFoundAsync(ct);
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Failed to create order");
        await Send.ErrorAsync("Failed to create order", 500, ct);
    }
}
```

### Custom Error Responses

```csharp
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    if (!await _service.IsValidAsync(req))
    {
        await Send.ErrorsAsync(new ErrorResponse
        {
            Message = "Validation failed",
            Code = "VALIDATION_ERROR",
            Timestamp = DateTime.UtcNow
        }, 400, ct);
    }
}
```

### Global Error Handler (Post Processor)

```csharp
public class GlobalExceptionHandler : IGlobalPostProcessor
{
    public async Task PostProcessAsync(
        IPostProcessorContext ctx,
        CancellationToken ct)
    {
        if (ctx.ExceptionDispatchInfo != null)
        {
            var exception = ctx.ExceptionDispatchInfo.SourceException;
            var logger = ctx.HttpContext.Resolve<ILogger>();

            logger.LogError(exception, "Unhandled exception");

            ctx.MarkExceptionAsHandled();

            var (statusCode, message) = exception switch
            {
                ValidationException => (400, "Validation failed"),
                UnauthorizedAccessException => (401, "Unauthorized"),
                NotFoundException => (404, "Resource not found"),
                _ => (500, "Internal server error")
            };

            await ctx.HttpContext.Response.Send.ErrorAsync(message, statusCode, ct);
        }
    }
}

// Register globally
app.UseFastEndpoints(c =>
{
    c.Endpoints.Configurator = ep =>
    {
        ep.PostProcessor<GlobalExceptionHandler>(Order.After);
    };
});
```

## Testing

### Integration Testing Setup

```bash
dotnet add package xUnit
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Shouldly
```

### AppFixture Pattern

```csharp
public class MyApp : AppFixture<Program>
{
    protected override async ValueTask SetupAsync()
    {
        // One-time setup before all tests
        await SeedDatabaseAsync();
    }

    protected override async ValueTask TearDownAsync()
    {
        // One-time cleanup after all tests
        await CleanupDatabaseAsync();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Replace services for testing
        services.RemoveAll<IEmailService>();
        services.AddSingleton<IEmailService, FakeEmailService>();

        services.RemoveAll<IPaymentGateway>();
        services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
    }

    protected override void ConfigureApp(IApplicationBuilder app)
    {
        // Configure middleware for tests
    }
}
```

### Route-Less Testing

```csharp
public class UserEndpointTests(MyApp app) : TestBase<MyApp>
{
    [Fact]
    public async Task Create_User_Returns_Success()
    {
        // ✅ No URL needed - strongly typed
        var (response, result) = await app.Client
            .POSTAsync<CreateUserEndpoint, CreateUserRequest, UserResponse>(
                new CreateUserRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 25
                });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.FullName.ShouldBe("John Doe");
        result.IsOver18.ShouldBeTrue();
    }

    [Fact]
    public async Task Get_User_Returns_NotFound()
    {
        var (response, result) = await app.Client
            .GETAsync<GetUserEndpoint, GetUserRequest, UserResponse>(
                new GetUserRequest { Id = "nonexistent" });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Invalid_Request_Returns_ValidationErrors()
    {
        var (response, result) = await app.Client
            .POSTAsync<CreateUserEndpoint, CreateUserRequest, ErrorResponse>(
                new CreateUserRequest
                {
                    FirstName = "", // Invalid
                    LastName = "Doe",
                    Age = 15 // Under 18
                });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Errors.ShouldContainKey("FirstName");
        result.Errors.ShouldContainKey("Age");
    }
}
```

### Test Organization

#### StateFixture (Shared State Within Class)

```csharp
public class StateFixture : TestBase<MyApp>
{
    public string SharedUserId { get; set; } = string.Empty;
}

public class OrderTests : StateFixture
{
    public OrderTests(MyApp app) : base(app) { }

    [Fact, Priority(1)]
    public async Task Step1_Create_User()
    {
        var (rsp, res) = await App.Client
            .POSTAsync<CreateUserEndpoint, CreateUserRequest, UserResponse>(
                new CreateUserRequest { /* ... */ });

        SharedUserId = res.Id; // Share state
    }

    [Fact, Priority(2)]
    public async Task Step2_Create_Order_For_User()
    {
        var (rsp, res) = await App.Client
            .POSTAsync<CreateOrderEndpoint, CreateOrderRequest, OrderResponse>(
                new CreateOrderRequest
                {
                    UserId = SharedUserId // Use shared state
                });

        rsp.IsSuccessStatusCode.ShouldBeTrue();
    }
}
```

#### Collection Fixtures (Share AppFixture Across Classes)

```csharp
[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<MyApp> { }

[Collection("Integration")]
public class UserTests(MyApp app) : TestBase<MyApp>
{
    // Tests...
}

[Collection("Integration")]
public class OrderTests(MyApp app) : TestBase<MyApp>
{
    // Tests...
}
```

### Unit Testing

Unit tests bypass the entire request pipeline:

```csharp
[Fact]
public async Task Unit_Test_Handler_Logic()
{
    // Arrange - Create fakes
    var fakeUserRepo = A.Fake<IUserRepository>();
    var fakeEmailService = A.Fake<IEmailService>();

    A.CallTo(() => fakeUserRepo.CreateAsync(A<CreateUserRequest>._))
        .Returns(new User { Id = "123", Name = "John Doe" });

    // Create endpoint with dependencies
    var endpoint = Factory.Create<CreateUserEndpoint>(
        fakeUserRepo,
        fakeEmailService);

    // Act - Call handler directly
    await endpoint.HandleAsync(new CreateUserRequest
    {
        FirstName = "John",
        LastName = "Doe",
        Age = 25
    }, default);

    // Assert
    endpoint.Response.ShouldNotBeNull();
    endpoint.Response.FullName.ShouldBe("John Doe");

    A.CallTo(() => fakeEmailService.SendWelcomeEmailAsync(A<string>._))
        .MustHaveHappened();
}
```

### Service Registration in Unit Tests

```csharp
[Fact]
public async Task Unit_Test_With_Service_Registration()
{
    var fakeMailer = A.Fake<IEmailService>();

    var endpoint = Factory.Create<CreateUserEndpoint>(ctx =>
    {
        ctx.AddTestServices(services =>
        {
            services.AddSingleton(fakeMailer);
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        });
    });

    await endpoint.HandleAsync(new CreateUserRequest { /* ... */ }, default);

    A.CallTo(() => fakeMailer.SendWelcomeEmailAsync(A<string>._))
        .MustHaveHappened();
}
```

## Swagger / OpenAPI

### Installation & Setup

```bash
dotnet add package FastEndpoints.Swagger
```

```csharp
var bld = WebApplication.CreateBuilder();

bld.Services
   .AddFastEndpoints()
   .SwaggerDocument(o =>
   {
       o.DocumentSettings = s =>
       {
           s.Title = "My API";
           s.Version = "v1.0";
           s.Description = "API for managing resources";
       };
   });

var app = bld.Build();

app.UseFastEndpoints()
   .UseSwaggerGen(); // Adds /swagger and /swagger/v1/swagger.json

app.Run();
```

Access Swagger UI at: `http://localhost:5000/swagger`

### Document Configuration Options

```csharp
bld.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "My API";
        s.Version = "v2";
        s.Description = "Comprehensive API documentation";
        s.TermsOfService = "https://example.com/terms";

        s.Contact = new()
        {
            Name = "API Support",
            Email = "support@example.com",
            Url = "https://example.com/support"
        };

        s.License = new()
        {
            Name = "MIT",
            Url = "https://opensource.org/licenses/MIT"
        };
    };

    // Enable JWT Bearer auth in Swagger UI
    o.EnableJWTBearerAuth = true;

    // Only include FastEndpoints (exclude Minimal APIs)
    o.ExcludeNonFastEndpoints = true;

    // Use short schema names instead of full namespaces
    o.ShortSchemaNames = true;

    // Remove empty request schemas
    o.RemoveEmptyRequestSchema = true;

    // Configure tag grouping (0 = by route segment)
    o.AutoTagPathSegmentIndex = 0;
});
```

### Endpoint Documentation

```csharp
public override void Configure()
{
    Post("/api/users");

    // Basic description
    Description(b => b
        .Accepts<CreateUserRequest>("application/json")
        .Produces<UserResponse>(200, "application/json")
        .ProducesProblemDetails(400)
        .ProducesProblemDetails(401)
    );

    // Detailed summary
    Summary(s =>
    {
        s.Summary = "Create a new user";
        s.Description = "Creates a new user account with the provided information. " +
                       "Returns the created user with assigned ID.";

        s.ExampleRequest = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Email = "john.doe@example.com"
        };

        s.ResponseExamples[200] = new UserResponse
        {
            Id = "usr_123456",
            FullName = "John Doe",
            IsOver18 = true
        };

        s.Responses[400] = "Invalid input data";
        s.Responses[401] = "Unauthorized - Authentication required";
    });
}
```

### Property-Level Documentation

#### XML Documentation Comments

```csharp
/// <summary>
/// User creation request
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// User's first name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's age in years
    /// </summary>
    /// <example>25</example>
    [DefaultValue(18)]
    public int Age { get; set; }
}
```

Enable XML documentation in .csproj:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>CS1591</NoWarn>
</PropertyGroup>
```

#### Query Parameter Documentation

```csharp
public class SearchRequest
{
    [QueryParam, DefaultValue("active")]
    public string Status { get; set; } = "active";

    [QueryParam, DefaultValue(10)]
    public int Limit { get; set; } = 10;
}
```

#### Hide Properties from Documentation

```csharp
public class MyRequest
{
    public string PublicProperty { get; set; } = string.Empty;

    [JsonIgnore]
    public string HiddenProperty1 { get; set; } = string.Empty;

    [HideFromDocs]
    public string HiddenProperty2 { get; set; } = string.Empty;
}
```

### Multiple Swagger Documents

```csharp
bld.Services
   .SwaggerDocument(o =>
   {
       o.DocumentSettings = s => s.DocumentName = "Public API";
       o.DocumentName = "v1-public";
   })
   .SwaggerDocument(o =>
   {
       o.DocumentSettings = s => s.DocumentName = "Admin API";
       o.DocumentName = "v1-admin";
   });

// Assign endpoints to documents
public override void Configure()
{
    Post("/api/admin/users");
    Options(x => x.WithTags("Admin"));
}
```

### API Client Generation (Kiota)

FastEndpoints integrates Kiota for automatic client generation:

```csharp
// Program.cs - Add client generation endpoints
app.MapApiClientEndpoint("/clients/csharp", c =>
{
    c.SwaggerDocumentName = "v1";
    c.Language = GenerationLanguage.CSharp;
    c.ClientNamespaceName = "MyCompany.ApiClient";
    c.ClientClassName = "MyApiClient";
});

app.MapApiClientEndpoint("/clients/typescript", c =>
{
    c.SwaggerDocumentName = "v1";
    c.Language = GenerationLanguage.TypeScript;
    c.ClientClassName = "MyApiClient";
});
```

Generate clients via CLI:

```bash
dotnet run --generateclients true
```

Or download from endpoints:
- `http://localhost:5000/clients/csharp`
- `http://localhost:5000/clients/typescript`

## Performance Considerations

### Singleton Lifecycle

**Key components are singletons** for performance:
- Validators
- Mappers
- Event handlers
- Pre/Post processors

**✅ DO:**
- Keep these components stateless
- Resolve dependencies per-request
- Use `CreateScope()` for scoped services

**❌ DON'T:**
- Store request-specific state in instance fields
- Hold references to scoped services

### Async All The Way

```csharp
// ✅ GOOD: Async methods properly awaited
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var user = await _userRepo.GetByIdAsync(req.UserId);
    var orders = await _orderRepo.GetByUserAsync(req.UserId);

    await Send.OkAsync(new { user, orders }, ct);
}

// ❌ BAD: Blocking on async code
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var user = _userRepo.GetByIdAsync(req.UserId).Result; // Deadlock risk!
    var orders = _orderRepo.GetByUserAsync(req.UserId).GetAwaiter().GetResult();

    await Send.OkAsync(new { user, orders }, ct);
}
```

### Use Cancellation Tokens

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    // ✅ Pass cancellation token to async operations
    var data = await _service.FetchDataAsync(req.Id, ct);
    await _cache.SetAsync(req.Id, data, ct);

    await Send.OkAsync(data, ct);
}
```

### Efficient Data Access

```csharp
// ❌ BAD: N+1 query problem
public override async Task HandleAsync(GetOrdersRequest req, CancellationToken ct)
{
    var orders = await _orderRepo.GetAllAsync();

    foreach (var order in orders)
    {
        order.User = await _userRepo.GetByIdAsync(order.UserId); // N queries
    }

    await Send.OkAsync(orders, ct);
}

// ✅ GOOD: Batch fetch
public override async Task HandleAsync(GetOrdersRequest req, CancellationToken ct)
{
    var orders = await _orderRepo.GetAllAsync();
    var userIds = orders.Select(o => o.UserId).Distinct().ToList();
    var users = await _userRepo.GetByIdsAsync(userIds); // 1 query

    var userMap = users.ToDictionary(u => u.Id);

    foreach (var order in orders)
    {
        order.User = userMap[order.UserId];
    }

    await Send.OkAsync(orders, ct);
}
```

## Best Practices Summary

### ✅ DO

1. **Use REPR pattern** - Separate Request, Endpoint, and Response
2. **Keep endpoints focused** - Single responsibility per endpoint
3. **Use property injection** for cleaner code
4. **Validate early** - Use FluentValidation for input validation
5. **Use pre/post processors** for cross-cutting concerns
6. **Write integration tests** - Use route-less testing
7. **Use strongly-typed responses** - Avoid `object` or `dynamic`
8. **Implement proper error handling** - Return appropriate status codes
9. **Document with Swagger** - Use summaries and examples
10. **Use async/await properly** - Pass cancellation tokens
11. **Keep validators stateless** - They're singletons
12. **Use dependency injection** - Don't create dependencies manually
13. **Follow security best practices** - Explicit `AllowAnonymous()`
14. **Use caching strategically** - Cache expensive operations
15. **Batch database operations** - Avoid N+1 queries

### ❌ DON'T

1. **Don't reuse entities as DTOs** - Keep them separate
2. **Don't store state in singletons** - Validators, processors, etc.
3. **Don't block on async code** - Use `await`, not `.Result`
4. **Don't ignore cancellation tokens** - Pass them through
5. **Don't use `.Wait()` or `.GetAwaiter().GetResult()`** - Deadlock risk
6. **Don't skip validation** - Always validate user input
7. **Don't expose internal errors** - Return user-friendly messages
8. **Don't use rate limiting for security** - Use gateway-level protection
9. **Don't forget to test** - Write tests for all endpoints
10. **Don't over-complicate** - Keep it simple
11. **Don't mutate request objects** - Create new instances
12. **Don't skip authentication** - Security first
13. **Don't ignore exceptions** - Handle errors properly
14. **Don't use global error handlers as a crutch** - Handle specific errors
15. **Don't skip documentation** - Future you will thank you

## Common Patterns

### Repository + Service Pattern

```csharp
// Repository (Data Access)
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

// Service (Business Logic)
public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserResponse> GetUserAsync(string id, CancellationToken ct);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;

    public UserService(IUserRepository userRepo, IEmailService emailService)
    {
        _userRepo = userRepo;
        _emailService = emailService;
    }

    public async Task<UserResponse> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken ct)
    {
        // Business logic
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.CreateAsync(user, ct);
        await _emailService.SendWelcomeEmailAsync(user.Email, ct);

        return new UserResponse
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }
}

// Endpoint (HTTP Layer)
public class CreateUserEndpoint : Endpoint<CreateUserRequest, UserResponse>
{
    public IUserService UserService { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/users");
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var response = await UserService.CreateUserAsync(req, ct);
        await Send.CreatedAtAsync<GetUserEndpoint>(
            new { userId = response.Id },
            response,
            ct);
    }
}
```

### Result Pattern for Error Handling

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// In service
public async Task<Result<UserResponse>> CreateUserAsync(
    CreateUserRequest request,
    CancellationToken ct)
{
    if (await _userRepo.EmailExistsAsync(request.Email, ct))
    {
        return Result<UserResponse>.Failure("Email already in use");
    }

    var user = await _userRepo.CreateAsync(/* ... */);

    return Result<UserResponse>.Success(new UserResponse { /* ... */ });
}

// In endpoint
public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
{
    var result = await UserService.CreateUserAsync(req, ct);

    if (!result.IsSuccess)
    {
        AddError("Email", result.Error!);
        await Send.BadRequestAsync(ct);
        return;
    }

    await Send.CreatedAtAsync<GetUserEndpoint>(
        new { userId = result.Value!.Id },
        result.Value,
        ct);
}
```

### Pagination Pattern

```csharp
public class PagedRequest
{
    [QueryParam, DefaultValue(1)]
    public int Page { get; set; } = 1;

    [QueryParam, DefaultValue(20)]
    public int PageSize { get; set; } = 20;
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}

public class GetUsersRequest : PagedRequest
{
    [QueryParam]
    public string? Search { get; set; }
}

public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
{
    var skip = (req.Page - 1) * req.PageSize;

    var (users, totalCount) = await _userRepo.GetPagedAsync(
        skip,
        req.PageSize,
        req.Search,
        ct);

    var response = new PagedResponse<UserResponse>
    {
        Items = users.Select(u => new UserResponse { /* ... */ }).ToList(),
        Page = req.Page,
        PageSize = req.PageSize,
        TotalItems = totalCount
    };

    await Send.OkAsync(response, ct);
}
```

---

**Remember:** FastEndpoints emphasizes simplicity, performance, and maintainability. Follow the REPR pattern, validate thoroughly, secure by default, and test extensively.
