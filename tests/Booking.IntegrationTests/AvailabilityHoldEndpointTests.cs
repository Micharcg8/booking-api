using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Booking.Contracts.Availability;
using Xunit;

namespace Booking.IntegrationTests;

public class AvailabilityHoldEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AvailabilityHoldEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAvailability_WithTripId_ReturnsOkAndAvailable()
    {
        var response = await _client.GetAsync("/api/availability?tripId=trip-1");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AvailabilityCheckResponse>();
        Assert.NotNull(result);
        Assert.Equal("trip-1", result.TripId);
        Assert.True(result.Available);
    }

    [Fact]
    public async Task CreateHold_ReturnsCreatedAndHoldDetails()
    {
        var request = new CreateHoldRequest { TripId = "trip-1", Passengers = 2 };
        var response = await _client.PostAsJsonAsync("/api/holds", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var hold = await response.Content.ReadFromJsonAsync<CreateHoldResponse>();
        Assert.NotNull(hold);
        Assert.NotNull(hold.HoldId);
        Assert.Equal("trip-1", hold.TripId);
        Assert.True(hold.ExpiresAt > DateTime.UtcNow);
        Assert.Equal(HoldStatus.Active, hold.Status);
    }

    [Fact]
    public async Task GetHold_AfterCreate_ReturnsHoldWithActiveStatus()
    {
        var request = new CreateHoldRequest { TripId = "trip-2" };
        var createResponse = await _client.PostAsJsonAsync("/api/holds", request);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CreateHoldResponse>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/holds/{created.HoldId}");
        getResponse.EnsureSuccessStatusCode();
        var hold = await getResponse.Content.ReadFromJsonAsync<HoldDto>();
        Assert.NotNull(hold);
        Assert.Equal(created.HoldId, hold.HoldId);
        Assert.Equal(HoldStatus.Active, hold.Status);
    }

    [Fact]
    public async Task GetHold_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/holds/unknown-hold-id");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
