namespace Site.lib.ViewModels
{
    public class VmSiteLink
    {
        public string LinkId { get; set; }
        public string MasterId { get; set; }
        public long TypeId { get; set; }
        public string TypeName { get; set; }
        public string Url { get; set; }
        public string TypeIcon { get; set; }
        public string Title { get; set; }
        public string Target { get; set; }
        public byte Status { get; set; }
    }
    public class VmSocialType
    {
        public long TypeId { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
    }
}
