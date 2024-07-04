using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PostHog.Model
{
    public class Batch
    {
        public Batch(List<BaseAction> actions, string apiKey)
        {
            Actions = actions;
            ApiKey = apiKey;
        }

        [JsonPropertyName(name: "batch")]
        public List<BaseAction> Actions { get; set; }

        [JsonPropertyName(name: "api_key")]
        public string ApiKey { get; set; }
    }
}