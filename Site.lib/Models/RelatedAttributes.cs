using System.ComponentModel.DataAnnotations.Schema;

namespace Site.lib.Models;

public class RelatedAttributes
{
    [ForeignKey("Master")]public long MasterId { get; set; }
    [ForeignKey("Slave")]public long SlaveId { get; set; }
    public int Order { get; set; }
    public byte Status { get; set; }
    public virtual SofAttribute Master { get; set; }
    public virtual SofAttribute Slave { get; set; }
}