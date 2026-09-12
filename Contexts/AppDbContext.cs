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

    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<TransactionAttachment> TransactionAttachments => Set<TransactionAttachment>();

    public DbSet<Transfer> Transfers => Set<Transfer>();

    public DbSet<Budget> Budgets => Set<Budget>();

    public DbSet<BudgetAlert> BudgetAlerts => Set<BudgetAlert>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<RecurringTransaction> RecurringTransactions => Set<RecurringTransaction>();

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
            entity.Property(u => u.Currency).HasMaxLength(10).IsRequired();
            entity.Property(u => u.Language).HasMaxLength(10).IsRequired();
            entity.Property(u => u.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FinancialAccount>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            entity.Property(x => x.InitialBalance).HasPrecision(18, 2);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.IsActive);

            entity.HasOne(x => x.User)
                .WithMany(x => x.FinancialAccounts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(x => x.Icon).HasMaxLength(100);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Type);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Location).HasMaxLength(255);

            entity.HasIndex(x => x.AccountId);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.TransactionDate);
            entity.HasIndex(x => x.Type);

            entity.HasOne(x => x.Account)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TransactionAttachment>(entity =>
        {
            entity.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            entity.Property(x => x.FileUrl).HasMaxLength(500).IsRequired();
            entity.Property(x => x.FileType).HasMaxLength(50);

            entity.HasIndex(x => x.TransactionId);

            entity.HasOne(x => x.Transaction)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.FromAccountId);
            entity.HasIndex(x => x.ToAccountId);
            entity.HasIndex(x => x.TransferDate);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Transfers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.FromAccount)
                .WithMany(x => x.FromTransfers)
                .HasForeignKey(x => x.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ToAccount)
                .WithMany(x => x.ToTransfers)
                .HasForeignKey(x => x.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.AlertThreshold).HasPrecision(5, 2);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.StartDate);
            entity.HasIndex(x => x.EndDate);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<BudgetAlert>(entity =>
        {
            entity.Property(x => x.Threshold).HasPrecision(5, 2);
            entity.Property(x => x.CurrentPercentage).HasPrecision(5, 2);
            entity.Property(x => x.Message).HasMaxLength(500);

            entity.HasIndex(x => x.BudgetId);
            entity.HasIndex(x => x.IsRead);

            entity.HasOne(x => x.Budget)
                .WithMany(x => x.Alerts)
                .HasForeignKey(x => x.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.DueDate);
            entity.HasIndex(x => x.Status);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecurringTransaction>(entity =>
        {
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Frequency)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(x => x.AccountId);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.NextExecutionDate);
            entity.HasIndex(x => x.IsActive);

            entity.HasOne(x => x.Account)
                .WithMany(x => x.RecurringTransactions)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.RecurringTransactions)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
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
