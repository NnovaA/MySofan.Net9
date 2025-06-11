namespace Site.lib.ViewModels
{
    public class VmCulture
    {
        public string CultureId { get; set; }
        public string Name { get; set; }
        public string Flag { get; set; }
        public byte Priority { get; set; }
        public bool IsPublic { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPrimary { get; set; }
        public string UiCulture { get; set; }
    }
}
