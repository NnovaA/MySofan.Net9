namespace Ore.Lib.ViewModels
{
    public class VmEntry
    {
        public string Id { get; set; }
        public string MasterId { get; set; }
        public long TypeId { get; set; }
        public long ChildTypeId { get; set; }
        public string CultureId { get; set; }
        public byte Status { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ShortDesc { get; set; }
        public string Prop01 { get; set; }
        public string Prop02 { get; set; }
        public string Prop03 { get; set; }
        public string Reference { get; set; }
        public string Content { get; set; }
        public int Order { get; set; } = 1;
        public string PropUrl { get; set; }
        public DateTime PropDate { get; set; }
        public string ImageUrl { get; set; }
        public PostImages PostImage { get; set; }
        public VmAddress Address { get; set; }
        public List<string> Locs { get; set; }
        public int RelatedCount { get; set; } = 0;
    }
}