using System;

namespace PostHog.Model
{
    public class Alias : BaseAction
    {
        public Alias(string distinctId, Properties? properties = null, DateTime? timestamp = null) : base(
            "$create_alias", distinctId, properties, timestamp)
        {
        }
    }
}