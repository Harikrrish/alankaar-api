namespace alankaar_api.Services;

public interface IPasswordHashService
{
    string Hash(string password);

    bool Verify(string password, string storedHash);
}
