using Microsoft.EntityFrameworkCore;
using UTB.Minute.Db;
using UTB.Minute.Db.Entities;

var builder = WebApplication.CreateBuilder(args);

// 1. Propojíme projekt s SQL Serverem přes Aspire
builder.AddSqlServerDbContext<MinuteDbContext>("minutedb");

var app = builder.Build();

// 2. HTTP příkaz pro kompletní smazání a znovuvytvoření databáze
app.MapPost("/db/reset", async (MinuteDbContext db) =>
{
    // Smaže DB pokud existuje
    await db.Database.EnsureDeletedAsync();

    // Vytvoří DB znovu podle složky Entities v projektu .Db
    await db.Database.EnsureCreatedAsync();

    // Vložíme testovací data (Seed)
    db.Foods.Add(new Food { Description = "Svíčková na smetaně", Price = 165 });
    db.Foods.Add(new Food { Description = "Guláš s knedlíkem", Price = 145 });

    await db.SaveChangesAsync();

    return Results.Ok("Databáze resetována a naplněna daty!");
});

app.Run();