using Presentation.IntegrationTests.Fixtures;
using System.Net;
using System.Text.Json;

namespace Presentation.IntegrationTests.Endpoints;

[Collection(ApiCollection.Name)]
public class OpenApiDocumentTests(ApiFixture fixture)
{
    [Fact]
    public async Task OpenApiDocument_ShouldExposeSystemInfo_WithStableOperationId()
    {
        HttpResponseMessage response = await fixture.Factory.CreateClient()
            .GetAsync(new Uri("/openapi/v1.json", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        using JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement operation = json.RootElement.GetProperty("paths").GetProperty("/api/v1/system/info").GetProperty("get");
        operation.GetProperty("operationId").GetString().ShouldBe("GetSystemInfo");
    }

    [Fact]
    public async Task OpenApiDocument_ShouldDescribeProblemDetails_WithCodeAndStrictStatus()
    {
        HttpResponseMessage response = await fixture.Factory.CreateClient()
            .GetAsync(new Uri("/openapi/v1.json", UriKind.Relative));

        using JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement problem = json.RootElement.GetProperty("components").GetProperty("schemas").GetProperty("ProblemDetails");
        JsonElement properties = problem.GetProperty("properties");

        properties.TryGetProperty("code", out _).ShouldBeTrue();
        properties.TryGetProperty("args", out _).ShouldBeTrue();
        properties.TryGetProperty("traceId", out _).ShouldBeTrue();
        properties.TryGetProperty("errors", out _).ShouldBeTrue();
        problem.GetProperty("required").EnumerateArray().Select(e => e.GetString()).ShouldContain("code");

        JsonElement status = properties.GetProperty("status");
        status.ToString().ShouldNotContain("string");
        status.ToString().ShouldContain("integer");
    }
}
