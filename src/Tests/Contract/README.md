
# Contract Tests WIP

Consumer driven contract tests using pact net: https://github.com/pact-foundation/pact-net

Tests for StudentApiClient and StudentApiController have been completed so far.

# Provider Tests
The Provider tests have a mocked data layer, In the case of the Students API IStudentRepository is mocked using Moq.

ProviderStateMiddleware.cs base file implementation from: [https://github.com/pact-foundation/pact-net/tree/master/samples/OrdersApi/Provider.Tests
](https://github.com/pact-foundation/pact-net/blob/master/samples/OrdersApi/Provider.Tests/ProviderStateMiddleware.cs)


# Nuget packages used
- NUnit
- FluentAssertions
- Moq
- PactNet
