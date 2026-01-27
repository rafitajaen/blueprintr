namespace Boilerplatr.Options;

public abstract class ICustomOptions<TOptions>
{
    public static string SectionName => typeof(TOptions).Name.Replace("Options", "");
}