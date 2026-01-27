namespace Boilerplatr.Options;

public class AnalyticsOptions : ICustomOptions<AnalyticsOptions>
{
    public bool Enabled { get; set; }
    public string? Source { get; set; }
    public string? Script { get; set; }
}
