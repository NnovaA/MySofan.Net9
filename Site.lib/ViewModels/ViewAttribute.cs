namespace Site.lib.ViewModels
{
    public class ViewAttribute
    {
        public long AttributeId { get; set; }
        public long ParentId { get; set; }
        public string CultureId { get; set; }
        public int Order { get; set; }
        public byte Status { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; } = string.Empty;
        public string Misc01 { get; set; } = string.Empty;
        public string Misc02 { get; set; } = string.Empty;
        public string Misc03 { get; set; } = string.Empty;
        public string Misc04 { get; set; } = string.Empty;
        public string Misc05 { get; set; } = string.Empty;
        public string Misc06 { get; set; } = string.Empty;
        public string Misc07 { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public int ChildsCount { get; set; }
        public List<ViewAttribute> Childs { get; set; }
        public List<string> Locs { get; set; }
    }
}
