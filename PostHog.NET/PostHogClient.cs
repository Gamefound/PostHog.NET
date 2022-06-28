using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostHog.Flush;
using PostHog.Model;
using PostHog.Request;
using PostHog.Stats;

namespace PostHog
{
    public class PostHogClient : IPostHogClient
    {
        private readonly AsyncIntervalFlushHandler _flushHandler;

        /// <summary>
        /// Creates a new REST client with a specified API writeKey and default config
        /// </summary>
        /// <param name="apiKey"></param>
        public PostHogClient(string apiKey) : this(apiKey, new Config()) { }

        public PostHogClient(string apiKey, Config config)
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));

            Config = config;
            Statistics = new Statistics();

            IRequestHandler requestHandler;

            if (config.MaxRetryTime.HasValue)
            {
                requestHandler = new BlockingRequestHandler(this, config.Timeout, new Backoff(max: (Convert.ToInt32(config.MaxRetryTime.Value.TotalSeconds) * 1000), jitter: 5000));
            }
            else
            {
                requestHandler = new BlockingRequestHandler(this, config.Timeout);
            }

            _flushHandler = new AsyncIntervalFlushHandler(requestHandler, config.MaxQueueSize, config.FlushAt, config.FlushInterval, config.Threads, apiKey);
        }

        public event IPostHogClient.FailedHandler? Failed;

        public event IPostHogClient.SucceededHandler? Succeeded;

        public Config Config { get; }

        public Statistics Statistics { get; set; }

        public void Alias(string newId, string originalId)
        {
            Enqueue(new Alias(originalId, new Dictionary<string, object>
            {
                { "distinct_id", originalId },
                { "alias", newId }
            }));
        }

        public void Capture(string distinctId, string eventName, Properties? properties = null)
        {
            Enqueue(new Capture(eventName, distinctId, properties));
        }

        public void Dispose()
        {
            _flushHandler.Dispose();
        }

        public Task FlushAsync()
        {
            return _flushHandler.FlushAsync();
        }

        public void Identify(string distinctId, Properties? properties = null)
        {
            Enqueue(new Identify(distinctId, properties));
        }

        public void Page(string distinctId, string name, string? category = null, Properties? properties = null)
        {
            Enqueue(new Page(name, category, distinctId, properties));
        }

        internal void RaiseFailure(BaseAction action, Exception e)
        {
            Failed?.Invoke(action, e);
        }

        internal void RaiseSuccess(BaseAction action)
        {
            Succeeded?.Invoke(action);
        }

        private void Enqueue(BaseAction action)
        {
            _flushHandler.Process(action);
            Statistics.IncrementSubmitted();
        }
    }
}