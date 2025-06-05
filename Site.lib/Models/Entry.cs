namespace Site.lib.Models;

public class Entry
{
    public Guid EntryId { get; set; }
    public long AttributeId { get; set; }
    public byte status { get; set; }
    public int order { get; set; }
    public Guid AddedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public long DateAdded { get; set; } = DateTime.Now.Ticks;
    public long DateUpdated { get; set; } = DateTime.Now.Ticks;
    public SofAttribute SofAttribute { get; set; }
    public List<LocEntry> LocEntry { get; set; }
    public List<RelatedEntries> Masters { get; set; }
    public List<RelatedEntries> Slaves { get; set; }
    public List<RelatedEntryAttributes> SlaveEntries { get; set; }
}