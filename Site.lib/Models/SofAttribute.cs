using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Site.lib.Models;

public class SofAttribute
{
    public long AttributeId { get; set; }
    [ForeignKey("Parent")] public long ParentId { get; set; }
    public byte Status { get; set; }
    public int Order { get; set; }
    public virtual SofAttribute Parent { get; set; }
    public List<SofAttribute> ChildAttributes { get; set; }
    public List<Entry> Entries { get; set; }
    public List<LocAttributes> Locales { get; set; }
    public List<RelatedAttributes> Masters { get; set; }
    public List<RelatedAttributes> Slaves { get; set; }
    public List<RelatedEntryAttributes> MasterEntries { get; set; }

    [NotMapped]
    public string Name { get; set; }
    [NotMapped]
    public string Misc001 { get; set; }
    [NotMapped]
    public string Misc002 { get; set; }
    [NotMapped]
    public string Misc003 { get; set; }
    
    [NotMapped]
    public string GetName
    {
        get
        {
            var cul=CultureInfo.CurrentCulture.Name.Split('-').First();
            return GetLocale(Locales,cul).Name;
        }
    }
    [NotMapped]
    public string GetMis001
    {
        get
        {
            var cul=CultureInfo.CurrentCulture.Name.Split('-').First();
            return GetLocale(Locales,cul).Misc001;
        }
    }

    [NotMapped]
    public string GetMis002
    {
        get
        {
            var cul=CultureInfo.CurrentCulture.Name.Split('-').First();
            return GetLocale(Locales,cul).Misc002;
        }
    }

    [NotMapped]
    public string GetMis003
    {
        get
        {
            var cul=CultureInfo.CurrentCulture.Name.Split('-').First();
            return GetLocale(Locales,cul).Misc003;
        }
    }

    private LocAttributes GetLocale(List<LocAttributes> locs, string cultureId)
    {
        if (locs == null || locs.Count == 0) return new LocAttributes(); // Return empty instead of null
    
        var localized = locs.FirstOrDefault(c => c.CultureId == cultureId);
        return localized ?? locs.FirstOrDefault(c => c.CultureId == "en") ?? new LocAttributes();
    }
}