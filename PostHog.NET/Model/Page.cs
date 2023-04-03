using System;

namespace PostHog.Model
{
    internal class Page : BaseAction
    {
        public Page(string? distinctId, Properties? properties = null, DateTime? timestamp = null) : base("$pageview",
            distinctId, properties, timestamp)
        {
        }
    }
}