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
}
