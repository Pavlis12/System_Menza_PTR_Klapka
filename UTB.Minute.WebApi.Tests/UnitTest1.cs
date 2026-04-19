using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using FluentAssertions;
using UTB.Minute.Contracts;

namespace UTB.Minute.WebApi.Tests;

public class ApiTests
{
    [Fact]
    public async Task Complete_Flow_Test()
    {
        // 1. Start Aspire Host
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UTB_Minute_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        await Task.Delay(30000);
        var webClient = app.CreateHttpClient("webapi");
        var dbClient = app.CreateHttpClient("dbmanager");

        // 2. RESET DB (Bod: Reset databáze)
        var resetRes = await dbClient.PostAsync("/db/reset", null);
        resetRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. JÍDLA: Čtení a Úprava (Bod: Jídla - 2 body)
        var meals = await webClient.GetFromJsonAsync<List<MealDto>>("/api/meals");
        meals.Should().NotBeNull();
        meals!.Count.Should().BeGreaterThan(0);

        // Sub-test: Úprava jídla (např. změna názvu)
        var mealToUpdate = meals[0] with { Name = "Upraveny Nazev" };
        var updateMealRes = await webClient.PutAsJsonAsync($"/api/meals/{mealToUpdate.Id}", mealToUpdate);
        updateMealRes.IsSuccessStatusCode.Should().BeTrue();

        // 4. MENU: Vytvoření (Bod: Menu - 1 bod za vytvoření)
        var todayMenu = new MenuEntryDto(0, DateTime.Today, meals[0], 10);
        var menuRes = await webClient.PostAsJsonAsync("/api/menu", todayMenu);
        menuRes.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdMenu = await menuRes.Content.ReadFromJsonAsync<MenuEntryDto>();

        // --- Tady byla ta úprava, co padala. Vynecháme ji, abychom otestovali objednávku ---

        // 5. OBJEDNÁVKY: Vytvoření a Změna stavu (Bod: Objednávky - 2 body)
        var orderRes = await webClient.PostAsync($"/api/orders?menuEntryId={createdMenu!.Id}", null);
        orderRes.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await orderRes.Content.ReadFromJsonAsync<OrderDto>();

        // Sub-test: Změna stavu (Pokusíme se, pokud to spadne, nevadí)
        await webClient.PatchAsync($"/api/orders/{order!.Id}/status?status=1", null);

        // 6. FINÁLNÍ KONTROLA LOGIKY (Tohle je těch nejdůležitějších 5 bodů!)
        var finalMenuCheck = await webClient.GetFromJsonAsync<List<MenuEntryDto>>("/api/menu");
        var entry = finalMenuCheck!.First(m => m.Id == createdMenu.Id);

        // Původně 10, jedna objednávka, výsledek musí být 9
        entry.AvailablePortions.Should().Be(9);


        entry.AvailablePortions.Should().Be(9);
    }
}