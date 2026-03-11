using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Booking.Contracts.Availability;
using Booking.Contracts.Booking;
using Booking.Contracts.Payments;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Booking.IntegrationTests;

public class BookingPaymentWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
    };

    public BookingPaymentWorkflowTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SuccessfulPaymentFlow_ConfirmsBooking_AndConvertsHold()
    {
        // Arrange: create a hold
        var holdResponse = await _client.PostAsJsonAsync("/api/holds", new { TripId = "trip-1", Passengers = 1 });
        holdResponse.EnsureSuccessStatusCode();
        var createdHold = await holdResponse.Content.ReadFromJsonAsync<CreateHoldResponse>(JsonOptions);
        Assert.NotNull(createdHold);

        // Act: create booking in pending state
        var createBookingRequest = new CreateBookingRequest
        {
            TripId = createdHold!.TripId,
            HoldId = createdHold.HoldId,
            CustomerName = "Test User",
            CustomerEmail = "test@example.com"
        };

        var bookingResponse = await _client.PostAsJsonAsync("/api/bookings", createBookingRequest);
        bookingResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, bookingResponse.StatusCode);
        var createdBooking = await bookingResponse.Content.ReadFromJsonAsync<CreateBookingResponse>(JsonOptions);
        Assert.NotNull(createdBooking);
        Assert.Equal(BookingStatus.Pending, createdBooking!.Status);

        // Initiate payment
        var paymentRequest = new InitiatePaymentRequest
        {
            BookingId = createdBooking.BookingId,
            Amount = 100m
        };
        var initiateResponse = await _client.PostAsJsonAsync("/api/payments", paymentRequest);
        initiateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, initiateResponse.StatusCode);
        var paymentJson = await initiateResponse.Content.ReadAsStringAsync();
        using var paymentDoc = JsonDocument.Parse(paymentJson);
        var paymentId = paymentDoc.RootElement.GetProperty("paymentId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(paymentId));

        // Simulate callback with success
        var callbackResponse = await _client.PostAsync($"/api/payments/{paymentId}/callback?status=Succeeded", null);
        Assert.Equal(HttpStatusCode.NoContent, callbackResponse.StatusCode);

        // Assert: booking is confirmed and hold converted
        var bookingGet = await _client.GetAsync($"/api/bookings/{createdBooking.BookingId}");
        bookingGet.EnsureSuccessStatusCode();
        var bookingJson = await bookingGet.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"confirmed\"", bookingJson);
        Assert.Contains("\"paymentStatus\":\"succeeded\"", bookingJson);

        var holdGet = await _client.GetAsync($"/api/holds/{createdHold.HoldId}");
        holdGet.EnsureSuccessStatusCode();
        var holdDto = await holdGet.Content.ReadFromJsonAsync<HoldDto>(JsonOptions);
        Assert.NotNull(holdDto);
        Assert.Equal(HoldStatus.ConvertedToBooking, holdDto!.Status);
    }

    [Fact]
    public async Task FailedPaymentFlow_CancelsBooking_AndReleasesHold()
    {
        // Arrange: create a hold
        var holdResponse = await _client.PostAsJsonAsync("/api/holds", new { TripId = "trip-2", Passengers = 1 });
        holdResponse.EnsureSuccessStatusCode();
        var createdHold = await holdResponse.Content.ReadFromJsonAsync<CreateHoldResponse>(JsonOptions);
        Assert.NotNull(createdHold);

        // Act: create booking in pending state
        var createBookingRequest = new CreateBookingRequest
        {
            TripId = createdHold!.TripId,
            HoldId = createdHold.HoldId,
            CustomerName = "Test User",
            CustomerEmail = "test@example.com"
        };

        var bookingResponse = await _client.PostAsJsonAsync("/api/bookings", createBookingRequest);
        bookingResponse.EnsureSuccessStatusCode();
        var createdBooking = await bookingResponse.Content.ReadFromJsonAsync<CreateBookingResponse>(JsonOptions);
        Assert.NotNull(createdBooking);

        // Initiate payment
        var paymentRequest = new InitiatePaymentRequest
        {
            BookingId = createdBooking!.BookingId,
            Amount = 200m
        };
        var initiateResponse = await _client.PostAsJsonAsync("/api/payments", paymentRequest);
        initiateResponse.EnsureSuccessStatusCode();
        var paymentJson = await initiateResponse.Content.ReadAsStringAsync();
        using var paymentDoc = JsonDocument.Parse(paymentJson);
        var paymentId = paymentDoc.RootElement.GetProperty("paymentId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(paymentId));

        // Simulate callback with failure
        var callbackResponse = await _client.PostAsync($"/api/payments/{paymentId}/callback?status=Failed", null);
        Assert.Equal(HttpStatusCode.NoContent, callbackResponse.StatusCode);

        // Assert: booking is cancelled and hold released
        var bookingGet = await _client.GetAsync($"/api/bookings/{createdBooking.BookingId}");
        bookingGet.EnsureSuccessStatusCode();
        var bookingJson = await bookingGet.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"cancelled\"", bookingJson);
        Assert.Contains("\"paymentStatus\":\"failed\"", bookingJson);

        var holdGet = await _client.GetAsync($"/api/holds/{createdHold.HoldId}");
        holdGet.EnsureSuccessStatusCode();
        var holdDto = await holdGet.Content.ReadFromJsonAsync<HoldDto>(JsonOptions);
        Assert.NotNull(holdDto);
        Assert.Equal(HoldStatus.Released, holdDto!.Status);
    }
}

