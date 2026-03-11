using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Booking.Contracts.Search;
using Xunit;

namespace Booking.IntegrationTests;

public class SearchEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SearchEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSearch_NoQuery_ReturnsOkAndArray()
    {
        var response = await _client.GetAsync("/api/search");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<TripOptionDto>>();
        Assert.NotNull(results);
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task GetSearch_WithQueryParams_ReturnsOkAndFilteredResults()
    {
        var response = await _client.GetAsync("/api/search?origin=MAD&destination=BCN&maxPrice=100");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<List<TripOptionDto>>();
        Assert.NotNull(results);
        Assert.All(results, r =>
        {
            Assert.NotNull(r.Id);
            Assert.Equal("MAD", r.Origin);
            Assert.Equal("BCN", r.Destination);
            Assert.True(r.Price <= 100);
        });
    }
}
