using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Ore.Lib.ViewModels;
using Ore.Lib.ViewModels.Auth;

namespace Ore.Lib.Authorization.Helpers
{
	public static class ClaimsHelper
{
    public static void GetPermissions(this List<RoleClaimsViewModel> allPermissions, Type policy, string roleId)
    {
        var fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

        allPermissions.AddRange(fields.Select(fi => new RoleClaimsViewModel { Value = fi.GetValue(null)?.ToString(), Type = fi.GetValue(null)?.ToString()?.Split('.')[1] }));
    }

    public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }
    }
}
}
