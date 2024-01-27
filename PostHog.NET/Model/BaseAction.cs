using System;
using System.Text.Json.Serialization;

namespace PostHog.Model
{
    public class BaseAction
    {
        public BaseAction(string @event, string? distinctId, Properties? properties = null, DateTime? timestamp = null)
        {
            Event = @event;
            DistinctId = distinctId;
            Properties = properties;
            Timestamp = timestamp ?? DateTime.UtcNow;
        }

        [JsonPropertyName(name: "distinct_id")]
        public string? DistinctId { get; set; }

        [JsonPropertyName(name: "event")]
        public string Event { get; set; }

        [JsonPropertyName(name: "properties")]
        public Properties? Properties { get; set; }

        [JsonIgnore]
        public int Size { get; set; }

        [JsonPropertyName(name: "timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
