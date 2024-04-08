namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    /// <summary>
    /// Provider state DTO
    /// </summary>
    /// From: https://github.com/pact-foundation/pact-net/blob/master/samples/OrdersApi/Provider.Tests/ProviderState.cs
    /// <param name="State">State description</param>
    /// <param name="Params">State parameters</param>
    public record ProviderState(string State, IDictionary<string, object> Params);
}
