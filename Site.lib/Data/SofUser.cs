using Microsoft.AspNetCore.Identity;

namespace Site.Lib.Data;

public class SofUser: IdentityUser
{
    public string CultureId { get; set; }
    public string FullName { get; set; }
    public string JobTitle { get; set; }
    public string Token { get; set; }
    public string Avatar { get; set; }
    public string Password { get; set; }
    public bool IsVerified { get; set; }
}