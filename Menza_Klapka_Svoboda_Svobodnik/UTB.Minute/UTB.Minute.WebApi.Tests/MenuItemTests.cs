using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Aspire.Hosting.Testing;
using UTB.Minute.Contracts.Dtos;

namespace UTB.Minute.WebApi.Tests;

public class MenuItemTests
{
    private const string ServiceName = "utb-minute-webapi";

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task MenuEndpoints()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Apphost.Program>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        var httpClient = app.CreateHttpClient(ServiceName);

        // Test: Create a meal for the menu item
        var mealRes = await httpClient.PostAsJsonAsync("/meals", new CreateMealRequest("Menu Jídlo", "Popis", 100m));
        var meal = await mealRes.Content.ReadFromJsonAsync<MealResponseDto>(_jsonOptions);

        // Test: Create a new menu item (POST)
        var createMenuReq = new CreateMenuItemRequest(meal!.Id, DateTime.UtcNow, 25);
        var postRes = await httpClient.PostAsJsonAsync("/menu", createMenuReq);
        Assert.Equal(HttpStatusCode.Created, postRes.StatusCode);

        var createdItem = await postRes.Content.ReadFromJsonAsync<MenuItemResponseDto>(_jsonOptions);

        // Test: Get today's menu (GET)
        var getRes = await httpClient.GetAsync("/menu");
        var menuList = await getRes.Content.ReadFromJsonAsync<List<MenuItemResponseDto>>(_jsonOptions);
        Assert.Contains(menuList!, mi => mi.Id == createdItem!.Id);

        // Test: Update menu item details (PATCH)
        var updateReq = new UpdateMenuItemRequest(DateTime.UtcNow, 40);
        var patchRes = await httpClient.PatchAsJsonAsync($"/menu/{createdItem!.Id}", updateReq);
        Assert.Equal(HttpStatusCode.OK, patchRes.StatusCode);

        // Test: Delete menu item (DELETE)
        var deleteRes = await httpClient.DeleteAsync($"/menu/{createdItem.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);
    }
}