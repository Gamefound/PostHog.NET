using System;
using System.Collections.Generic;

namespace PostHog.Model
{
    public class Alias : BaseAction
    {
        public Alias(string distincId, IReadOnlyDictionary<string, object>? properties = null, DateTime? timestamp = null) : base("$create_alias", distincId, properties, timestamp)
        {
        }
    }
}
