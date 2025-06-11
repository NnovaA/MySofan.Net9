namespace Site.lib.ViewModels
{
    public class VmPermission
    {
        public string RoleId { get; set; }
        public IList<RoleClaimsViewModel> RoleClaims { get; set; }
    }
    public class VmOrePermission
    {
        public string RoleId { get; set; }
        public string EntryId { get; set; }
        public string EntryName { get; set; }
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}