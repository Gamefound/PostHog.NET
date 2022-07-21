using System;

namespace PostHog.Model
{
    public class Alias : BaseAction
    {
        public Alias(string distincId, Properties? properties = null, DateTime? timestamp = null) : base("$create_alias", distincId, properties, timestamp)
        {
        }
    }
}