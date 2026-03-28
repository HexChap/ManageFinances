using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Expense>()
            .Property(e => e.Value)
            .HasColumnType("numeric(10,2)");

        modelBuilder.Entity<Income>()
            .Property(i => i.Value)
            .HasColumnType("numeric(10,2)");

        // Implicit many-to-many: EF Core creates ExpenseTag join table
        modelBuilder.Entity<Expense>()
            .HasMany(e => e.Tags)
            .WithMany(t => t.Expenses);
    }
}
