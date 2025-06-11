using System.ComponentModel.DataAnnotations.Schema;

namespace Site.lib.Models;

public class LocAttributes
{
    [Column(Order = 1)]
    public long AttributeId { get; set; }
    [Column(Order = 2)] 
    public string CultureId { get; set; }
    public byte Status { get; set; }
    public string Name { get; set; }
    public string Misc001 { get; set; }
    public string Misc002 { get; set; }
    public string Misc003 { get; set; }
    public virtual SofAttribute Attribute { get; set; }
    public virtual Culture Culture { get; set; }
}