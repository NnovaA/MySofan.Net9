using Site.lib.ViewModels;
using Sofan.Hub;

namespace Site.lib.Sofan.Seed;

public class SeedSite
{
    private static readonly long BaseId = SeedEntryTypes.Department.PropId;//301
    public static VmInitialEntry Home => new() { EntryId = GuidHelper.EntryIdPadRight(BaseId,1),AttrId=BaseId, Title = "Home", RefName = "Home",Url = "home",Order = 1};
    public static VmInitialEntry About => new() { EntryId = GuidHelper.EntryIdPadRight(BaseId,2),AttrId=BaseId, Title = "About Us", RefName = "About Us",Url = "about",Order = 2};
    public static VmInitialEntry Products => new() { EntryId = GuidHelper.EntryIdPadRight(BaseId,3),AttrId=BaseId, Title = "Products", RefName = "Products",Url = "products",Order = 3,ChildTypeId = SeedTypes.Product.PropId,IsListed = true};
    public static VmInitialEntry Careers => new() { EntryId = GuidHelper.EntryIdPadRight(BaseId,4),AttrId=BaseId, Title = "Careers", RefName = "Careers",Url = "careers",Order = 4,ChildTypeId = SeedTypes.Applicant.PropId};
    public static VmInitialEntry ContactUs => new() { EntryId = GuidHelper.EntryIdPadRight(SeedEntryTypes.Page.PropId,5),AttrId=SeedEntryTypes.Page.PropId, Title = "Contact Us", RefName = "Contact Us",Url = "contact",Order = 5,ChildTypeId = SeedTypes.Request.PropId,IsListed = true,MenuIcon = "<i class=\"ki-outline ki-information-4 fs-2x\"></i>"};
    public static VmInitialEntry Privacy => new() { EntryId = GuidHelper.EntryIdPadRight(SeedEntryTypes.Page.PropId,6),AttrId=SeedEntryTypes.Page.PropId, Title = "Privacy Policy", RefName = "Privacy Policy",Url = "privacy",Order = 6,MenuIcon = "<i class=\"ki-outline ki-medal-star fs-2x\"></i>"};
    public static VmInitialEntry Branches => new() { EntryId = GuidHelper.EntryIdPadRight(BaseId,7),AttrId=BaseId, Title = "Branches", RefName = "Branches",Url = "branches",Order = 7,ChildTypeId = SeedTypes.Branch.PropId};
    public static readonly List<VmInitialEntry> Deps = [Home, About, Products, Careers,Branches];
    public static readonly List<VmInitialEntry> CustemPages = [ContactUs,Privacy];
}
public class SeedHomeSlider
{
    private static readonly long BaseId = SeedEntryTypes.Slider.PropId;//303
    public static VmInitialEntry HomeSlider => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000001"),AttrId=BaseId, Title = "Main Slider", RefName = "Home - Main Slider",Order = 1};
}
public class SeedHeader
{
    private static readonly long BaseId = SeedEntryTypes.Header.PropId;
    public static VmInitialEntry MainHeader => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000001"),AttrId=BaseId, Title = "Main Header", RefName = "Site - Main Header",Order = 1};
}
public class SeedFooter
{
    private static readonly long BaseId = SeedEntryTypes.Footer.PropId;
    public static VmInitialEntry MainFooter => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000001"),AttrId=BaseId, Title = "Main Footer", RefName = "Site - Main Footer",Order = 1};
}

public class SeedHomeSections
{
    private static readonly long BaseId = SeedEntryTypes.Section.PropId;//304
    public static VmInitialEntry HomeSec001 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000001"),AttrId=BaseId, Title = "Who We Are", RefName = "Home - Section one",Order = 2};
    public static VmInitialEntry HomeSec002 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000002"),AttrId=BaseId, Title = "Why We Do It", RefName = "Home - Section Two",Order = 3,IsListed = true,ChildTypeId = SeedTypes.Image.PropId};
    public static VmInitialEntry HomeSec003 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000003"),AttrId=BaseId, Title = "Featured Products", RefName = "Home - Section Three",Order = 4,IsListed = true,ChildTypeId = SeedTypes.Product.PropId};
    public static VmInitialEntry HomeSec004 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000004"),AttrId=BaseId, Title = "What We Do", RefName = "Home - Section Four",Order = 4,IsListed = true,ChildTypeId = SeedTypes.Counter.PropId};
    public static readonly List<VmInitialEntry> Secs=[HomeSec001,HomeSec002,HomeSec003,HomeSec004];
}
public class SeedAboutSections
{
    private static readonly long BaseId = SeedEntryTypes.Section.PropId;//303
    public static VmInitialEntry AboutSec001 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000011"),AttrId=BaseId, Title = "About Sofan Steel", RefName = "About - Section One",Order = 1};
    public static VmInitialEntry AboutSec002 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000012"),AttrId=BaseId, Title = "Counters",ChildTypeId = SeedTypes.Counter.PropId,RefName = "About - Section Two",Order = 2};
    public static VmInitialEntry AboutSec003 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000013"),AttrId=BaseId, Title = "Our Growth",ChildTypeId = SeedTypes.Counter.PropId, RefName = "About - Section Three",Order = 3};
    public static VmInitialEntry AboutSec004 => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-{BaseId}000000015"),AttrId=BaseId, Title = "Company Vision", RefName = "About - Section Four",Order = 4};
    public static readonly List<VmInitialEntry> Secs=[AboutSec001,AboutSec002,AboutSec003,AboutSec004];
}

public class SeedSiteInfo
{
    public static VmInitialEntry Address => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-123000000001"),AttrId=SeedContact.Address.PropId, Title = "Your Address here", RefName = "Site - Address",Order = 1};
    public static VmInitialEntry Phone => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-123000000002"),AttrId=SeedContact.Phone.PropId, Title = "Your Phone here", RefName = "Site - Phone",Order = 1};
    public static VmInitialEntry WhatsApp => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-123000000003"),AttrId=SeedContact.Phone.PropId, Title = "Your Whats app here", RefName = "Site - WhatsApp",Order = 1};
    public static VmInitialEntry Email => new() { EntryId = Guid.Parse($"{ConfigHub.GuidPattern}-123000000004"),AttrId=SeedContact.Phone.PropId, Title = "Your Email here", RefName = "Site - Email",Order = 1};
}