namespace alankaar_api.Services;

public static class AuthText
{
    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }
}
