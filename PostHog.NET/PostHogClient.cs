using PostHog.Flush;
using PostHog.Model;
using PostHog.Request;
using PostHog.Stats;
using System;
using System.Threading.Tasks;

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

        public Config Config { get; }

        public Func<BaseAction, Exception, Task> OnFailure { get; set; } = (action, e) => Task.CompletedTask;

        public Func<BaseAction, Task> OnSuccess { get; set; } = context => Task.CompletedTask;

        public Statistics Statistics { get; set; }

        public string Version => Constants.VERSION;

        public void Alias(string newId, string originalId, DateTime? timestamp = null)
        {
            var properties = new Properties().SetEventProperty("alias", newId);
            Enqueue(new Alias(originalId, properties, timestamp));
        }

        public void Capture(string distinctId, string eventName, Properties? properties = null, DateTime? timestamp = null)
        {
            Enqueue(new Capture(eventName, distinctId, properties, timestamp));
        }

        public void Dispose()
        {
            _flushHandler.Dispose();
        }

        public Task FlushAsync()
        {
            return _flushHandler.FlushAsync();
        }

        public void Identify(string distinctId, Properties? properties = null, DateTime? timestamp = null)
        {
            Enqueue(new Identify(distinctId, properties, timestamp));
        }

        public void Page(string distinctId, Properties? properties = null, DateTime? timestamp = null)
        {
            Enqueue(new Page(distinctId, properties, timestamp));
        }

        internal void RaiseFailure(BaseAction action, Exception e)
        {
            OnFailure.Invoke(action, e);
        }

        internal void RaiseSuccess(BaseAction action)
        {
            OnSuccess.Invoke(action);
        }

        private void Enqueue(BaseAction action)
        {
            _flushHandler.Process(action);
            Statistics.IncrementSubmitted();
        }
    }
}