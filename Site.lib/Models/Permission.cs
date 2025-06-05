using System.ComponentModel.DataAnnotations.Schema;

namespace Site.lib.Models;

public class Permission
{
    [ForeignKey("Role")]public Guid RoleId { get; set; }
    [ForeignKey("Entry")]public Guid EntryId { get; set; }
    public string EntryName { get; set; }
    public long EntryType { get; set; }
    public bool IsAdd { get; set; }
    public bool IsView { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsDelete { get; set; }
    public Entry Entry { get; set; }
    public Entry Role { get; set; }
}