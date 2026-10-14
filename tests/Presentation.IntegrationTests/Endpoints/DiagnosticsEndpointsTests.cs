using Presentation.IntegrationTests.Fixtures;
using System.Net;
using System.Text.Json;

namespace Presentation.IntegrationTests.Endpoints;

[Collection(ApiCollection.Name)]
public class DiagnosticsEndpointsTests(ApiFixture fixture)
{
    [Fact]
    public async Task SystemInfo_ShouldReturnEnvelopedNameAndVersion()
    {
        HttpResponseMessage response = await fixture.Factory.CreateClient()
            .GetAsync(new Uri("/api/v1/system/info", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        using JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement data = json.RootElement.GetProperty("data");
        data.GetProperty("name").GetString().ShouldBe("Schemy");
        data.GetProperty("version").GetString().ShouldNotBeNullOrWhiteSpace();
    }
}
