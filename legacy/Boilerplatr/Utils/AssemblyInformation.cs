using System.Reflection;
using System.Runtime.Versioning;

namespace Boilerplatr.Utils;

/// <summary>
/// Retrieves Assembly Information From Project Attributes.
/// </summary>
/// 
/// <remarks>
/// Official documentation: https://learn.microsoft.com/en-us/dotnet/standard/assembly/set-attributes-project-file
/// </remarks>
public static class AssemblyInformation
{
    private const string Unknown = "<unknown>";

    /// <summary>
    /// Returns Assembly's Company Name.
    /// </summary>
    public static string GetCompanyName(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? Unknown;

    /// <summary>
    /// Returns Assembly's Configuration.
    /// </summary>
    public static string GetConfiguration(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? Unknown;

    /// <summary>
    /// Returns Assembly's Copyright.
    /// </summary>
    public static string GetCopyright(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? Unknown;

    /// <summary>
    /// Returns Assembly's Description.
    /// </summary>
    public static string GetDescription(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? Unknown;

    /// <summary>
    /// Returns Assembly's Product.
    /// </summary>
    public static string GetProduct(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? Unknown;

    /// <summary>
    /// Returns Assembly's Title.
    /// </summary>
    public static string GetTitle(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? Unknown;

    /// <summary>
    /// Returns Assembly's Target Version.
    /// </summary>
    public static string GetTargetFramework(this Assembly assembly) => assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName ?? Unknown;

    /// <summary>
    /// Returns Assembly's Version.
    /// </summary>
    public static string GetAppVersion(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').ElementAtOrDefault(0) ?? Unknown;

    /// <summary>
    /// Returns Assembly's GitCommit Hash.
    /// </summary>
    public static string GetGitCommit(this Assembly assembly) => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').ElementAtOrDefault(1) ?? Unknown;

    /// <summary>
    /// Initializes the console and displays basic project information, including the project description, version, and latest commit hash.
    /// </summary>
    /// 
    /// <remarks>
    /// This method sets up the console environment and outputs key details about the project. It prints the project name, current version,
    /// and the latest commit hash for quick reference. This is useful for providing context during application startup or debugging.
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

        Console.WriteLine();

        // ColorConsole.WriteKeyValueLine("Company", NullIfUnknown(assembly.GetCompanyName()), ConsoleColor.DarkMagenta);
        // ColorConsole.WriteKeyValueLine("Product", NullIfUnknown(assembly.GetProduct()), ConsoleColor.DarkMagenta);
        ColorConsole.WriteKeyValueLine("Description", NullIfUnknown(assembly.GetDescription()), ConsoleColor.DarkMagenta);
        ColorConsole.WriteKeyValueLine("Copyright", NullIfUnknown(assembly.GetCopyright()), ConsoleColor.DarkMagenta);

        Console.WriteLine();
        ColorConsole.WriteKeyValueLine("ContentRoot", NullIfUnknown(Directory.GetCurrentDirectory()), ConsoleColor.DarkBlue);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        
        return assembly;
    }

    private static string? NullIfUnknown(string value)
    {
        if (string.Equals(value, Unknown, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        return value;
    }
}
