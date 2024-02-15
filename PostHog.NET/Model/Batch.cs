using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PostHog.Model
{
    internal class Batch
    {
        internal Batch(List<BaseAction> actions, string apiKey)
        {
            Actions = actions;
            ApiKey = apiKey;
        }

        [JsonPropertyName(name: "batch")]
        internal List<BaseAction> Actions { get; set; }

        [JsonPropertyName(name: "api_key")]
        internal string ApiKey { get; set; }
    }
}