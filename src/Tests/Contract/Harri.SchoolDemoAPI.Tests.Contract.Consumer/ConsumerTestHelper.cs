using FluentAssertions;
using System.Text.Json;
using RestSharp;
using System.Text.Json.Nodes;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public static class ConsumerTestHelper {

        public static void ShouldContainErrorMessageForProperty<T>(this RestResponse<T> response, string propertyName)
        {
            AssertErrorMessageExistsForProperty(propertyName, response);
        }

        private static void AssertErrorMessageExistsForProperty(string propertyName, RestResponse response)
        {
            var errorJson = GetErrorJson(response);
            errorJson["title"].GetValue<string>().Should().NotBeEmpty();
            errorJson["status"].GetValue<int>().Should().BeGreaterThan(0);

            var errors = errorJson["errors"];
            errors[propertyName].Should().NotBeNull();

            var errorMessages = errors[propertyName].AsArray().GetValues<string>();
            errorMessages.Should().ContainSingle().Which.Should().NotBeEmpty();
        }

        private static JsonNode? GetErrorJson(RestResponse response)
        {
            return JsonSerializer.Deserialize<JsonNode>(response.Content);
        }
    }
}