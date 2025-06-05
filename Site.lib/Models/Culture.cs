namespace Site.lib.Models;

public class Culture
{
    public required string CultureId { get; set; }
    public required string Name { get; set; }
    public required string Flag { get; set; }
    public required string UiCulture { get; set; }
    public bool IsPublic { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsPrimary { get; set; }
    public byte Order { get; set; }
    public List<Localize> locales { get; set; }
}