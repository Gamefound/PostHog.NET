using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PostHog.Model
{
    public class BaseAction
    {
        public BaseAction(string @event, string? distinctId, IReadOnlyDictionary<string, object>? properties = null, DateTime? timestamp = null)
        {
            Event = @event;
            DistinctId = distinctId;
            Properties = properties;
            Timestamp = timestamp ?? DateTime.Now;
        }

        [JsonProperty(PropertyName = "distinct_id")]
        public string? DistinctId { get; set; }

        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public IReadOnlyDictionary<string, object>? Properties { get; set; }

        [JsonIgnore]
        public int Size { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }
    }
}