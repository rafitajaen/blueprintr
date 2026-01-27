namespace Boilerplatr.Sitemaps;

public sealed class Metadata
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<string> Keywords { get; set; } = [];
}
