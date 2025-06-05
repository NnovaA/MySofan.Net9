using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Site.lib.Models;

public class Localize
{
    [Key, Column(Order = 0)]
    public required string ResKey { get; set; }
    [Key, Column(Order = 1)]
    public required string CultureId { get; set; }
    [Key, Column(Order = 2)]
    public required string Group { get; set; }
    public required string Value { get; set; }
    public Culture Culture { get; set; }
}