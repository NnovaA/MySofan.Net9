namespace Site.lib.ViewModels
{
    public class VmVideoGallery
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Cover { get; set; }
        public string Url { get; set; }
        public List<VmVideo> Videos { get; set; }
        public byte Status { get; set; }
        public int Order { get; internal set; }
        public List<string> Locs { get; internal set; }
    }
}
