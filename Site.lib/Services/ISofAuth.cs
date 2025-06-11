using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Site.lib.Services;

public interface IOreAuth
{
    string UserId { get; }
    Task<OreUser> UserById(string id);
    Task<OreUser> CurrentUser();
    string UserToken { get; }
    string UserName { get; }
    string UserEmail { get; }
    Task<OreUser> UserByEmail(string email);
    string CultureId { get; }
    string GenerateToken(OreUser user);
    VmOrePermission CheckUserRoutePermission(string route, string userId);
    string Locale(string resourceKey);
    List<VmActivity> UserActivity(string id);
    Task<ResponseDto> CheckAdmin(string id);
    void AddRole(string roleName);
    List<VmOrePermission> ListRolePermission(string roleId);
    OreUser CreateUser();
    Task BlockUser(string id);
    LocEntry GetLocalizeEntry(string entryId);
    ResponseDto IsSignedIn();
    Task<string> UpdateRolePermissions(PermissionViewModel vm);
    Task<bool> ChangePassword(VmPass vm);
    Task<string> UpdateUserCulture(string cultureId);
}

public class OreAuth(
    OreDb oreDb,
    IHttpContextAccessor ctx,
    RoleManager<IdentityRole> roleManager,
    SignInManager<OreUser> signInManager,
    IUserStore<OreUser> userStore,
    UserManager<OreUser> userManager
) : IOreAuth
{
    private readonly HttpContext _ctx = ctx.HttpContext;
    private readonly ResponseDto _res=new();
    
    public string UserId => _ctx.User.Identity is { IsAuthenticated: true } ? new Guid(_ctx.User.Claims.First(c => c.Type == "Id").Value).ToString() : "";
    public string CultureId => _ctx.User.Identity is { IsAuthenticated: true } ? _ctx.User.Claims.First(c => c.Type == "CultureId").Value: "en";

    public string UserToken => _ctx.User.Identity is { IsAuthenticated: true } ? _ctx.User.Claims.First(c => c.Type == "Token").Value : "";

    public string UserName => _ctx.User.Identity is { IsAuthenticated: true }
        ? _ctx.User.Claims.First(c => c.Type == "FullName").Value
        : "";

    public string UserEmail => _ctx.User.Identity is { IsAuthenticated: true }
        ? _ctx.User.Claims.First(c => c.Type == "EmailAddress").Value
        : "";
    public async Task<string> UpdateUserCulture(string cultureId)
    {
        var user = oreDb.Users.FirstOrDefault(c=>c.Id == UserId);
        if (user == null) return cultureId;
        user.CultureId = cultureId;
        await oreDb.SaveChangesAsync();
        return cultureId;
    }
    public string GenerateToken(OreUser user)
    {
        if (user.UserName == null) return "";
        if (user.Email == null) return "";
        var authClaims = new[]
        {
            new Claim(type: "Id", user.Id),
            new Claim(type: "NameIdentifier", user.Id),
            new Claim(type: "Name", user.UserName),
            new Claim(type: "FullName", user.FullName),
            new Claim(type: "Email", user.Email),
            new Claim(type: "CultureId", user.CultureId),
            new Claim(type: "Phone", string.IsNullOrEmpty(user.PhoneNumber) ? "" : user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Aud, ConfigHub.Site),
        };
        // var roles =  userManager.GetRolesAsync(user).Result;
        // authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigHub.Key));
        var tokenExpiryTimeInHour = Convert.ToInt64(ConfigHub.ExpirationHours);
        var token = new JwtSecurityToken(
            issuer: ConfigHub.Site,
            audience: null,
            claims: authClaims,
            expires: DateTime.Now.AddDays(tokenExpiryTimeInHour),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public ResponseDto IsSignedIn()
    {
        var isSignedIn = signInManager.IsSignedIn(_ctx.User);
        if (isSignedIn) return _res;
        _res.IsSuccess = false;
        _res.StatusCode = 302;
        _res.Message = "Redirect to sign in";
        return _res;
    }

    public VmOrePermission CheckUserRoutePermission(string route, string userId)
    {
        var permission = (from ue in oreDb.UserRoles
            where ue.UserId == userId
            join re in oreDb.Permissions on ue.RoleId equals re.RoleId.ToString()
            where re.EntryName == route.ToLower()
            select new VmOrePermission
            {
                RoleId = ue.RoleId,
                EntryId = re.EntryId.ToString(),
                IsAdd = re.IsAdd,
                IsView = re.IsView,
                IsUpdate = re.IsUpdate,
                IsDelete = re.IsDelete,
            }).FirstOrDefault();
        return permission;
    }
    public async Task<string> UpdateRolePermissions(PermissionViewModel vm)
    {
        var role = await roleManager.FindByIdAsync(vm.RoleId);
        if (role != null)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await roleManager.RemoveClaimAsync(role, claim);
            }
        }

        var selectedClaims = vm.RoleClaims.Where(a => a.Selected).ToList();
        foreach (var claim in selectedClaims)
        {
            await roleManager.AddPermissionClaim(role, claim.Value);
        }

        //return RedirectToAction("Index", new { roleId = model.RoleId });
        return vm.RoleId;
    }

    public string Locale(string resKey) => oreDb.Localizes
        .FirstOrDefault(x => x.ResKey.Trim().ToLower() == resKey.Trim().ToLower() && x.CultureId == CultureId)?.Value;
    
    public List<VmOrePermission> ListRolePermission(string roleId)
    {
        var permissions = (from re in oreDb.Permissions
            where re.RoleId.ToString() == roleId
            select new VmOrePermission
            {
                RoleId = roleId,
                EntryId = re.EntryId.ToString(),
                IsAdd = re.IsAdd,
                IsView = re.IsView,
                IsUpdate = re.IsUpdate,
                IsDelete = re.IsDelete,
            }).ToList();
        foreach (var item in permissions)
        {
            var lo = GetLocalizeEntry(item.EntryId);
            item.EntryName = lo.Title;
        }

        return permissions;
    }
    
    public async Task<List<IdentityRole>> AllRoles()
    {
        var roles = await roleManager.Roles.ToListAsync();
        return roles;
    }
    public async void AddRole(string roleName)
    {
        if (roleName != null)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
        }
    }
    public List<VmActivity> UserActivity(string id)
        {
            var query = (from a in oreDb.Activities
                where a.ActToId.ToString() == id
                join locale in oreDb.LocEntries on a.ActOnId equals locale.EntryId
                where locale.CultureId == CultureId
                join uBy in oreDb.Users on a.ActById.ToString() equals uBy.Id
                join uTo in oreDb.Users on a.ActById.ToString() equals uTo.Id
                join action in oreDb.LocAttributes on a.ActionId equals action.AttrId
                where action.CultureId == "en"
                select new VmActivity()
                {
                    Id = a.Id,
                    ActionId = a.ActionId,
                    ActById = a.ActById,
                    ActOnId = a.ActOnId,
                    ActToId = a.ActToId,
                    ActionName = action.Name,
                    ActByName = uBy.FullName,
                    ActToName = uTo.FullName,
                    ActOnName = locale.Title,
                    ActOnRoute = a.ActOnRoute,
                    DateAcion = a.ActionDate,
                    Message = CultureId == "en" ? action.Misc001 : action.Misc002,
                }).ToList();
            //if (query.Count == 0) return null;
            foreach (var q in query)
            {
                var replacements = new SortedList<string, string>
                {
                    { "{%ActById%}", q.ActById.ToString() },
                    { "{%ActByName%}", q.ActByName },
                    { "{%ActOnName%}", q.ActOnName },
                    { "{%ActOnRoute%}", q.ActOnRoute },
                    { "{%ActOnId%}", q.ActOnId.ToString() },
                    { "{%OtpDate%}", DateTime.Now.ToString("dd/MM/yyyy hh:mm") },
                };
                q.Message = replacements.Aggregate(q.Message,
                    (current, replacement) => current.Replace(replacement.Key, replacement.Value));
            }

            return query;
        }
    public async Task<ResponseDto> CheckAdmin(string id)
    {
        var user = userManager.FindByIdAsync(id).Result;
        var isInRole = user != null && await userManager.IsInRoleAsync(user, "Admin");
        _res.IsSuccess = isInRole;
        return _res;
    }
    public OreUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<OreUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(OreUser)}'. " +
                                                $"Ensure that '{nameof(OreUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
    public async Task<OreUser> UserByEmail(string email)
    {
        //UserStore<OreUser> UserStore = new(OreDb);
        var user = await userManager.FindByEmailAsync(email);
        return user;
    }
    public async Task BlockUser(string id)
    {
        //UserStore<OreUser> UserStore = new(OreDb);
        var user = await userManager.FindByIdAsync(id);
        //String hashedNewPassword = _userManager.PasswordHasher.HashPassword(user, "Xyz@123#678@Ayham");
        //await UserStore.SetPasswordHashAsync(user, hashedNewPassword);
        var isLockedOut = user is { LockoutEnabled: false };
        if (user != null) user.LockoutEnabled = isLockedOut;
        await oreDb.SaveChangesAsync();
    }
    public async Task<bool> ResetPassword(VmPass vm)
    {
        UserStore store = new(oreDb);
        var user = await userManager.FindByIdAsync(vm.Id);
        if (user == null) return false;
        var hashedNewPassword = userManager.PasswordHasher.HashPassword(user, vm.Password);
        await store.SetPasswordHashAsync(user, hashedNewPassword);
        await oreDb.SaveChangesAsync();
        return true;
    }
    public LocEntry GetLocalizeEntry(string entryId)
    {
        //Lx = TranslateString("Untitled");
        var lx = oreDb.LocEntries.FirstOrDefault(c => c.EntryId.ToString() == entryId && c.CultureId == CultureId);
        lx ??= oreDb.LocEntries.FirstOrDefault(
            c => c.EntryId.ToString() == entryId && c.CultureId == "en");
        return lx;
    }

    public async Task<OreUser> UserById(string id)
    {
        return await Task.FromResult(userManager.FindByIdAsync(id).Result);
    }
    public async Task<OreUser> CurrentUser()
    {
        return await Task.FromResult(userManager.FindByIdAsync(UserId).Result);
    }
    public async Task<bool> ChangePassword(VmPass vm)
    {
        UserStore<OreUser> store = new(oreDb);
        var input=vm.Email;
        var user=await userManager.Users.Where(c=>c.PhoneNumber==input || c.Email==input || c.Id==input).FirstOrDefaultAsync();
        if (user?.PasswordHash == null) return false;
        var hashedNewPassword = userManager.PasswordHasher.HashPassword(user, vm.Password);
        await store.SetPasswordHashAsync(user, hashedNewPassword);
        await oreDb.SaveChangesAsync();
        return true;
    }
}