using Microsoft.EntityFrameworkCore;
using UTB.Minute.Db;
using UTB.Minute.Db.Entities;
using UTB.Minute.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddNpgsqlDbContext<MinuteDbContext>("minutedb");

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MinuteDbContext>();
    
}

app.MapPost("/reset", async (MinuteDbContext db) =>
{
    await db.Database.EnsureDeletedAsync();
    await db.Database.MigrateAsync();
    return Results.Ok("Database has been completely reset and migrated.");
});

app.MapPost("/seed", async (MinuteDbContext db) =>
{
    if (await db.Meals.AnyAsync())
    {
        return Results.Ok("Database already contains data. Seeding skipped.");
    }

    var burger = new Meal
    {
        Id = Guid.NewGuid(),
        Name = "Svickova",
        Description = "s knedlikem",
        Price = 155.00m,
        IsActive = true
    };
    db.Meals.Add(burger);

    var menuBurger = new MenuItem
    {
        Id = Guid.NewGuid(),
        MealId = burger.Id,
        Date = DateTime.UtcNow.Date,
        AvailablePortions = 20
    };
    db.MenuItems.Add(menuBurger);

    await db.SaveChangesAsync();

    var testOrder = new Order
    {
        Id = Guid.NewGuid(),
        MenuItemId = menuBurger.Id,
        OrderNumber = 1,
        Status = OrderStatus.Preparing,
        CreatedAt = DateTime.UtcNow
    };
    db.Orders.Add(testOrder);

    await db.SaveChangesAsync();

    return Results.Ok("Seeding completed successfully.");
});

app.Run();