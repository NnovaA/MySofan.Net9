using System.ComponentModel.DataAnnotations.Schema;

namespace Site.lib.Models;

public class RelatedEntryAttributes
{
    [ForeignKey("Entry")]public Guid EntryId { get; set; }
    [ForeignKey("Attribute")]public long AttributeId { get; set; }
    public int Order { get; set; }
    public byte Status { get; set; }
    public string Value { get; set; }
    public SofAttribute SofAttribute { get; set; }
    public Entry Entry { get; set; }
}