using Presentation.IntegrationTests.Fixtures;
using System.Net;

namespace Presentation.IntegrationTests.Endpoints;

[Collection(ApiCollection.Name)]
public class HealthEndpointTests(ApiFixture fixture)
{
    [Fact]
    public async Task Health_ShouldBeHealthy_WhenDatabaseIsReachable()
    {
        HttpResponseMessage response = await fixture.Factory.CreateClient().GetAsync(new Uri("/health", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("Healthy");
    }

    [Fact]
    public async Task Alive_ShouldAnswer()
    {
        HttpResponseMessage response = await fixture.Factory.CreateClient().GetAsync(new Uri("/alive", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
