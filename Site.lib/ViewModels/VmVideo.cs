namespace Ore.Lib.ViewModels
{
    public class VmVideo
    {
        public string Id { get; set; }
        public string MasterId { get; set; }
        public string Title { get; set; }
        public string ThumbUrl { get; set; }
        public string VideoUrl { get; set; }
        public byte Status { get; set; }
        public int Order { get; set; }
        public DateTime PubDate { get; set; }
        public string ShortDesc { get; set; }
        public List<string> Locs { get; set; }
    }
}
