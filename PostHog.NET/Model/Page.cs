using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PostHog.Model
{
    internal class Page : BaseAction
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string? Category { get; set; }

        public Page(string name, string? category, string? distinctId, IReadOnlyDictionary<string, object>? properties = null, DateTime? timestamp = null) : base("$pageview", distinctId, properties, timestamp)
        {
            Name = name;
            Category = category;
        }
    }
}
