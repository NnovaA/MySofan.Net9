namespace Site.lib.Services;

public partial class VmActivity
{
    public long Id { get; set; }
    public long ActionId { get; set; }
    public Guid ActById { get; set; }
    public Guid ActToId { get; set; }
    public Guid ActOnId { get; set; }
    public DateTime DateAcion { get; set; }
    public byte Status { get; set; }
    public string ActionName { get; set; }
    public string ActByName { get; set; }
    public string ActOnName { get; set; }
    public string ActOnRoute { get; set; }
    public string ActToName { get; set; }
    public string Message { get; set; }
}