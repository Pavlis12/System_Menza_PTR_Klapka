using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Aspire.Hosting.Testing;
using UTB.Minute.Contracts;
using UTB.Minute.Contracts.Dtos;

namespace UTB.Minute.WebApi.Tests;

public class OrderTests
{
    private const string ServiceName = "utb-minute-webapi";

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task OrderEndpoints()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Apphost.Program>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        var httpClient = app.CreateHttpClient(ServiceName);

        var mealRes = await httpClient.PostAsJsonAsync("/meals", new CreateMealRequest("Test jidlo", "Popis", 200m));
        var meal = await mealRes.Content.ReadFromJsonAsync<MealResponseDto>(_jsonOptions);

        var menuRes = await httpClient.PostAsJsonAsync("/menu", new CreateMenuItemRequest(meal!.Id, DateTime.UtcNow, 10));
        var menuItem = await menuRes.Content.ReadFromJsonAsync<MenuItemResponseDto>(_jsonOptions);

        // Test: Create a new order (POST)
        var createOrderReq = new CreateOrderRequest(menuItem!.Id);
        var postOrderRes = await httpClient.PostAsJsonAsync("/orders", createOrderReq);

        Assert.Equal(HttpStatusCode.Created, postOrderRes.StatusCode);
        var createdOrder = await postOrderRes.Content.ReadFromJsonAsync<OrderResponseDto>(_jsonOptions);

        // Test: Update order status (PATCH)
        var newStatus = OrderStatus.ReadyToPickUp;
        var patchRes = await httpClient.PatchAsync($"/orders/{createdOrder!.Id}/status?newStatus={newStatus}", null);
        Assert.Equal(HttpStatusCode.OK, patchRes.StatusCode);

        // Test: Verify order status in list (GET)
        var getOrdersRes = await httpClient.GetAsync("/orders");
        var orders = await getOrdersRes.Content.ReadFromJsonAsync<List<OrderResponseDto>>(_jsonOptions);
        var updatedOrder = orders!.First(o => o.Id == createdOrder.Id);

        Assert.Equal(OrderStatus.ReadyToPickUp, updatedOrder.Status);
    }
}