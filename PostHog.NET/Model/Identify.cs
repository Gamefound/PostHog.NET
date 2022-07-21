namespace PostHog.Model
{
    public class Identify : BaseAction
    {
        public Identify(string userId, Properties? properties) : base("$identify", userId, properties)
        {
        }
    }
}