using Microsoft.EntityFrameworkCore;
using UTB.Minute.Db;
using UTB.Minute.Db.Entities;
using UTB.Minute.Contracts;
using UTB.Minute.Contracts.Dtos;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<MinuteDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database Inicialization error.");
    }
}

//endpoints for Meals ---------------------------------------------------------------------------------------------------------------------------------------------------

// Get all meals
app.MapGet("/meals", async (MinuteDbContext db) =>
{
    var meals = await db.Meals.ToListAsync();
    var dtos = meals.Select(m => new MealResponseDto(m.Id, m.Name, m.Description, m.Price));
    return TypedResults.Ok(dtos);
});

// Create a new meal
app.MapPost("/meals", async (CreateMealRequest request, MinuteDbContext db) =>
{
    var meal = new Meal { Id = Guid.NewGuid(), Name = request.Name, Description = request.Description, Price = request.Price };
    db.Meals.Add(meal);
    await db.SaveChangesAsync();

    var response = new MealResponseDto(meal.Id, meal.Name, meal.Description, meal.Price);
    return TypedResults.Created($"/meals/{meal.Id}", response);
});

// Get single meal by ID
app.MapGet("/meals/{id}", async (Guid id, MinuteDbContext db) =>
{
    var meal = await db.Meals.FindAsync(id);
    return meal is not null
        ? TypedResults.Ok(new MealResponseDto(meal.Id, meal.Name, meal.Description, meal.Price))
        : Results.NotFound();
});

// Update meal details and Deactivation
app.MapPatch("/meals/{id}", async (Guid id, UpdateMealRequest request, MinuteDbContext db) =>
{
    var meal = await db.Meals.FindAsync(id);
    if (meal is null) return Results.NotFound();

    meal.Name = request.Name;
    meal.Description = request.Description;
    meal.Price = request.Price;
    meal.IsActive = request.IsActive;

    await db.SaveChangesAsync();
    return TypedResults.Ok(new MealResponseDto(meal.Id, meal.Name, meal.Description, meal.Price));
});

//endpoints for MenuItems ---------------------------------------------------------------------------------------------------------------------------------------------------

// Get todays menu
app.MapGet("/menu", async (MinuteDbContext db) =>
{
    var menuItems = await db.MenuItems.Include(mi => mi.Meal).ToListAsync();
    var dtos = menuItems.Select(mi => new MenuItemResponseDto(
        mi.Id, mi.MealId, mi.Meal?.Name ?? "Unknown", mi.Date, mi.AvailablePortions));
    return TypedResults.Ok(dtos);
});

// Add newMenuItem
app.MapPost("/menu", async (CreateMenuItemRequest request, MinuteDbContext db) =>
{
    var meal = await db.Meals.FindAsync(request.MealId);
    if (meal is null) return Results.BadRequest("Meal not found");

    var menuItem = new MenuItem
    {
        Id = Guid.NewGuid(),
        MealId = request.MealId,
        Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
        AvailablePortions = request.AvailablePortions
    };
    db.MenuItems.Add(menuItem);
    await db.SaveChangesAsync();

    var response = new MenuItemResponseDto(menuItem.Id, menuItem.MealId, meal.Name, menuItem.Date, menuItem.AvailablePortions);
    return TypedResults.Created($"/menu/{menuItem.Id}", response);
});

// Update menu item 
app.MapPatch("/menu/{id}", async (Guid id, UpdateMenuItemRequest request, MinuteDbContext db) =>
{
    var menuItem = await db.MenuItems.Include(mi => mi.Meal).FirstOrDefaultAsync(mi => mi.Id == id);
    if (menuItem is null) return Results.NotFound();

    menuItem.Date = request.Date;
    menuItem.AvailablePortions = request.AvailablePortions;

    await db.SaveChangesAsync();

    return TypedResults.Ok(new MenuItemResponseDto(
        menuItem.Id, menuItem.MealId, menuItem.Meal?.Name ?? "Unknown", menuItem.Date, menuItem.AvailablePortions));
});

// Delete menu item
app.MapDelete("/menu/{id}", async (Guid id, MinuteDbContext db) =>
{
    var menuItem = await db.MenuItems.FindAsync(id);
    if (menuItem is null) return Results.NotFound();

    db.MenuItems.Remove(menuItem);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

//endpoints for Orders ---------------------------------------------------------------------------------------------------------------------------------------------------

// Get all orders
app.MapGet("/orders", async (MinuteDbContext db) =>
{
    var orders = await db.Orders.Include(o => o.MenuItem).ThenInclude(mi => mi!.Meal).ToListAsync();
    var dtos = orders.Select(o => new OrderResponseDto(
        o.Id, o.OrderNumber, o.Status, o.CreatedAt, o.MenuItem?.Meal?.Name ?? "Unknown"));
    return TypedResults.Ok(dtos);
});

// Create a new order
app.MapPost("/orders", async (CreateOrderRequest request, MinuteDbContext db) =>
{
    var menuItem = await db.MenuItems.Include(mi => mi.Meal).FirstOrDefaultAsync(mi => mi.Id == request.MenuItemId);
    if (menuItem is null) return Results.BadRequest("Menu item not found");

    var order = new Order
    {
        Id = Guid.NewGuid(),
        MenuItemId = request.MenuItemId,
        Status = OrderStatus.Preparing,
        CreatedAt = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    var response = new OrderResponseDto(order.Id, order.OrderNumber, order.Status, order.CreatedAt, menuItem.Meal?.Name ?? "Unknown");
    return TypedResults.Created($"/orders/{order.Id}", response);
});

// Change order status
app.MapPatch("/orders/{id}/status", async (Guid id, OrderStatus newStatus, MinuteDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    if (order is null) return Results.NotFound();

    order.Status = newStatus;
    await db.SaveChangesAsync();
    return TypedResults.Ok();
});

app.Run();