using System;

namespace PostHog.Model
{
    internal class Capture : BaseAction
    {
        public Capture(string @event, string? distinctId, Properties? properties = null, DateTime? timestamp = null) :
            base(@event, distinctId, properties, timestamp)
        {
        }
    }
}