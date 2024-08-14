
# Contract Tests WIP
Consumer driven contract tests using pact net: https://github.com/pact-foundation/pact-net.

Tests for StudentApiClient and StudentApiController have been completed so far.

Contract tests are used in this project to reduce the required number of Integration tests and E2E tests needed to achieve full test coverage of the API.

Benefits of using Contract tests over Integration or E2E tests to validate contracts:
- Contract tests are unit tests
- They run in the build step and fail fast (before deploy step)
- Test execution time (fast)

# Consumer Tests
See [StudentApiConsumerTests.cs](https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Tests/Contract/Harri.SchoolDemoAPI.Tests.Contract.Consumer/StudentApiConsumerTests.cs)

# Provider Tests
The Provider tests have a mocked data layer, In the case of the Students API IStudentRepository is mocked using Moq.

ProviderStateMiddleware.cs base file implementation from: [pact-net ProviderStateMiddleware.cs](https://github.com/pact-foundation/pact-net/blob/master/samples/OrdersApi/Provider.Tests/ProviderStateMiddleware.cs)

Provider tests assert that a specific json request, given a state of the provider, returns a specific json response.
These tests go one step further to assert the object is deserialised correctly within the provider.

Assertions are made on the repository interface mock in [ProviderStateMiddleware.cs](Harri.SchoolDemoAPI.Tests.Contract.Provider/Provider/ProviderStateMiddleware.cs).

The expected values to assert are passed along with the provider state in the contract test definition in the consumer project [Example: StudentApiConsumerTests.cs](Harri.SchoolDemoAPI.Tests.Contract.Consumer/StudentApiConsumerTests.cs). This is so the request json, expected response json, and expected deserialised json all live in the test definition.

``` C#
            .Given("a student with sId {sId} exists", new Dictionary<string, string>() {
                // Expected deserialised values inside the provider
                { "sId", sId.ToString() }, 
                { "name", name },
                { "GPA", GPA?.ToString() ?? "null" },
            })
```
