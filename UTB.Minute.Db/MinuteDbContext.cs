using Microsoft.EntityFrameworkCore;
using UTB.Minute.Db.Entities;

namespace UTB.Minute.Db;

public class MinuteDbContext : DbContext
{
    // Konstruktor, který Aspire použije k předání Connection Stringu
    public MinuteDbContext(DbContextOptions<MinuteDbContext> options) : base(options)
    {
    }

    // Definice tabulek
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<MenuEntry> MenuEntries => Set<MenuEntry>();

    // Zde můžeme nastavit pokročilejší pravidla (např. přesnost ceny)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Food>()
            .Property(f => f.Price)
            .HasPrecision(18, 2); // 18 číslic celkem, 2 desetinná místa
    }
}
