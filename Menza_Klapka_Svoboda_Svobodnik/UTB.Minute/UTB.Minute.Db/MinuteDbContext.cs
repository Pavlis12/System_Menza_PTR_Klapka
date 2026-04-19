using Microsoft.EntityFrameworkCore;
using UTB.Minute.Db.Entities;

namespace UTB.Minute.Db;

public class MinuteDbContext : DbContext
{
    public MinuteDbContext(DbContextOptions<MinuteDbContext> options) : base(options)
    {

    }
    public DbSet<Meal> Meals { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
}