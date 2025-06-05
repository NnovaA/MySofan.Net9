#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Site.lib.DTO;

public class AttributeDTO
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }
    [StringLength(50)]
    public string? Misc001 { get; set; }
    [StringLength(50)]
    public string? Misc002 { get; set; }
    [StringLength(50)]
    public string? Misc003 { get; set; }
}