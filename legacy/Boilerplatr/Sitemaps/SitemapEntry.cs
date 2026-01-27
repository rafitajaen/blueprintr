using NodaTime;

namespace Boilerplatr.Sitemaps;

public sealed class SitemapEntry
{
    public required string Location { get; set; }
    public double Priority { get; set; } = 0.5;
    public ChangeFrequency ChangeFreq { get; set; } = ChangeFrequency.Weekly;
    public required Instant LastMod { get; set; }
}
