namespace Site.lib.ViewModels
{
    public class VmImage
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
    public class VmSlide
    {
        public string Id { get; set; }
        public byte Status { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string BtnText { get; set; }
        public string BtnLink { get; set; }
        public string Url { get; set; }
    }
    public class VmBgImage
    {
        public string Id { get; set; }
        public string MasterId { get; set; }
        public string Name { get; set; }
        public bool IsOverlay { get; set; }=false;
        public string Color { get; set; }
        public double Opacity { get; set; } = 1;
        public string Url { get; set; }
    }
}
