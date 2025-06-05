namespace Site.lib.Models;

public class LocEntry
{
        public long EntryId { get; set; }
    
        // Store CultureId as a foreign key instead of an object
        public string CultureId { get; set; }
    
        public byte Status { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ShortDesc { get; set; }
        public string Content { get; set; }
        public string Prop01 { get; set; }
        public string Prop02 { get; set; }
        public string Prop03 { get; set; }
        public string PropUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Reference { get; set; }
        public long PropData { get; set; }

        // Navigation property
        public virtual Culture Culture { get; set; }
        public virtual Entry Entry { get; set; }
}