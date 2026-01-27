# Blueprintr

Production-ready boilerplate utilities library for .NET projects including converters, validators, extensions, and common patterns.

[![NuGet](https://img.shields.io/nuget/v/Blueprintr.svg)](https://www.nuget.org/packages/Blueprintr/)

## Installation

```bash
dotnet add package Blueprintr
```

## Features

- **Type Converters** - Convert between Guid7, ULID, IP addresses, and more
- **Validators** - FluentValidation validators for common scenarios
- **Extensions** - Useful extension methods for strings, collections, and entities
- **Utilities** - Password hashing, character sets, file detection
- **Entity Framework** - Base entity classes and interfaces
- Fully documented with XML comments
- .NET 10.0 target framework

## Quick Start

### Type Converters

```csharp
using Blueprintr.Converters;

// Convert Guid7
var guid7Converter = new Guid7Converter();
var guid = guid7Converter.ConvertFromProvider("some-value");

// Convert IP addresses
var ipConverter = new IpAddressConverter();
var ip = ipConverter.ConvertFromProvider("192.168.1.1");

// Convert ULIDs
var ulidConverter = new UlidConverter();
var ulid = ulidConverter.ConvertToProvider(Guid.NewGuid());
```

### Password Hashing

```csharp
using Blueprintr.Utils;

// Generate salt and hash password
var salt = PasswordHasher.GenerateSaltHexString();
var hash = PasswordHasher.HashHexString("password123", salt);

// Verify password
bool isValid = PasswordHasher.Verify("password123", hash, salt);
```

### Character Sets

```csharp
using Blueprintr.Utils;

// Use predefined character sets
var alphanumeric = CharSets.Alphanumeric;
var digits = CharSets.Numeric;
var lowercase = CharSets.LowerAlphabetic;
```

## API Documentation

For complete API documentation, visit the [Blueprintr Documentation](https://rafitajaen.github.io/blueprintr/).

## Contributing

Contributions are welcome! Please read the [Contributing Guide](https://github.com/rafitajaen/blueprintr/blob/main/CONTRIBUTING.md).

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [LICENSE](https://github.com/rafitajaen/blueprintr/blob/main/LICENSE) file for details.

## Support

- [Documentation](https://rafitajaen.github.io/blueprintr/)
- [GitHub Issues](https://github.com/rafitajaen/blueprintr/issues)
- [GitHub Discussions](https://github.com/rafitajaen/blueprintr/discussions)
