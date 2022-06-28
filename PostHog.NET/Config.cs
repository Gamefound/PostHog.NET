using System;

namespace PostHog
{
    public class Config
    {
        public Config(
            TimeSpan? flushInterval = null,
            string host = "https://app.posthog.com",
            TimeSpan? timeout = null,
            int maxQueueSize = 10000,
            int flushAt = 20,
            int threads = 1,
            string? userAgent = null,
            TimeSpan? maxRetryTime = null
        )
        {
            Host = host;
            Timeout = timeout ?? TimeSpan.FromSeconds(5);
            MaxQueueSize = maxQueueSize;
            FlushAt = flushAt;
            FlushInterval = flushInterval ?? TimeSpan.FromSeconds(10);
            UserAgent = userAgent ?? GetDefaultUserAgent();
            Threads = threads;
            MaxRetryTime = maxRetryTime;
        }

        public int FlushAt { get; set; }

        public TimeSpan FlushInterval { get; set; }

        public string Host { get; set; }

        public int MaxQueueSize { get; set; }

        public TimeSpan? MaxRetryTime { get; set; }

        public int Threads { get; set; }

        public TimeSpan Timeout { get; set; }

        public string UserAgent { get; set; }

        private static string GetDefaultUserAgent()
        {
            return $"PostHog.NET/{Constants.VERSION}";
        }
    }
}