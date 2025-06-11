namespace Site.lib.ViewModels
{
    public class VmPhotoGallery
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Url { get; set; }
        public string Cover { get; set; }
        public byte Status { get; set; }
        public int Order { get; set; }
        public List<VmImage> Images { get; set; }
    }
}
