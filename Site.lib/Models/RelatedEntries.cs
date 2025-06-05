using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Site.lib.Models;

public class RelatedEntries
{
    [ForeignKey("MasterEntry")]public Guid MasterId { get; set; }
    [ForeignKey("SlaveEntry")]public Guid SlaveId { get; set; }
    public int Order { get; set; }
    public byte Status { get; set; }
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]public DateTime UpDate { get; set; }
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]public DateTime AddedBy { get; set; }
    public Guid AddedById { get; set; }
    public Guid UpdatedById { get; set; }
    
    public Entry MasterEntry { get; set; }
    public Entry SlaveEntry { get; set; }
}