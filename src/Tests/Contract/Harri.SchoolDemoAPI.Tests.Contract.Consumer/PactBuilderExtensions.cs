using System.Text.Json;
using PactNet;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public static class PactBuilderExtensions {

        // Provider state dictionary object setup helpers
        public static IRequestBuilderV4 Given<T>(this IRequestBuilderV4 pact, string providerState, T stateObject)
        {
            pact.Given(providerState, CreateBaseProviderStateDictionary(stateObject));
            return pact;
        }

        public static IRequestBuilderV4 Given<T>(this IRequestBuilderV4 pact, string providerState, T stateObject, KeyValuePair<string, string> keyPair)
        {
            var dictionary = CreateBaseProviderStateDictionary(stateObject);
            dictionary.Add(keyPair.Key, keyPair.Value);

            pact.Given(providerState, dictionary);
            return pact;
        }

        private static Dictionary<string, string> CreateBaseProviderStateDictionary<T>(T stateObject)
        {
            //TODO type check the provider state against T before serialising, currently if the types don't match they fail when running the provider state setup methods
            Type stateObjectType = typeof(T);

            var dictionary = new Dictionary<string, string>
            {
                { "stateObject", JsonSerializer.Serialize(stateObject) },
                { "stateObjectType", stateObjectType.Name },
            };

            return dictionary;
        }
    }
}