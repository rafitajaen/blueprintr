namespace Boilerplatr.Shared;

public enum Socials
{
    Website,
    X,
    Linkedin,
    Instagram,
    Facebook,
    Tiktok,
    Github,
    Gitlab,
    Youtube,
    DevTo,
    Discord,
    Telegram,
    Medium
}

public sealed class Social
{
    public string? Website { get; set; }
    public string? X { get; set; }
    public string? Linkedin { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Tiktok { get; set; }
    public string? Github { get; set; }
    public string? Gitlab { get; set; }
    public string? Youtube { get; set; }
    public string? DevTo { get; set; }
    public string? Discord { get; set; }
    public string? Telegram { get; set; }
    public string? Medium { get; set; }
    public string? Whatsapp { get; set; }

    public bool Has(Socials social)
    {
        var value = social switch
        {
            Socials.Website => Website,
            Socials.X => X,
            Socials.Linkedin => Linkedin,
            Socials.Instagram => Instagram,
            Socials.Facebook => Facebook,
            Socials.Tiktok => Tiktok,
            Socials.Github => Github,
            Socials.Gitlab => Gitlab,
            Socials.Youtube => Youtube,
            Socials.DevTo => DevTo,
            Socials.Discord => Discord,
            Socials.Medium => Medium,
            Socials.Telegram => Telegram,
            _ => null
        };

        return !string.IsNullOrWhiteSpace(value);
    }

    public string GetFilledClass(Socials social)
    {
        return Has(social) ? "filled" : "button-ghost";
    }
}
