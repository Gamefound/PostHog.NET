using System;
using System.Threading.Tasks;
using PostHog.Model;
using PostHog.Stats;

namespace PostHog
{
    public interface IPostHogClient : IDisposable
    {
        /// <summary>
        /// Aliases an anonymous user into an identified user.
        /// </summary>
        /// <param name="newId">The anonymous user's id before they are logged in.</param>
        /// <param name="originalId">The identified user's id after they're logged in.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void Alias(string newId, string originalId, DateTime? timestamp = null);

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it.
        /// </summary>
        /// <param name="distinctId">
        /// The visitor's identifier after they log in, or you know
        /// who they are.
        /// </param>
        /// <param name="eventName">
        /// The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"
        /// </param>
        /// <param name="properties">
        /// A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// </param>
        /// <param name="timestamp">The event timestamp.</param>
        void Capture(string distinctId, string eventName, Properties? properties = null, DateTime? timestamp = null);

        /// <summary>
        /// Blocks until all messages are flushed
        /// </summary>
        Task FlushAsync();

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        /// <param name="distinctId">
        /// The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// </param>
        /// <param name="properties">
        /// A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// </param>
        /// <param name="timestamp">The event timestamp.</param>
        void Identify(string distinctId, Properties? properties = null, DateTime? timestamp = null);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        /// <param name="distinctId">
        /// The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like segment-based email campaigns.
        /// </param>
        /// <param name="properties">
        /// A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// </param>
        /// <param name="timestamp">The event timestamp.</param>
        void Page(string distinctId, Properties? properties = null, DateTime? timestamp = null);

        Config? Config { get; }

        /// <summary>
        /// Invoked when action ingestion fails.
        /// </summary>
        Func<BaseAction, Exception, Task> OnFailure { get; set; }

        /// <summary>
        /// Invoked when action is successfully ingested.
        /// </summary>
        Func<BaseAction, Task> OnSuccess { get; set; }

        /// <summary>
        /// Client statistics.
        /// </summary>
        Statistics Statistics { get; }

        /// <summary>
        /// The package version.
        /// </summary>
        public string Version { get; }
    }
}