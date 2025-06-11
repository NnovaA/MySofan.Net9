using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Site.Lib.Data;
using Site.lib.Models;
using Site.lib.Sofan.Seed;
using Site.lib.ViewModels;
using Sofan.Hub;

namespace Site.lib.Services;

public interface ISofLoc
{
    string UserId { get; }
    string SiteName { get; }
    string CultureId { get; }
    string UrlSite { get; }
    string DefaultCulture { get; }
    void ChangeCulture(string cultureId);
    List<ViewAttribute> ListParent(long parentId);
    List<Culture> ListCultures();
    LocAttributes GetLocAttribute(long attrId);
    LocAttributes GetLocalizeAttribute(long attrId, string langId);
    LocEntry GetLocalizeEntry(string entryId);
    string Locale(string resKey);
    ViewAttribute AttributeById(long id);
    ViewAttribute AttributeByIdByCultureId(long id, string langId);
    bool ToggleAttribute(long id);
    Task<bool> ToggleEntryStatus(string id);
    bool ReOrderAttribute(long id, byte value);
    bool DeleteAttribute(long id);
    Task<bool> DeleteEntry(string id);
    Task<bool> ReorderEntry(string id, string masterId, int order);
    Task DeleteRangeEntry(List<Entry> ids);
    List<Entry> EntryTypedSlaves(string id, long slaveTypeId);
    List<Entry> EntryTypedMasters(string id, long masterTypeId);
    Task<long> PostAttribute(ViewAttribute vm);
    long GetMaxAttributeOrder(long parentId);
    long SetAttrMaxId(long parentId);
    Guid PostEntry(long attrId, byte status = 1, int order = 1);
    void PostRelated(Guid masterId, Guid slaveId, byte status = 1, int order = 0);

    void PostLocale(long entryId,
        string cultureId, string title, long propDate = 0, string subTitle = "", string shortDesc = "",
        string prop01 = "",
        string prop02 = "", string prop03 = "", string propUrl = "", string imageUrl = "", string content = "",
        byte status = 0);

    void AddAction(Guid actOn, long actId);
    string TimeDifference(DateTime dateGiven, DateTime dateRef);
    VmEntry GetEntry(string id, string langId);
    List<VmEntry> ListPageSections(string id, string langId);
    List<ViewProp002> ListChilds(long id);
    List<VmImage> EntryImages(string id);
    List<VmInitialEntry> ListInitials();
    List<VmEntry> EntryPosts(string id, long typeId, string langId);

    (List<VmEntry> Posts, PaginationInfo Pagination) PagedEntryPosts(string entryId, long childTypeId,
        string cultureId, int pageNumber = 1, int pageSize = 6);
    List<VmEntry> CountedEntryPosts(string id, long typeId, string langId, int count);
    public List<VmAddress> EntryAddresses(string id);
    List<VmImage> GalleryImages(string id);
    List<VmImage> EntryFiles(string id);
    List<VmVideo> EntryVideos(string id, string langId);
    VmVideoGallery VideoGallery(string id);
    List<VmVideo> GalleryVideos(string id);
    VmPhotoGallery PhotoGallery(string id);
    List<VmVideoGallery> EntryVideoGalleries(string id);
    Guid PostLink(VmSiteLink vm);
    List<VmSiteLink> EntryLinks(string id);
    List<VmSiteLink> EntryEmails(string id);
    List<VmPhone> EntryPhones(string id);
    Guid PostPhone(VmPhone vm);
    bool IsImage(string fileUrl);
    List<VmEntry> ListPages();
    List<RelatedEntryAttributes> ListEntryAttributes(string id);
    string PageCover(string id);
    bool IsValidJson(string jsonString);
}

public class SofLoc(
    SofanDbContext SofanDbContext,
    IHttpContextAccessor xtx) : ISofLoc
{
    private readonly HttpContext _ctx = xtx.HttpContext;
    public string DefaultCulture => "en";
    public string UrlSite => ConfigHub.Site;
    public string CurrentUrl => _ctx.Request.GetDisplayUrl();

    public string UserId => _ctx.User.Identity is { IsAuthenticated: true }
        ? new Guid(_ctx.User.Claims.First(c => c.Type == "Id" | c.Type == "NameIdentifier").Value).ToString()
        : "";

    public string SiteName => ConfigHub.SiteName;

    public string CultureId
    {
        get
        {
            _ctx.Request.Cookies.TryGetValue(ConfigHub.CultureCookie, out var culId);
            if (!string.IsNullOrEmpty(culId))
            {
                return culId;
            }

            culId = DefaultCulture;
            var url = CurrentUrl;
            var uriBuilder = new UriBuilder(url);
            var segments = uriBuilder.Path.Split(['/'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var t in segments)
            {
                if (ListCultures().All(c => c.CultureId != t)) continue;
                culId = t;
                break;
            }

            ChangeCulture(culId);
            return culId;
        }
    }

    public void ChangeCulture(string cultureId)
    {
        var options = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(365),
            SameSite = SameSiteMode.Lax,
            Secure = true,
            HttpOnly = true,
            Path = "/"
        };
        if (!ConfigHub.IsLocal)
        {
            options.Domain = "." + ConfigHub.PublicSite;
        }

        _ctx.Response.Cookies.Delete(ConfigHub.CultureCookie);
        _ctx.Response.Cookies.Append(ConfigHub.CultureCookie, cultureId, options);
    }

    public string UserCulture
    {
        get
        {
            _ctx.Request.Headers.TryGetValue("Culture-Id", out var culId);
            if (!string.IsNullOrEmpty(culId))
            {
                return culId;
            }

            _ctx.Response.Headers.Remove("Culture-Id");
            _ctx.Response.Headers.Append("Culture-Id", DefaultCulture);
            return DefaultCulture;
        }
    }

    public List<VmCulture> AdminListCultures()
    {
        var query = (from c in SofanDbContext.Culture
            select new VmCulture
            {
                CultureId = c.CultureId,
                Name = c.Name,
                UiCulture = c.UiCulture,
                Priority = c.Order,
                IsPrimary = c.IsPrimary,
                IsPublic = c.IsPublic,
                IsAdmin = c.IsAdmin,
            }).OrderBy(c => c.Priority).ToList();
        return query;
    }

    public VmCulture GetCultureById(string id)
    {
        var query = (from c in SofanDbContext.Culture
            where c.CultureId == id
            orderby c.Order
            select new VmCulture
            {
                CultureId = c.CultureId,
                Name = c.Name,
                UiCulture = c.UiCulture,
                Priority = c.Order,
                IsPrimary = c.IsPrimary,
                IsPublic = c.IsPublic,
                IsAdmin = c.IsAdmin,
            }).FirstOrDefault();
        return query;
    }

    public List<Culture> ListCultures()
    {
        return SofanDbContext.Culture.Where(c => c.IsPublic == true).OrderBy(c => c.Order).ToList();
    }

    LocAttributes ISofLoc.GetLocAttribute(long attrId)
    {
        throw new NotImplementedException();
    }

    LocAttributes ISofLoc.GetLocalizeAttribute(long attrId, string langId)
    {
        throw new NotImplementedException();
    }

    public List<ViewAttribute> ListParent(long parentId)
    {
        var query = (from lo in SofanDbContext.LocAttributes
            where lo.Attribute.ParentId == parentId
            where lo.CultureId == CultureId
            select new ViewAttribute
            {
                AttributeId = lo.AttributeId,
                Name = lo.Name,
                Misc01 = lo.Misc001,
                Misc02 = lo.Misc002,
                Misc03 = lo.Misc003,
                Status = lo.Attribute.Status,
                Order = lo.Attribute.Order
            }).ToList();
        foreach (var cat in query)
        {
            var locs = (from o in SofanDbContext.Culture
                let Lo = SofanDbContext.LocAttributes.FirstOrDefault(c => c.AttributeId == cat.AttributeId && c.CultureId == o.CultureId)
                where Lo != null
                select o.CultureId).ToList();

            cat.Locs = locs;
        }

        return query;
    }

    List<Culture> ISofLoc.ListCultures()
    {
        throw new NotImplementedException();
    }

    public string Locale(string resKey) => SofanDbContext.Localize
        .FirstOrDefault(x => x.ResKey.Trim().ToLower() == resKey.Trim().ToLower() && x.CultureId == CultureId)?.Value;

    public LocAttributes GetLocAttribute(long attrId)
    {
        var ao = SofanDbContext.LocAttributes.FirstOrDefault(c => c.AttributeId == attrId && c.CultureId == CultureId);
        ao ??= SofanDbContext.LocAttributes.FirstOrDefault(c => c.AttributeId == attrId && c.CultureId == DefaultCulture);
        return ao;
    }

    public LocAttributes GetLocalizeAttribute(long attrId, string langId)
    {
        var ao = SofanDbContext.LocAttributes.Include(c => c.Attribute)
            .FirstOrDefault(c => c.AttributeId == attrId && c.CultureId == langId);
        ao ??= SofanDbContext.LocAttributes.Include(c => c.Attribute)
            .FirstOrDefault(c => c.AttributeId == attrId && c.CultureId == DefaultCulture);
        return ao;
    }

    public LocEntry GetLocalizeEntry(string entryId)
    {
        //Lx = TranslateString("Untitled");
        var lx = SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId.ToString() == entryId && c.CultureId == CultureId);
        lx ??= SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId.ToString() == entryId && c.CultureId == DefaultCulture);
        return lx;
    }

    public LocEntry GetLocalizeEntryWithCulture(string entryId, string langId)
    {
        //Lx = TranslateString("Untitled");
        var lx = SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId.ToString() == entryId && c.CultureId == langId);
        lx ??= SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId.ToString() == entryId && c.CultureId == DefaultCulture);
        return lx;
    }

    public long SetAttrMaxId(long parentId)
    {
        long count = SofanDbContext.SofAttribute.Count();
        if (count == 0)
        {
            return 1;
        }

        count = SofanDbContext.SofAttribute.Count(c => c.ParentId == parentId);
        if (count == 0)
        {
            return parentId * 100 + 1;
        }

        return SofanDbContext.SofAttribute.Where(c => c.ParentId == parentId).Max(c => c.AttributeId) + 1;
    }

    List<Entry> ISofLoc.EntryTypedMasters(string id, long masterTypeId)
    {
        throw new NotImplementedException();
    }

    public async Task<long> PostAttribute(ViewAttribute vm)
    {
        try
        {
            var id = SetAttrMaxId(vm.ParentId);
            var langId = CultureId;
            if (vm.CultureId != null)
            {
                langId = vm.CultureId;
            }

            if (vm.AttributeId == 0)
            {
                var attribute = new SofAttribute
                {
                    AttributeId = id,
                    ParentId = vm.ParentId,
                    Status = vm.Status,
                    Order = vm.Order,
                };
                SofanDbContext.SofAttribute.Add(attribute);
                SofanDbContext.Entry(attribute).State = EntityState.Added;
                var locale = new LocAttributes
                {
                    AttributeId = id,
                    CultureId = langId,
                    Name = vm.Name,
                    Misc001 = vm.Misc01,
                    Misc002 = vm.Misc02,
                    Misc003 = vm.Misc03,
                    Status = vm.Status
                };
                SofanDbContext.Entry(locale).State = EntityState.Added;
            }
            else
            {
                id = vm.AttributeId;
                var a = SofanDbContext.SofAttribute.FirstOrDefault(c => c.AttributeId == id);
                var l = SofanDbContext.LocAttributes.FirstOrDefault(c => c.AttributeId == id && c.CultureId == langId);
                if (a != null)
                {
                    a.AttributeId = id;
                    a.Status = vm.Status;
                    a.Order = vm.Order;
                    if (vm.ParentId != 0)
                    {
                        a.ParentId = vm.ParentId;
                    }
                }

                if (l != null)
                {
                    if (vm.Name != null) l.Name = vm.Name;
                    if (vm.Misc01 != null) l.Misc001 = vm.Misc01;
                    if (vm.Misc02 != null) l.Misc002 = vm.Misc02;
                    if (vm.Misc03 != null) l.Misc003 = vm.Misc03;
                    l.Status = vm.Status;
                }
                else
                {
                    var locale = new LocAttributes
                    {
                        AttributeId = id,
                        CultureId = langId,
                        Name = vm.Name,
                        Misc001 = vm.Misc01,
                        Misc002 = vm.Misc02,
                        Misc003 = vm.Misc03,
                        Status = vm.Status
                    };
                    SofanDbContext.LocAttributes.Add(locale);
                }
            }

            await SofanDbContext.SaveChangesAsync();
            return id;
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public ViewAttribute AttributeById(long id)
    {
        var query = (from at in SofanDbContext.SofAttribute
            where at.AttributeId == id
            select new ViewAttribute
            {
                AttributeId = at.AttributeId,
                ParentId = at.ParentId,
                Order = at.Order,
                Status = at.Status,
            }).FirstOrDefault();
        if (query == null) return null;
        var lo = GetLocalizeAttribute(query.AttributeId, CultureId);
        var pa = GetLocalizeAttribute(query.ParentId, CultureId);
        query.Name = lo.Name;
        query.ParentName = pa.Name;
        query.Misc01 = lo.Misc001;
        query.Misc02 = lo.Misc002;
        query.Misc03 = lo.Misc003;
        return query;
    }

    public ViewAttribute AttributeByIdByCultureId(long id, string langId)
    {
        var q = (from at in SofanDbContext.SofAttribute
            where at.AttributeId == id
            select new ViewAttribute
            {
                AttributeId = at.AttributeId,
                ParentId = at.ParentId,
                Order = at.Order,
                Status = at.Status,
            }).FirstOrDefault();
        var lo = SofanDbContext.LocAttributes.FirstOrDefault(c => c.AttributeId == id && c.CultureId == langId);
        lo ??= SofanDbContext.LocAttributes.FirstOrDefault(c => c.AttributeId == id && c.CultureId == "en");
        if (lo == null) return null;
        q!.Name = lo.Name;
        q.Misc01 = lo.Misc001;
        q.Misc02 = lo.Misc002;
        q.Misc03 = lo.Misc003;
        return q;
    }

    public long GetMaxAttributeOrder(long parentId)
    {
        var query = SofanDbContext.SofAttribute.Where(c => c.ParentId == parentId).Select(c => (int?)c.Order).Max() + 1 ??
                    1;
        return query;
    }

    public bool DeleteAttribute(long id)
    {
        using (SofanDbContext)
        {
            if (id! <= 0) return false;
            var a = SofanDbContext.SofAttribute.FirstOrDefault(c => c.AttributeId == id);
            if (a != null) SofanDbContext.SofAttribute.Remove(a);
            return SofanDbContext.SaveChanges() > 0;
        }
    }

    async Task ISofLoc.DeleteRangeEntry(List<Entry> ids)
    {
        SofanDbContext.Entries.RemoveRange(ids);
        await SofanDbContext.SaveChangesAsync();
    }

    public Task DeleteRangeEntry(List<Entry> ids)
    {
        throw new NotImplementedException();
    }

    List<Entry> ISofLoc.EntryTypedSlaves(string id, long slaveTypeId)
    {
        throw new NotImplementedException();
    }

    public List<Entry> EntryTypedSlaves(string id, long slaveTypeId)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == slaveTypeId
            select re.SlaveEntry).ToList();
        return query;
    }

    public List<Entry> EntryTypedMasters(string id, long masterTypeId)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.SlaveId.ToString() == id
            where re.MasterEntry.AttributeId == masterTypeId
            select re.MasterEntry).ToList();
        return query;
    }

    public bool ToggleAttribute(long id)
    {
        using (SofanDbContext)
        {
            var a = SofanDbContext.SofAttribute.FirstOrDefault(c => c.AttributeId == id);
            if (a == null) return false;
            var active = a.Status;
            {
                active = active == 0 ? (byte)1 : (byte)0;
                a.Status = active;
                SofanDbContext.LocAttributes.Where(c => c.AttributeId == id).ToList().ForEach(c => c.Status = active);
                if (SofanDbContext.SaveChanges() > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool ReOrderAttribute(long id, byte value)
    {
        var a = SofanDbContext.SofAttribute.FirstOrDefault(c => c.AttributeId == id);
        if (a == null) return false;
        a.Order = value;
        var res = SofanDbContext.SaveChanges() > 0;
        return res;
    }

    public Guid PostEntry(long attrId, byte status = 1, int order = 1)
    {
        var entryId = Guid.NewGuid();
        Entry cIe = new()
        {
            EntryId = entryId,
            AttributeId = attrId,
            AddedBy = string.IsNullOrEmpty(UserId) ? entryId : Guid.Parse(UserId),
            DateAdded = DateTime.UtcNow.Ticks,
            Order = order,
            Status = status
        };
        SofanDbContext.Entries.Add(cIe);
        SofanDbContext.Entry(cIe).State = EntityState.Added;
        return entryId;
    }

    public void PostLocale(long entryId,
        string cultureId,
        string title,
        long propDate = 0,
        string subTitle = "",
        string shortDesc = "",
        string prop01 = "",
        string prop02 = "",
        string prop03 = "",
        string propUrl = "",
        string imageUrl = "",
        string content = "",
        byte status = 0)
    {
        var loc = SofanDbContext.LocEntry.Include(x => x.Entry)
            .FirstOrDefault(c => c.EntryId == entryId && c.CultureId == cultureId);
        if (loc == null)
        {
            LocEntry cLe = new()
            {
                EntryId = entryId,
                CultureId = cultureId,
                Title = title,
                PropDate = propDate,
                SubTitle = subTitle,
                ShortDesc = shortDesc,
                Prop01 = prop01,
                Prop02 = prop02,
                Prop03 = prop03,
                Content = content,
                PropUrl = propUrl,
                ImageUrl = imageUrl,
                Status = status
            };
            SofanDbContext.LocEntry.Add(cLe);
            SofanDbContext.Entry(cLe).State = EntityState.Added;
        }
        else
        {
            if (!string.IsNullOrEmpty(title))
            {
                loc.Title = title;
            }

            if (!string.IsNullOrEmpty(subTitle))
            {
                loc.SubTitle = subTitle;
            }

            if (!string.IsNullOrEmpty(prop01))
            {
                loc.Prop01 = prop01;
            }

            if (!string.IsNullOrEmpty(prop02))
            {
                loc.Prop02 = prop02;
            }

            if (!string.IsNullOrEmpty(prop03))
            {
                loc.Prop03 = prop03;
            }

            if (!string.IsNullOrEmpty(shortDesc))
            {
                loc.ShortDesc = shortDesc;
            }

            if (!string.IsNullOrEmpty(propUrl))
            {
                loc.PropUrl = propUrl;
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                loc.ImageUrl = imageUrl;
            }

            if (propDate != 0)
            {
                loc.PropDate = propDate;
            }

            if (!string.IsNullOrEmpty(content))
            {
                loc.Content = content;
            }

            SofanDbContext.Entry(loc).State = EntityState.Modified;
        }
    }

    public void PostRelated(Guid masterId, Guid slaveId, byte status = 1, int order = 0)
    {
        if (SofanDbContext.RelatedEntries.Any(c => c.MasterId == masterId & c.SlaveId == slaveId)) return;
        var coRe = new RelatedEntries
        {
            MasterId = masterId,
            SlaveId = slaveId,
            Status = status,
            Order = order
        };
        SofanDbContext.RelatedEntries.Add(coRe);
        SofanDbContext.Entry(coRe).State = EntityState.Added;
    }

    public void AddAction(Guid actOn, long actId)
    {
        SofanDbContext.Activities.Add(new Activity
        {
            ActById = Guid.Parse(UserId),
            ActOnId = actOn,
            ActionId = actId,
            Status = 0,
            ActionDate = DateTime.UtcNow,
        });
    }

    public VmEntry GetEntry(string id, string langId)
    {
        var query = (from e in SofanDbContext.Entries
            where e.EntryId.ToString() == id
            join lo in SofanDbContext.LocEntry on e.EntryId equals lo.EntryId
            where lo.CultureId == langId
            select new VmEntry
            {
                Id = id,
                CultureId = langId,
                TypeId = e.AttributeId,
                Title = !string.IsNullOrEmpty(lo.Title) ? lo.Title : "",
                SubTitle = !string.IsNullOrEmpty(lo.SubTitle) ? lo.SubTitle : "",
                ShortDesc = !string.IsNullOrEmpty(lo.ShortDesc) ? lo.ShortDesc : "",
                Prop01 = !string.IsNullOrEmpty(lo.Prop01) ? lo.Prop01 : "",
                Prop02 = !string.IsNullOrEmpty(lo.Prop02) ? lo.Prop02 : "",
                Prop03 = !string.IsNullOrEmpty(lo.Prop03) ? lo.Prop03 : "",
                Reference = !string.IsNullOrEmpty(lo.Reference) ? lo.Reference : "",
                PropUrl = !string.IsNullOrEmpty(lo.PropUrl) ? lo.PropUrl : "",
                ImageUrl = !string.IsNullOrEmpty(lo.ImageUrl) ? lo.ImageUrl : "",
                PropDate = new DateTime(lo.PropDate),
                Content = !string.IsNullOrEmpty(lo.Content) ? lo.Content : "",
                Status = e.Status,
                Order = e.Order
            }).FirstOrDefault();
        if (query == null) return null;
        if (!string.IsNullOrEmpty(query.ImageUrl))
        {
            if (IsValidJson(query.ImageUrl))
            {
                var postImages = JsonConvert.DeserializeObject<PostImages>(query.ImageUrl);
                if (postImages is not null)
                {
                    query.PostImage = JsonConvert.DeserializeObject<PostImages>(query.ImageUrl);
                }
            }
        }

        var chTypeId = ListInitials().FirstOrDefault(c => c.EntryId.ToString() == id);
        if (chTypeId == null) return query;
        if (chTypeId.ChildTypeId != -1)
        {
            query!.ChildTypeId = chTypeId.ChildTypeId;
        }

        return query;
    }

    public List<VmEntry> ListPageSections(string id, string langId)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == SeedEntryTypes.Section.PropId ||
                  re.SlaveEntry.AttributeId == SeedEntryTypes.Slider.PropId
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == langId
            orderby lo.Entry.Order
            select new VmEntry
            {
                Id = re.SlaveId.ToString(),
                CultureId = langId,
                TypeId = re.SlaveEntry.AttrId,
                Title = !string.IsNullOrEmpty(lo.Title) ? lo.Title : "",
                SubTitle = !string.IsNullOrEmpty(lo.SubTitle) ? lo.SubTitle : "",
                ShortDesc = !string.IsNullOrEmpty(lo.ShortDesc) ? lo.ShortDesc : "",
                Prop01 = !string.IsNullOrEmpty(lo.Prop01) ? lo.Prop01 : "",
                Prop02 = !string.IsNullOrEmpty(lo.Prop02) ? lo.Prop02 : "",
                Prop03 = !string.IsNullOrEmpty(lo.Prop03) ? lo.Prop03 : "",
                Reference = !string.IsNullOrEmpty(lo.Reference) ? lo.Reference : "",
                PropUrl = !string.IsNullOrEmpty(lo.PropUrl) ? lo.PropUrl : "",
                ImageUrl = !string.IsNullOrEmpty(lo.ImageUrl) ? lo.ImageUrl : "",
                PropDate = new DateTime(lo.PropDate),
                Content = !string.IsNullOrEmpty(lo.Content) ? lo.Content : "",
                Status = lo.Entry.Status
            }).ToList();
        return query;
    }

    public string TimeDifference(DateTime dateGiven, DateTime dateRef)
    {
        var difference = dateRef - dateGiven;
        if (difference.TotalSeconds < 60)
        {
            return difference.Seconds == 1 ? "just now" : $"since {difference.Seconds} secs";
        }

        if (difference.TotalMinutes < 60)
        {
            return difference.Minutes == 1 ? "since 1 minute" : $"since {difference.Minutes} minutes";
        }

        if (difference.TotalHours < 24)
        {
            return difference.Hours == 1 ? "since 1 hour" : $"since {difference.Hours} hours";
        }

        switch (difference.TotalDays)
        {
            case < 30:
                return difference.Days == 1 ? "since 1 day" : $"since {difference.Days} days";
            case < 365:
            {
                var months = (int)(difference.TotalDays / 30);
                return months == 1 ? "since 1 month" : $"since {months} months";
            }
            default:
            {
                var years = (int)(difference.TotalDays / 365);
                return years == 1 ? "since 1 year" : $"since {years} years";
            }
        }
    }

    public List<ViewProp002> ListChilds(long id)
    {
        var query = (from at in SofanDbContext.SofAttribute
            where at.ParentId == id
            where at.AttributeId != 0
            orderby at.Order
            select new ViewProp002
            {
                Id = at.AttributeId,
                Priority = at.Order,
                Status = at.Status,
            }).ToList();
        foreach (var r in query)
        {
            var lo = GetLocAttribute(r.Id);
            if (lo == null) continue;
            r.Name = lo.Name;
            r.Icon = lo.Misc001;
        }

        return query;
    }

    public List<VmImage> EntryImages(string id)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == SeedTypes.Image.PropId
            where re.Status == 1
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == "en"
            select new VmImage
            {
                Id = lo.EntryId.ToString(),
                Title = lo.Title ?? "",
                Url = lo.ImageUrl ?? ""
            }).ToList();
        return query;
    }

    public List<VmInitialEntry> ListInitials()
    {
        List<VmInitialEntry> secs = [];
        secs.AddRange(SeedSite.Deps);
        secs.AddRange(SeedSite.CustemPages);
        secs.AddRange(SeedHomeSections.Secs);
        secs.AddRange(SeedAboutSections.Secs);
        return secs;
    }

    public List<VmAddress> EntryAddresses(string id) => (from re in SofanDbContext.RelatedEntries
        where re.MasterId.ToString() == id
        where re.SlaveEntry.AttributeId == SeedContact.Address.PropId
        join li in SofanDbContext.Address on re.SlaveId equals li.AddressId
        join go in SofanDbContext.LocAttributes on li.GovId equals go.AttributeId
        where go.CultureId == "en"
        join co in SofanDbContext.LocAttributes on li.CountryId equals co.AttributeId
        where go.CultureId == "en"
        select new VmAddress
        {
            Id = li.AddressId.ToString(),
            CountryId = li.CountryId,
            GovId = li.GovId,
            AreaId = li.AreaId,
            Block = li.Block,
            House = li.House,
            Floor = li.Floor,
            ApartmentNo = li.ApartmentNo,
            OfficeNo = li.OfficeNo,
            Avenue = li.Avenue,
            Street = li.Street,
            Country = co.Name,
            City = go.Name,
        }).ToList();

    public List<VmEntry> EntryPosts(string id, long typeId, string langId)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.Status != 2
            where re.SlaveEntry.AttributeId == typeId
            orderby re.SlaveEntry.Order
            select new VmEntry
            {
                MasterId = id,
                Id = re.SlaveId.ToString(),
                TypeId = re.SlaveEntry.AttributeId,
                Status = re.SlaveEntry.Status,
                Order = re.SlaveEntry.Order,
                //PostImage=JsonConvert.DeserializeObject<PostImages>(lo.ImageUrl)
            }).ToList();
        foreach (var item in query)
        {
            var lo = GetLocalizeEntry(item.Id);
            item.Title = lo.Title;
            item.SubTitle = !string.IsNullOrEmpty(lo.SubTitle) ? lo.SubTitle : "";
            item.ShortDesc = !string.IsNullOrEmpty(lo.ShortDesc) ? lo.ShortDesc : "";
            item.PropUrl = !string.IsNullOrEmpty(lo.PropUrl)
                ? lo.PropUrl.Contains("http") ? lo.PropUrl : $"{langId}/{lo.PropUrl}"
                : "";
            item.ImageUrl = !string.IsNullOrEmpty(lo.ImageUrl) ? lo.ImageUrl : "";
            item.Prop01 = !string.IsNullOrEmpty(lo.Prop01) ? lo.Prop01 : "";
            item.Prop02 = !string.IsNullOrEmpty(lo.Prop02) ? lo.Prop02 : "";
            item.PropDate = new DateTime(lo.PropDate);
            List<string> locs = [];
            locs.AddRange(from o in ListCultures()
                let locale =
                    SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId.ToString() == item.Id && c.CultureId == o.CultureId)
                where locale != null
                select o.CultureId);
            item.Locs = locs;
            var postImages = JsonConvert.DeserializeObject<PostImages>(item.ImageUrl);
            if (postImages is not null)
            {
                item.PostImage = JsonConvert.DeserializeObject<PostImages>(item.ImageUrl);
            }

            var addresses = EntryAddresses(item.Id);
            if (addresses is not null)
            {
                item.Address = addresses.FirstOrDefault();
            }
        }

        return query;
    }

    public List<VmEntry> CountedEntryPosts(string id, long typeId, string langId, int count)
    {
        var query = SofanDbContext.RelatedEntries
            .Where(re => re.MasterId.ToString() == id)
            .Where(re => re.SlaveEntry.Status == 1)
            .Where(re => re.SlaveEntry.AttributeId == typeId)
            .OrderBy(re => re.Order)
            .Take(count)
            .Select(re => new VmEntry
            {
                MasterId = id,
                Id = re.SlaveId.ToString(),
                TypeId = re.SlaveEntry.AttributeId,
                Status = re.SlaveEntry.Status,
                Order = re.Order,
            })
            .ToList();

        foreach (var item in query)
        {
            var lo = GetLocalizeEntry(item.Id);
            item.Title = lo.Title;
            item.PropUrl = !string.IsNullOrEmpty(lo.PropUrl)
                ? lo.PropUrl.Contains("http") ? lo.PropUrl : $"{langId}/{lo.PropUrl}"
                : "";
            item.ImageUrl = !string.IsNullOrEmpty(lo.ImageUrl) ? lo.ImageUrl : "";
            item.SubTitle = !string.IsNullOrEmpty(lo.SubTitle) ? lo.SubTitle : "";
            item.ShortDesc = !string.IsNullOrEmpty(lo.ShortDesc) ? lo.ShortDesc : "";
            item.Prop01 = !string.IsNullOrEmpty(lo.Prop01) ? lo.Prop01 : "";
            item.Prop02 = !string.IsNullOrEmpty(lo.Prop02) ? lo.Prop02 : "";
            item.Prop03 = !string.IsNullOrEmpty(lo.Prop03) ? lo.Prop03 : "";
            item.Content = !string.IsNullOrEmpty(lo.Content) ? lo.Content : "";
            item.PropDate = new DateTime(lo.PropDate);

            List<string> locs = [];
            locs.AddRange(from o in ListCultures()
                let locale =
                    SofanDbContext.LocEntry.FirstOrDefault(c =>
                        c.EntryId.ToString() == item.Id && c.CultureId == o.CultureId)
                where locale != null
                select o.CultureId);
            item.Locs = locs;

            var postImages = JsonConvert.DeserializeObject<PostImages>(item.ImageUrl);
            if (postImages is not null)
            {
                item.PostImage = JsonConvert.DeserializeObject<PostImages>(item.ImageUrl);
            }
        }

        return query;
    }

    public List<VmImage> GalleryImages(string id)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == SeedTypes.Image.PropId
            where re.Status == 1
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == "en"
            select new VmImage
            {
                Id = lo.EntryId.ToString(),
                Title = lo.Title ?? "",
                Url = lo.ImageUrl ?? ""
            }).ToList();
        return query;
    }

    public List<VmImage> EntryFiles(string id)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == SeedTypes.File.PropId
            where re.Status == 1
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == "en"
            select new VmImage
            {
                Id = lo.EntryId.ToString(),
                Title = lo.Title ?? "",
                Url = lo.ImageUrl ?? ""
            }).ToList();
        return query;
    }

    public List<VmVideo> EntryVideos(string id, string langId)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.Status != 2
            where re.SlaveEntry.AttributeId == SeedTypes.Video.PropId
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == langId
            orderby re.Order
            select new VmVideo
            {
                MasterId = id,
                Id = re.SlaveId.ToString(),
                Status = re.SlaveEntry.Status,
                Order = re.SlaveEntry.Order,
                Title = lo.Title,
                ShortDesc = lo.ShortDesc,
                VideoUrl = lo.PropUrl,
                ThumbUrl = lo.ImageUrl,
                PubDate = new DateTime(lo.PropDate),
            }).ToList();
        foreach (var item in query)
        {
            List<string> locs = [];
            locs.AddRange(from o in ListCultures()
                let Lo = SofanDbContext.LocEntry.FirstOrDefault(c =>
                    c.EntryId.ToString() == item.Id && c.CultureId == o.CultureId)
                where Lo != null
                select o.CultureId);
            item.Locs = locs;
        }

        return query;
    }
    public (List<VmEntry> Posts, PaginationInfo Pagination) PagedEntryPosts(string entryId, long childTypeId,
        string cultureId, int pageNumber = 1, int pageSize = 6)
    {
        var allPosts = EntryPosts(entryId, childTypeId, cultureId)
            .Where(c => c.Status == 1)
            .ToList();
        var paginationInfo = new PaginationInfo
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = allPosts.Count
        };
        var paginatedPosts = allPosts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return (paginatedPosts, paginationInfo);
    }
    public VmVideoGallery VideoGallery(string id)
    {
        var query = (from lo in SofanDbContext.LocEntry
            where lo.CultureId == CultureId
            where lo.EntryId.ToString() == id
            select new VmVideoGallery
            {
                Id = lo.EntryId.ToString(),
                Name = lo.Title,
                ShortDesc = lo.ShortDesc,
                Cover = lo.ImageUrl
            }).FirstOrDefault();
        if (query != null)
        {
            query.Videos = GalleryVideos(id);
        }

        return query;
    }

    public List<VmVideo> GalleryVideos(string id)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == SeedTypes.Video.PropId
            where re.Status == 1
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == "en"
            select new VmVideo
            {
                Id = lo.EntryId.ToString(),
                Title = lo.Title,
                VideoUrl = lo.PropUrl,
                ThumbUrl = lo.ImageUrl,
            }).ToList();
        return query;
    }

    public VmPhotoGallery PhotoGallery(string id)
    {
        var query = (from lo in SofanDbContext.LocEntry
            where lo.CultureId == CultureId
            where lo.EntryId.ToString() == id
            select new VmPhotoGallery
            {
                Id = lo.EntryId.ToString(),
                Status = lo.Entry.Status,
                Name = lo.Title,
                ShortDesc = lo.ShortDesc,
                Url = lo.PropUrl,
                Cover = lo.ImageUrl,
            }).FirstOrDefault();
        if (query != null)
        {
            query.Images = GalleryImages(id);
        }

        return query;
    }

    public List<VmVideoGallery> EntryVideoGalleries(string id)
    {
        var query = (from re in SofanDbContext.RelatedEntries
            where re.MasterId.ToString() == id
            where re.SlaveEntry.AttributeId == SeedTypes.PlayList.PropId
            join lo in SofanDbContext.LocEntry on re.SlaveId equals lo.EntryId
            where lo.CultureId == CultureId
            select new VmVideoGallery
            {
                Id = lo.EntryId.ToString(),
                Name = lo.Title ?? "",
                ShortDesc = lo.ShortDesc ?? "",
                Cover = lo.ImageUrl,
                Url = lo.PropUrl,
                Status = lo.Entry.Status,
                Order = lo.Entry.Order
            }).ToList();
        foreach (var I in query)
        {
            I.Videos = EntryVideos(I.Id, CultureId);
            List<string> locs = [];
            locs.AddRange(from o in ListCultures()
                let Lo =
                    SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId.ToString() == I.Id && c.CultureId == o.CultureId)
                where Lo != null
                select o.CultureId);
            I.Locs = locs;
        }

        return query;
    }

    public Guid PostLink(VmSiteLink vm)
    {
        Guid linkId;
        if (vm.LinkId == "0")
        {
            linkId = PostEntry(vm.TypeId);
            SofanDbContext.LocEntry.Add(new LocEntry()
            {
                EntryId = linkId,
                CultureId = "en",
                Title = vm.Title,
                PropUrl = vm.Url,
                Prop01 = vm.Target,
                Status = vm.Status
            });
            PostRelated(Guid.Parse(vm.MasterId), linkId);
        }
        else
        {
            linkId = Guid.Parse(vm.LinkId);
            var link = SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId == linkId && c.CultureId == "en");
            if (link != null)
            {
                link.PropUrl = vm.Url;
                link.Prop01 = vm.Target;
                link.Status = vm.Status;
            }
            else
            {
                SofanDbContext.LocEntry.Add(new LocEntry()
                {
                    EntryId = linkId,
                    CultureId = "en",
                    Title = vm.Title,
                    PropUrl = vm.Url,
                    Prop01 = vm.Target,
                    Status = vm.Status
                });
            }
        }

        return linkId;
    }

    public List<VmSiteLink> EntryLinks(string id) => (from re in SofanDbContext.RelatedEntries
        where re.MasterId.ToString() == id
        where re.SlaveEntry.Status == 5
        where re.SlaveEntry.AttributeId != 40102
        join li in SofanDbContext.LocEntry on re.SlaveId equals li.EntryId
        where li.CultureId == "en"
        join lo in SofanDbContext.LocAttributes on re.SlaveEntry.AttrId equals lo.AttrId
        where lo.CultureId == "en"
        select new VmSiteLink
        {
            LinkId = li.EntryId.ToString(),
            Url = li.PropUrl,
            TypeId = lo.AttrId,
            TypeName = lo.Name,
            TypeIcon = lo.Misc001,
            Status = re.MasterEntry.Status
        }).ToList();

    public List<VmSiteLink> EntryEmails(string id) => (from re in SofanDbContext.RelatedEntries
        where re.MasterId.ToString() == id
        where re.SlaveEntry.Status == 5
        where re.SlaveEntry.AttributeId == SeedContact.Email.PropId
        join li in SofanDbContext.LocEntry on re.SlaveId equals li.EntryId
        where li.CultureId == "en"
        join lo in SofanDbContext.LocAttributes on re.SlaveEntry.AttrId equals lo.AttrId
        where lo.CultureId == "en"
        select new VmSiteLink
        {
            LinkId = li.EntryId.ToString(),
            Url = li.PropUrl,
            TypeId = lo.AttrId,
            TypeName = lo.Name,
            TypeIcon = lo.Misc001,
            Status = re.MasterEntry.Status
        }).ToList();

    public List<VmPhone> EntryPhones(string id) => (from re in SofanDbContext.RelatedEntries
        where re.MasterId.ToString() == id
        where re.SlaveEntry.Status == 5
        where re.SlaveEntry.SofAttribute.ParentId == 402
        join li in SofanDbContext.LocEntry on re.SlaveId equals li.EntryId
        where li.CultureId == "en"
        join lo in SofanDbContext.LocAttributes on re.SlaveEntry.AttrId equals lo.AttrId
        where lo.CultureId == "en"
        select new VmPhone
        {
            Id = li.EntryId.ToString(),
            PhoneNo = li.Title,
            TypeId = lo.AttrId,
            TypeName = lo.Name,
            TypeIcon = lo.Misc001,
            Status = re.MasterEntry.Status
        }).ToList();

    public Guid PostPhone(VmPhone vm)
    {
        Guid phoneId;
        if (vm.Id == "0")
        {
            phoneId = PostEntry(SeedContact.Phone.PropId);
            SofanDbContext.LocEntry.Add(new LocEntry()
            {
                EntryId = phoneId,
                CultureId = "en",
                Title = "Phone number",
                PropUrl = vm.PhoneNo,
                Status = vm.Status
            });
        }
        else
        {
            phoneId = Guid.Parse(vm.Id);
            var phone = SofanDbContext.LocEntry.FirstOrDefault(c => c.EntryId == phoneId & c.CultureId == "en");
            if (phone == null)
            {
                SofanDbContext.LocEntry.Add(new LocEntry()
                {
                    EntryId = phoneId,
                    CultureId = "en",
                    Title = "Phone number",
                    PropUrl = vm.PhoneNo,
                    Status = vm.Status
                });
            }
            else
            {
                phone.PropUrl = vm.PhoneNo;
                phone.Status = vm.Status;
                var entry = SofanDbContext.Entries.FirstOrDefault(c => c.EntryId == phoneId);
                if (entry != null) entry.Status = vm.Status;
            }
        }

        PostRelated(Guid.Parse(vm.MasterId), phoneId);
        return phoneId;
    }

    public bool IsImage(string fileUrl)
    {
        var extension = Path.GetExtension(fileUrl);
        string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg", ".webp"];
        return Array.Exists(imageExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }

    public List<VmEntry> ListPages()
    {
        var query = (from lo in SofanDbContext.LocEntry
            where lo.Entry.AttributeId == SeedEntryTypes.Department.PropId
            where lo.CultureId == CultureId
            orderby lo.Entry.Order
            select new VmEntry
            {
                Id = lo.EntryId.ToString(),
                Title = lo.Title,
            }).ToList();
        return query;
    }

    public List<RelatedEntryAttributes> ListEntryAttributes(string id)
    {
        var query = (from re in SofanDbContext.RelatedEntryAttributes
            where re.EntryId.ToString() == id
            where re.SofAttribute.ParentId == SeedStatic.EntryContentTypes.PropId
            select re).ToList();
        return query;
    }
    public async Task<bool> ToggleEntryStatus(string id)
    {
        var a = SofanDbContext.Entries.FirstOrDefault(c => c.EntryId.ToString() == id);
        if (a == null) return false;
        a.Status = a.Status == 0 ? (byte)1 : (byte)0;
        SofanDbContext.Entry(a).State = EntityState.Modified;
        return await SofanDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> ReorderEntry(string id, string masterId, int order)
    {
        var re = SofanDbContext.RelatedEntries.FirstOrDefault(c =>
            c.MasterId.ToString() == masterId && c.SlaveId.ToString() == id);
        if (re != null)
        {
            re.Order = order;
        }

        return await SofanDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteEntry(string id)
    {
        var re = SofanDbContext.RelatedEntries.Where(c => c.MasterId.ToString() == id || c.SlaveId.ToString() == id)
            .ToList();
        if (re.Count != 0)
        {
            SofanDbContext.RelatedEntries.RemoveRange(re);
        }

        var entry = SofanDbContext.Entries.FirstOrDefault(c => c.EntryId.ToString() == id);
        if (entry != null) SofanDbContext.Entries.Remove(entry);
        return await SofanDbContext.SaveChangesAsync() > 0;
    }
    public string PageCover(string id)
    {
        var query = (from e in SofanDbContext.Entries
            where e.EntryId.ToString() == id
            join lo in SofanDbContext.LocEntry on e.EntryId equals lo.EntryId
            where lo.CultureId=="en"
            select new VmEntry
            {
                ImageUrl = !string.IsNullOrEmpty(lo.ImageUrl) ? lo.ImageUrl : "",
            }).FirstOrDefault();
        var defaultImage=$"{UrlSite}/assets/images/bg/default-cover.jpg";
        if (query == null || string.IsNullOrEmpty(query.ImageUrl) || !IsValidJson(query.ImageUrl)) return defaultImage;
        var postImages = JsonConvert.DeserializeObject<PostImages>(query.ImageUrl);
        if (postImages is not null)
        {
            query.PostImage = JsonConvert.DeserializeObject<PostImages>(query.ImageUrl);
        }
        return query.PostImage.CoverImage;
    }

    public bool IsValidJson(string jsonString)
    {
        try
        {
            JObject.Parse(jsonString);

            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
}