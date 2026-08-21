using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Barcode> Barcodes => Set<Barcode>();
    public DbSet<User> Users => Set<User>();
    public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уникальность штрихкода — на один код только один продукт
        modelBuilder.Entity<Barcode>()
            .HasIndex(b => b.Code)
            .IsUnique();

        // Индекс по имени продукта — для быстрого поиска
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name);

        base.OnModelCreating(modelBuilder);
    }
}