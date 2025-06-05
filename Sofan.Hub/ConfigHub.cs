namespace Sofan.Hub;

public abstract class ConfigHub
{
    public static bool IsLocal { get; set; }
    public static string GuidPAttern => "00000000-1111-1111-1001";
    public static Guid AdminId { get; } = Guid.Parse("00100000-0010-0010-0010-001000000000");
    public static string CookieName { get; set; } = "SofanCookie";
    public static string CultureCookie { get; set; } = "SofanCulture";
    public static string PublicSite { get; set; } = "Sofan.linekw.xyz";
    public static string TokenName { get; set; } = "SofanToken";
    public static int HttpPort { get; set; } = 7600;
    public static int HttpsPort { get; set; } = 7601;
    public static string Site { get; } = IsLocal ? $"https://localhost/{HttpsPort}" : $"https://{PublicSite}";
    public static string SiteName { get; } = "Sofan Steel";
    public static string UserName { get; set; } = IsLocal ? "Sofan" : "SofanUser";
    public static string Password { get; set; } = "Ali@12356780";
    private static string DataBase { get; set; } = "MySofDb.V01";
    private static string ServerName { get; set; } = "localhost";
    public static string Cs { get;} = $"server={ServerName};user={UserName};password={Password};database={DataBase}";
    public static string DockerCs { get; } = $"server=mdb;port=3306;database={DataBase};user={UserName};password={Password};";
    public static int ExpirationHours { get; } = 720;
    public static string MailBeeLicense { get; } = "MN120-90E01F65B3CE0988EA4FCB66C3CA-33AB";
    public static string SmtpServer { get; } = "orasys.org";
    public static string EmailAccount { get; } = "alibusiness460@gmail.com";
    public static string EmailPassword { get; } = "Bai@216457!aT";
    public static string NotificationEmail { get; } = "alibusiness460@gmail.com";
    public static int EmailPort { get; } = 587;
    public const string Key = "ABD77axe7fhHAsAF51xa0zdu6c47dxbjlka";
}