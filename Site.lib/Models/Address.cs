using System.ComponentModel.DataAnnotations.Schema;

namespace Site.lib.Models;

public class Address
{
    [ForeignKey("Entry")]
    public Guid EntryId { get; set; }
    public Guid AddressId { get; set; }
    public bool IsPrimary { get; set; }
    public long CountryId { get; set; } = 0;
    public long GovId { get; set; } = 0;
    public long AreaId { get; set; } = 0;
    public string Block { get; set; } = "";
    public string Street { get; set; } = "";
    public string House { get; set; } = "";
    public string Building { get; set; } = "";
    public string Floor { get; set; } = "";
    public string ApartmentNo { get; set; } = "";
    public string OfficeNo { get; set; } = "";
    public string Avenue { get; set; } = "";
    public Entry Entry { get; set; }
}