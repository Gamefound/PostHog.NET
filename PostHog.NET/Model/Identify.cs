using System.Collections.Generic;

namespace PostHog.Model
{
    public class Identify : BaseAction
    {
        public Identify(string userId, IReadOnlyDictionary<string, object>? properties) : base("$identify", userId, properties)
        {

        }
    }
}
