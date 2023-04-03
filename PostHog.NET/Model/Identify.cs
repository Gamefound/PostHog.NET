using System;

namespace PostHog.Model
{
    public class Identify : BaseAction
    {
        public Identify(string userId, Properties? properties, DateTime? timestamp = null) : base("$identify", userId, properties, timestamp)
        {
        }
    }
}