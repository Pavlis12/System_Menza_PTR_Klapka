using Aspire.Hosting;
using Aspire.Hosting.Testing;
using System.Net;
using System.Net.Http.Json;
using UTB.Minute.Contracts.Dtos;

namespace UTB.Minute.WebApi.Tests;

public class MealTests
{
    private const string ServiceName = "utb-minute-webapi";

    private async Task<(IDistributedApplicationTestingBuilder, DistributedApplication)> StartAppAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Apphost.Program>();
        var app = await appHost.BuildAsync();
        await app.StartAsync();
        return (appHost, app);
    }

    [Fact]
    public async Task MealEndpoints()
    {
        var (appHost, app) = await StartAppAsync();
        await using var _ = app;
        var httpClient = app.CreateHttpClient(ServiceName);

        // Test: Create a new meal (POST)
        var createRequest = new CreateMealRequest("Smažák", "s brambory a tatarkou", 150m);
        var postResponse = await httpClient.PostAsJsonAsync("/meals", createRequest);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var createdMeal = await postResponse.Content.ReadFromJsonAsync<MealResponseDto>();
        Assert.NotNull(createdMeal);
        Assert.Equal("Smažák", createdMeal.Name);

        // Test: Get meal details by ID (GET)
        var getByIdResponse = await httpClient.GetAsync($"/meals/{createdMeal.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);
        var detailedMeal = await getByIdResponse.Content.ReadFromJsonAsync<MealResponseDto>();
        Assert.Equal(createdMeal.Id, detailedMeal!.Id);

        // Test: Update meal details and deactivate (PATCH)
        var updateRequest = new UpdateMealRequest("Smažený sýr", "S domácí tatarkou", 180m, false);
        var patchResponse = await httpClient.PatchAsJsonAsync($"/meals/{createdMeal.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);
        var patchedMeal = await patchResponse.Content.ReadFromJsonAsync<MealResponseDto>();
        Assert.Equal("Smažený sýr", patchedMeal!.Name);
        Assert.Equal(180m, patchedMeal.Price);

        // Test: Get list of all meals (GET)
        var listResponse = await httpClient.GetAsync("/meals");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var meals = await listResponse.Content.ReadFromJsonAsync<IEnumerable<MealResponseDto>>();

        Assert.NotNull(meals);
        Assert.Contains(meals!, m => m.Id == createdMeal.Id && m.Name == "Smažený sýr");

        // Test: Return NotFound for non-existent meal ID (GET)
        var notFoundResponse = await httpClient.GetAsync($"/meals/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
    }
}