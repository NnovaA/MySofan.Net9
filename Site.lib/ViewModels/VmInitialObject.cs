namespace Site.lib.ViewModels
{
    public class VmInitialObject
    {
        public Guid ObjectId { get; set; }
        public long TypeId { get; set; }
        public long ChildTypeId { get; set; }
        public string EnName { get; set; }
        public string ArName { get; set; }
        public string DepRoute { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public bool IsInNavBar { get; set; }
        public bool IsInFooter { get; set; }
    }

    public class VmInitialConstant
    {
        public Guid ConId { get; set; }
        public long TypeId { get; set; }
        public string ConName { get; set; }
    }

    public class VmInitialProperty
    {
        public long PropId { get; set; }
        public long ParentId { get; set; }
        public string PropEnName { get; set; }
        public string PropArName { get; set; }
        public string Misc001 { get; set; }
        public string Misc002 { get; set; }
        public string Misc003 { get; set; }
        public string Misc004 { get; set; }
        public int Order { get; set; }
    }

    public class VmInitialActivity
    {
        public long PropId { get; set; }
        public long ParentId { get; set; }
        public string ActName { get; set; }
        public string EnMessage { get; set; }
        public string ArMessage { get; set; }
        public string Icon { get; set; }
    }

    public class VmInitialEntry
    {
        public Guid EntryId { get; set; }
        public long AttrId { get; set; }
        public long ChildTypeId { get; set; } = -1;
        public string RefName { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string MenuIcon { get; set; }
        public bool IsActive { get; set; }
        public bool IsListed { get; set; } = false;
        public int Order { get; set; }
    }
}