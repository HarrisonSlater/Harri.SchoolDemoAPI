using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.HealthCheckClient;
using HealthChecks.UI.Core;
using PactNet.Matchers;
using System.Net;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class HealthCheckApiConsumerTests : ConsumerTestBase
    {
        [Test]
        public async Task Health_WhenCalled_ReturnsAHealthyReport()
        {
            _pact.UponReceiving($"a request to get health 1")
                    .Given("the api is healthy")
                    .WithRequest(HttpMethod.Get, $"/health")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json")
                 .WithJsonBody(new
                 {
                     status = Match.Equality("Healthy"),
                     totalDuration = Match.Type("00:00:00.0858619"),
                     entries = new
                     {
                         sql = new
                         {
                             data = new { },
                             duration = Match.Type("00:00:00.0856835"),
                             status = Match.Equality("Healthy"),
                             tags = Array.Empty<string>()
                         }
                     }
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new HealthCheckApiClient(ctx.MockServerUri.ToString());
                var healthReport = await client.HealthCheck();

                // Client Assertions
                healthReport.Should().NotBeNull();
                healthReport.Status.Should().Be(UIHealthStatus.Healthy);
                healthReport.TotalDuration.Should().BeGreaterThan(TimeSpan.Zero);

                healthReport.Entries.Should().NotBeNull();
                healthReport.Entries.Should().ContainSingle();

                var singleEntry = healthReport.Entries.Single();
                singleEntry.Key.Should().Be("sql");
                singleEntry.Value.Data.Should().BeEmpty();
                singleEntry.Value.Status.Should().Be(UIHealthStatus.Healthy);
                singleEntry.Value.Tags.Should().BeEmpty();
            });
        }

        [Test]
        public async Task Health_WhenCalled_ReturnsAnUnhealthyReport()
        {
            _pact.UponReceiving($"a request to get health 2")
                    .Given("the api is unhealthy")
                    .WithRequest(HttpMethod.Get, $"/health")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.ServiceUnavailable)
                 .WithHeader("Content-Type", "application/json")
                 .WithJsonBody(new
                 {
                     status = Match.Equality("Unhealthy"),
                     totalDuration = Match.Type("00:00:00.0858619"),
                     entries = new
                     {
                         sql = new
                         {
                             data = new { },
                             description = Match.Type("A connection was successfully established with the server, but then an error occurred during the pre-login handshake."),
                             duration = Match.Type("00:00:00.0856835"),
                             exception = Match.Type("A connection was successfully established with the server, but then an error occurred during the pre-login handshake."),
                             status = Match.Equality("Unhealthy"),
                             tags = Array.Empty<string>()
                         }
                     }
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new HealthCheckApiClient(ctx.MockServerUri.ToString());
                var healthReport = await client.HealthCheck();

                // Client Assertions
                healthReport.Should().NotBeNull();
                healthReport.Status.Should().Be(UIHealthStatus.Unhealthy);
                healthReport.TotalDuration.Should().BeGreaterThan(TimeSpan.Zero);

                healthReport.Entries.Should().NotBeNull();
                healthReport.Entries.Should().ContainSingle();

                var singleEntry = healthReport.Entries.Single();
                singleEntry.Key.Should().Be("sql");
                singleEntry.Value.Data.Should().BeEmpty();
                singleEntry.Value.Description.Should().NotBeNullOrWhiteSpace();
                singleEntry.Value.Exception.Should().NotBeNullOrWhiteSpace();
                singleEntry.Value.Status.Should().Be(UIHealthStatus.Unhealthy);
                singleEntry.Value.Tags.Should().BeEmpty();
            });
        }
    }
}
