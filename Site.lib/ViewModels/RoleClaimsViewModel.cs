using Ore.Lib.ViewModels.Auth;

namespace Ore.Lib.ViewModels
{
	public class RoleClaimsViewModel
	{
		public string Type { get; set; }
		public string Value { get; set; }
		public bool Selected { get; set; }
		public string Permission { get; set; }
		public List<VmClaimValue> Values { get; set; }
	}
}
