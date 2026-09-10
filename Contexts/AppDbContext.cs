using Microsoft.EntityFrameworkCore;
using SFM_BE.Entities;
using SFM_BE.Enums;

namespace SFM_BE.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
            entity.Property(u => u.DisplayName).HasMaxLength(255).IsRequired();
            entity.Property(u => u.AvatarUrl).HasMaxLength(1000);

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(r => r.Id).ValueGeneratedNever();
            entity.Property(r => r.Name).HasConversion<int>();
            entity.Property(r => r.Description).HasMaxLength(255).IsRequired();

            entity.HasData(
                new Role { Id = 1, Name = UserRole.User, Description = "Regular User" },
                new Role { Id = 2, Name = UserRole.Admin, Description = "Administrator" });
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(x => x.Token).HasMaxLength(512).IsRequired();

            entity.HasIndex(x => x.Token).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.Property(x => x.ProviderUserId).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(255);

            entity.HasIndex(x => new { x.Provider, x.ProviderUserId }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.ExternalLogins)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
