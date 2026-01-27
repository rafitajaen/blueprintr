namespace Boilerplatr.Sitemaps;

public sealed class SitemapOptions
{
    public double Priority { get; set; } = 0.5;
    public ChangeFrequency ChangeFrequency { get; set; } = ChangeFrequency.Weekly;
}
