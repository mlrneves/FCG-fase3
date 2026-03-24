using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Swagger_Should_Return_Valid_Response()
    {
        var response = await _client.GetAsync("/swagger");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Moved,
            HttpStatusCode.Redirect,
            HttpStatusCode.TemporaryRedirect,
            HttpStatusCode.PermanentRedirect,
            HttpStatusCode.NotFound);
    }
}