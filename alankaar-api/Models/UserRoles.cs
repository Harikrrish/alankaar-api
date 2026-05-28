namespace alankaar_api.Models;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Designer = "Designer";
    public const string SiteEngineer = "SiteEngineer";

    public static bool IsAdmin(string? role)
    {
        return string.Equals(role?.Trim(), Admin, StringComparison.Ordinal);
    }

    public static string Normalize(string? role)
    {
        return role?.Trim() switch
        {
            Admin => Admin,
            Designer => Designer,
            SiteEngineer => SiteEngineer,
            _ => Admin
        };
    }
}
