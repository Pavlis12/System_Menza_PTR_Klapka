using Microsoft.EntityFrameworkCore;
using UTB.Minute.Contracts;
using UTB.Minute.Db;
using UTB.Minute.Db.Entities;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES ---
builder.AddSqlServerDbContext<MinuteDbContext>("minutedb");
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- INFRASTRUCTURE ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MinuteDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// --- MEALS SECTION (Admin) ---

// Get all meals
app.MapGet("/api/meals", async (MinuteDbContext db) =>
{
    var meals = await db.Meals
        .Select(m => new MealDto(m.Id, m.Description, m.Price, m.IsActive))
        .ToListAsync();

    return TypedResults.Ok(meals);
});

// Create new meal
app.MapPost("/api/meals", async (MealDto mealDto, MinuteDbContext db) =>
{
    var meal = new Meal
    {
        Description = mealDto.Description,
        Price = mealDto.Price,
        IsActive = mealDto.IsActive
    };

    db.Meals.Add(meal);
    await db.SaveChangesAsync();

    var resultDto = new MealDto(meal.Id, meal.Description, meal.Price, meal.IsActive);
    return TypedResults.Created($"/api/meals/{meal.Id}", resultDto);
});

// Update meal (Description, Price, IsActive)
app.MapPut("/api/meals/{id}", async (int id, MealDto mealDto, MinuteDbContext db) =>
{
    var meal = await db.Meals.FindAsync(id);
    if (meal == null) return Results.NotFound();

    meal.Description = mealDto.Description;
    meal.Price = mealDto.Price;
    meal.IsActive = mealDto.IsActive; // Zde se řeší i deaktivace

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
});

// --- MENU SECTION (Admin/Student) ---

// Get current menu
app.MapGet("/api/menu", async (MinuteDbContext db) =>
{
    var menu = await db.MenuEntries
        .Include(m => m.Meal)
        .Select(m => new MenuEntryDto(
            m.Id,
            m.Date,
            new MealDto(m.Meal.Id, m.Meal.Description, m.Meal.Price, m.Meal.IsActive),
            m.AvailablePortions))
        .ToListAsync();

    return TypedResults.Ok(menu);
});

// Create menu entry
app.MapPost("/api/menu", async (MenuEntryDto dto, MinuteDbContext db) =>
{
    var entry = new MenuEntry
    {
        MealId = dto.Food.Id,
        Date = dto.Date,
        AvailablePortions = dto.AvailablePortions
    };

    db.MenuEntries.Add(entry);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/api/menu/{entry.Id}", dto with { Id = entry.Id });
});

// Delete menu entry
app.MapDelete("/api/menu/{id}", async (int id, MinuteDbContext db) =>
{
    var entry = await db.MenuEntries.FindAsync(id);
    if (entry == null) return Results.NotFound();

    db.MenuEntries.Remove(entry);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
});

// --- ORDERS SECTION (Student/Canteen) ---

// Create order (Student) - Automaticaly reduces portions
app.MapPost("/api/orders", async (int menuEntryId, MinuteDbContext db) =>
{
    var entry = await db.MenuEntries.FindAsync(menuEntryId);

    if (entry == null || entry.AvailablePortions <= 0)
        return Results.BadRequest("Jídlo je vyprodané nebo neexistuje.");

    entry.AvailablePortions--; // Snížení počtu porcí (Požadavek zadání)

    var order = new Order
    {
        MenuEntryId = menuEntryId,
        Status = OrderStatus.Preparing,
        CreatedAt = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/api/orders/{order.Id}",
        new OrderDto(order.Id, order.MenuEntryId, "Order Received", order.Status, order.CreatedAt));
});

// Get all orders for Canteen
app.MapGet("/api/orders", async (MinuteDbContext db) =>
{
    var orders = await db.Orders
        .Include(o => o.MenuEntry)
        .ThenInclude(m => m.Meal)
        .Select(o => new OrderDto(o.Id, o.MenuEntryId, o.MenuEntry.Meal.Description, o.Status, o.CreatedAt))
        .ToListAsync();

    return TypedResults.Ok(orders);
});

// Update order status (Canteen)
app.MapPatch("/api/orders/{id}/status", async (int id, OrderStatus newStatus, MinuteDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    if (order == null) return Results.NotFound();

    order.Status = newStatus;
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.MapDefaultEndpoints();
app.Run();

// Pro testy
public partial class Program { };