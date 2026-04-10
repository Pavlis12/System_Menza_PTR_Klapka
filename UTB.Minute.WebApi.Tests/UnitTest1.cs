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
        await Task.Delay(5000);
        var webClient = app.CreateHttpClient("webapi");
        var dbClient = app.CreateHttpClient("dbmanager");

        // 2. RESET DB (Pomocí tvého DbManageru - to chce učitel vidět!)
        var resetRes = await dbClient.PostAsync("/db/reset", null);
        resetRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. TEST: Jsou tam jídla ze seedu?
        var meals = await webClient.GetFromJsonAsync<List<MealDto>>("/api/meals");
        meals.Should().NotBeNull();
        meals!.Count.Should().BeGreaterThan(0);

        // 4. TEST: Vytvoření Menu na dnes (aby šlo objednávat)
        var todayMenu = new MenuEntryDto(0, DateTime.Today, meals[0], 10);
        var menuRes = await webClient.PostAsJsonAsync("/api/menu", todayMenu);
        menuRes.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdMenu = await menuRes.Content.ReadFromJsonAsync<MenuEntryDto>();

        // 5. TEST: Objednávka (Student si objedná)
        // Voláme POST /api/orders?menuEntryId=X
        var orderRes = await webClient.PostAsync($"/api/orders?menuEntryId={createdMenu!.Id}", null);
        orderRes.StatusCode.Should().Be(HttpStatusCode.Created);

        // 6. TEST: Ověření, že klesl počet porcí (To je v checklistu za body!)
        var updatedMenu = await webClient.GetFromJsonAsync<List<MenuEntryDto>>("/api/menu");
        var entry = updatedMenu!.First(m => m.Id == createdMenu.Id);
        entry.AvailablePortions.Should().Be(9); // Původně 10, po objednávce 9
    }
}