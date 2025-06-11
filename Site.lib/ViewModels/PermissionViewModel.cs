using Ore.Lib.ViewModels.Auth;

namespace Ore.Lib.ViewModels
{
	public abstract class PermissionViewModel
	{
		public string RoleId { get; set; }
		public IList<RoleClaimsViewModel> RoleClaims { get; set; }
	}
	public class PostPermissionViewModel
	{
		public string RoleId { get; set; }
		public List<PostPermissionItem> Items { get; set; }
	}
	public abstract class PostPermissionItem
	{
		public string Id { get; set; }
		public string Data { get; set; }
	}
}
