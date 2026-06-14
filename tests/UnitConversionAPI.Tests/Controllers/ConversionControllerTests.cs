using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionAPI.Core.Models;

namespace UnitConversionAPI.Tests.Controllers;

public class ConversionControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConversionControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // --- GET /api/units ---

    [Fact]
    public async Task GetUnits_ReturnsOkWithCategories()
    {
        var response = await _client.GetAsync("/api/units");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        body.Should().NotBeNullOrEmpty();

        var categories = body!.Select(e => e.GetProperty("category").GetString()).ToList();
        categories.Should().Contain("length");
        categories.Should().Contain("temperature");
        categories.Should().Contain("weight");
    }

    [Fact]
    public async Task GetUnits_EachCategoryHasUnits()
    {
        var body = await _client.GetFromJsonAsync<JsonElement[]>("/api/units");

        foreach (var categoryGroup in body!)
        {
            var units = categoryGroup.GetProperty("units").EnumerateArray().ToList();
            units.Should().NotBeEmpty(because: $"category '{categoryGroup.GetProperty("category")}' should have units");

            foreach (var unit in units)
            {
                unit.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();
                unit.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace();
                unit.GetProperty("symbol").GetString().Should().NotBeNullOrWhiteSpace();
            }
        }
    }

    // --- GET /api/units/{category} ---

    [Theory]
    [InlineData("length")]
    [InlineData("temperature")]
    [InlineData("weight")]
    public async Task GetUnitsByCategory_KnownCategory_ReturnsUnits(string category)
    {
        var response = await _client.GetAsync($"/api/units/{category}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        body.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetUnitsByCategory_UnknownCategory_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/units/velocity");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUnitsByCategory_CaseInsensitive_ReturnsUnits()
    {
        var response = await _client.GetAsync("/api/units/LENGTH");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // --- GET /api/convert ---

    [Fact]
    public async Task Convert_MetersToFeet_ReturnsCorrectResult()
    {
        var response = await _client.GetAsync("/api/convert?value=1&from=meter&to=foot");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(3.28083, 1e-4);
        result.Category.Should().Be("length");
    }

    [Fact]
    public async Task Convert_BoilingPoint_CelsiusToFahrenheit()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=celsius&to=fahrenheit");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result!.OutputValue.Should().BeApproximately(212.0, 1e-6);
    }

    [Fact]
    public async Task Convert_UnknownUnit_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/convert?value=1&from=parsec&to=meter");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("error").GetString().Should().Contain("parsec");
    }

    [Fact]
    public async Task Convert_IncompatibleUnits_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=celsius&to=kilogram");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_MissingFromParameter_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/convert?value=1&to=meter");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_MissingToParameter_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/convert?value=1&from=meter");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_NegativeValue_WorksCorrectly()
    {
        var response = await _client.GetAsync("/api/convert?value=-40&from=celsius&to=fahrenheit");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result!.OutputValue.Should().BeApproximately(-40.0, 1e-6);
    }

    [Fact]
    public async Task Convert_ZeroValue_WorksCorrectly()
    {
        var response = await _client.GetAsync("/api/convert?value=0&from=meter&to=foot");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result!.OutputValue.Should().BeApproximately(0.0, 1e-9);
    }
}
