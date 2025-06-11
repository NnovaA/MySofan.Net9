namespace Site.lib.Models;

public class Activity
{
    public int Id { get; set; }
    public long ActionId { get; set; }
    public Guid ActById { get; set; }
    public Guid ActOnId { get; set; }
    public Guid ActToId { get; set; }
    public DateTime ActionDate { get; set; }
    public byte Status { get; set; }
    public string ActOnRoute { get; set; }
}