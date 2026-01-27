using System.Reflection;
using System.Runtime.Versioning;

namespace Blueprintr.Utils;

/// <summary>
/// Provides extension methods for retrieving assembly metadata and displaying assembly information.
/// </summary>
/// <remarks>
/// This class extracts information from assembly attributes set in the project file.
/// For more information, see: https://learn.microsoft.com/en-us/dotnet/standard/assembly/set-attributes-project-file
/// Added in version 1.0.0.
/// </remarks>
public static class AssemblyInformation
{
    private const string Unknown = "<unknown>";

    /// <summary>
    /// Gets the company name from the assembly's <see cref="AssemblyCompanyAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The company name, or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetCompanyName(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? Unknown;

    /// <summary>
    /// Gets the build configuration from the assembly's <see cref="AssemblyConfigurationAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The configuration (e.g., "Debug" or "Release"), or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetConfiguration(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? Unknown;

    /// <summary>
    /// Gets the copyright information from the assembly's <see cref="AssemblyCopyrightAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The copyright text, or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetCopyright(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? Unknown;

    /// <summary>
    /// Gets the description from the assembly's <see cref="AssemblyDescriptionAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The description, or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetDescription(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? Unknown;

    /// <summary>
    /// Gets the product name from the assembly's <see cref="AssemblyProductAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The product name, or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetProduct(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? Unknown;

    /// <summary>
    /// Gets the title from the assembly's <see cref="AssemblyTitleAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The title, or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetTitle(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? Unknown;

    /// <summary>
    /// Gets the target framework display name from the assembly's <see cref="TargetFrameworkAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The target framework display name (e.g., ".NET 10.0"), or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GetTargetFramework(this Assembly assembly) => assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName ?? Unknown;

    /// <summary>
    /// Gets the application version from the assembly's <see cref="AssemblyInformationalVersionAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The version string (before the '+' separator if present), or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>
    /// This extracts the version portion before the '+' character, which typically separates
    /// the semantic version from the git commit hash in the InformationalVersion attribute.
    /// Added in version 1.0.0.
    /// </remarks>
    public static string GetAppVersion(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').ElementAtOrDefault(0) ?? Unknown;

    /// <summary>
    /// Gets the git commit hash from the assembly's <see cref="AssemblyInformationalVersionAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to query.</param>
    /// <returns>The git commit hash (after the '+' separator if present), or "&lt;unknown&gt;" if not set.</returns>
    /// <remarks>
    /// This extracts the portion after the '+' character in the InformationalVersion attribute,
    /// which typically contains the git commit hash when using source link or MinVer.
    /// Added in version 1.0.0.
    /// </remarks>
    public static string GetGitCommit(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').ElementAtOrDefault(1) ?? Unknown;

    /// <summary>
    /// Displays comprehensive assembly information to the console with colored output.
    /// </summary>
    /// <param name="assembly">The assembly to display information for.</param>
    /// <returns>The same assembly instance for method chaining.</returns>
    /// <remarks>
    /// This method sets the console title and outputs formatted information including:
    /// - Application title, version, and git commit
    /// - Build configuration and target framework
    /// - Description and copyright (only if not empty)
    /// - Current content root directory
    /// 
    /// The output uses <see cref="ColorConsole"/> for enhanced readability with different colors
    /// for different types of information.
    /// Added in version 1.0.0.
    /// </remarks>
    public static Assembly DisplayAssemblyInformation(this Assembly assembly)
    {
        Console.Title = $"{assembly.GetTitle()} v.{assembly.GetAppVersion()} ({assembly.GetGitCommit().AsSpan(0, 6)})";
        ColorConsole.WriteLine($"Starting {assembly.GetTitle()}", ConsoleColor.Yellow);

        Console.WriteLine();

        ColorConsole.WriteKeyValueLine("Version", assembly.GetAppVersion());
        ColorConsole.WriteKeyValueLine("Git Commit", assembly.GetGitCommit());

        Console.WriteLine();

        ColorConsole.WriteKeyValueLine("Configuration", assembly.GetConfiguration(), ConsoleColor.DarkCyan);
        ColorConsole.WriteKeyValueLine("TargetFramework", assembly.GetTargetFramework(), ConsoleColor.DarkCyan);

        // Check which fields have values
        var company = NullIfUnknown(assembly.GetCompanyName());
        var product = NullIfUnknown(assembly.GetProduct());
        var description = NullIfUnknown(assembly.GetDescription());
        var copyright = NullIfUnknown(assembly.GetCopyright());

        var hasCompany = !string.IsNullOrWhiteSpace(company);
        var hasProduct = !string.IsNullOrWhiteSpace(product);
        var hasDescription = !string.IsNullOrWhiteSpace(description);
        var hasCopyright = !string.IsNullOrWhiteSpace(copyright);

        // Only display the block if at least one field has a value
        if (hasCompany || hasProduct || hasDescription || hasCopyright)
        {
            Console.WriteLine();
            if (hasCompany) ColorConsole.WriteKeyValueLine("Company", company, ConsoleColor.DarkMagenta);
            if (hasProduct) ColorConsole.WriteKeyValueLine("Product", product, ConsoleColor.DarkMagenta);
            if (hasDescription) ColorConsole.WriteKeyValueLine("Description", description, ConsoleColor.DarkMagenta);
            if (hasCopyright) ColorConsole.WriteKeyValueLine("Copyright", copyright, ConsoleColor.DarkMagenta);
        }

        Console.WriteLine();
        ColorConsole.WriteKeyValueLine("ContentRoot", NullIfUnknown(Directory.GetCurrentDirectory()), ConsoleColor.DarkBlue);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        
        return assembly;
    }

    /// <summary>
    /// Converts the "&lt;unknown&gt;" placeholder to null for cleaner output.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>Null if the value equals "&lt;unknown&gt;" (case-insensitive); otherwise, the original value.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    private static string? NullIfUnknown(string value)
    {
        if (string.Equals(value, Unknown, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        return value;
    }
}
