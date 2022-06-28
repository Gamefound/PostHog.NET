using System;
using System.Threading.Tasks;
using PostHog.Model;
using PostHog.Stats;

namespace PostHog
{
    public interface IPostHogClient : IDisposable
    {
        public delegate void FailedHandler(BaseAction action, Exception e);

        public delegate void SucceededHandler(BaseAction action);

        event FailedHandler Failed;

        event SucceededHandler Succeeded;

        Config? Config { get; }

        Statistics Statistics { get; set; }

        /// <summary>
        /// Aliases an anonymous user into an identified user.
        /// </summary>
        ///
        /// <param name="newId">The anonymous user's id before they are logged in.</param>
        ///
        /// <param name="originalId">the identified user's id after they're logged in.</param>
        void Alias(string newId, string originalId);

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it.
        /// </summary>
        ///
        /// <param name="distinctId">The visitor's identifier after they log in, or you know
        /// who they are. </param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="properties">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record. </param>
        void Capture(string distinctId, string eventName, Properties? properties = null);

        /// <summary>
        /// Blocks until all messages are flushed
        /// </summary>
        Task FlushAsync();

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        ///
        /// <param name="distinctId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="properties">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record. </param>
        void Identify(string distinctId, Properties? properties = null);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="distinctId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like segment-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        ///
        /// <param name="category">The (optional) category of the webpage, like "Authentication", "Sports"</param>
        ///
        /// <param name="properties">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record. </param>
        void Page(string distinctId, string name, string? category = null, Properties? properties = null);
    }
}