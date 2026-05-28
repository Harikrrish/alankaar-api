using alankaar_api.Models;
using Microsoft.EntityFrameworkCore;

namespace alankaar_api.Data;

public class AlankaarDbContext(DbContextOptions<AlankaarDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);

            entity.Property(user => user.FullName)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(user => user.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(user => user.Role)
                .IsRequired()
                .HasMaxLength(40);

            entity.Property(user => user.CreatedAtUtc)
                .IsRequired();

            entity.HasIndex(user => user.NormalizedEmail)
                .IsUnique();
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(client => client.Id);

            entity.Property(client => client.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(client => client.FlatNumber)
                .IsRequired()
                .HasMaxLength(40);

            entity.Property(client => client.Location)
                .IsRequired()
                .HasMaxLength(160);

            entity.Property(client => client.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(client => client.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(client => client.HandoverDate)
                .IsRequired();

            entity.Property(client => client.CreatedAtUtc)
                .IsRequired();

            entity.HasIndex(client => client.Status);
            entity.HasIndex(client => client.HandoverDate);
        });
    }
}
