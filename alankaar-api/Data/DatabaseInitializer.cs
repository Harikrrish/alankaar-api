using alankaar_api.Models;
using alankaar_api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace alankaar_api.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AlankaarDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await context.Database.EnsureCreatedAsync();
        await EnsureRoleColumnAsync(context);
        await EnsureClientsTableAsync(context);

        foreach (var seedUser in GetSeedUsers(configuration))
        {
            var normalizedEmail = AuthText.NormalizeEmail(seedUser.Email);
            var existingUser = await context.Users
                .SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail);

            if (existingUser is not null)
            {
                existingUser.Role = seedUser.Role;
                continue;
            }

            context.Users.Add(new User
            {
                FullName = seedUser.FullName,
                Email = seedUser.Email,
                NormalizedEmail = normalizedEmail,
                PasswordHash = passwordHasher.Hash(seedUser.Password),
                Role = seedUser.Role,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureRoleColumnAsync(AlankaarDbContext context)
    {
        const string sql = """
            IF COL_LENGTH('Users', 'Role') IS NULL
            BEGIN
                ALTER TABLE [Users]
                ADD [Role] nvarchar(40) NOT NULL
                CONSTRAINT [DF_Users_Role] DEFAULT N'Admin'
            END
            """;

        await context.Database.ExecuteSqlRawAsync(sql);
    }

    private static async Task EnsureClientsTableAsync(AlankaarDbContext context)
    {
        const string sql = """
            IF OBJECT_ID(N'[Clients]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Clients] (
                    [Id] int NOT NULL IDENTITY,
                    [Name] nvarchar(120) NOT NULL,
                    [FlatNumber] nvarchar(40) NOT NULL,
                    [Location] nvarchar(160) NOT NULL,
                    [PhoneNumber] nvarchar(20) NOT NULL,
                    [Status] nvarchar(20) NOT NULL,
                    [HandoverDate] date NOT NULL,
                    [CreatedAtUtc] datetime2 NOT NULL,
                    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id])
                );

                CREATE INDEX [IX_Clients_Status] ON [Clients] ([Status]);
                CREATE INDEX [IX_Clients_HandoverDate] ON [Clients] ([HandoverDate]);
            END
            """;

        await context.Database.ExecuteSqlRawAsync(sql);
    }

    private static IEnumerable<SeedUser> GetSeedUsers(IConfiguration configuration)
    {
        var configuredUsers = configuration.GetSection("SeedUsers")
            .GetChildren()
            .Select(section => new SeedUser(
                section["FullName"] ?? "Alankaar User",
                section["Email"] ?? string.Empty,
                section["Password"] ?? "Password@123",
                UserRoles.Normalize(section["Role"])))
            .Where(user => !string.IsNullOrWhiteSpace(user.Email));

        return configuredUsers.Any()
            ? configuredUsers
            : [
                new SeedUser("Alankaar Admin", "admin@alankaar.local", "Admin@123", UserRoles.Admin),
                new SeedUser("Design Team", "designer@alankaar.local", "Designer@123", UserRoles.Designer),
                new SeedUser("Site Engineer", "siteengineer@alankaar.local", "Site@123", UserRoles.SiteEngineer)
            ];
    }

    private sealed record SeedUser(string FullName, string Email, string Password, string Role);
}
