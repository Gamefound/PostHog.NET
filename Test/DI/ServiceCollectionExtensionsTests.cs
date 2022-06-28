using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostHog;
using PostHog.DI;

namespace Test.DI
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void ServiceCollectionExtensionsTests_ShouldAddServiceWithValidType()
        {
            var apiKey = "writeKey";
            var services = new ServiceCollection();
            services.AddPostHog(apiKey);

            var provider = services.BuildServiceProvider();
            var analyticsInstance = provider.GetRequiredService(typeof(IPostHogClient));

            analyticsInstance.Should().NotBeNull();
            analyticsInstance.Should().BeAssignableTo<PostHogClient>();
        }

        [TestMethod]
        public void ServiceCollectionExtensionsTests_ShouldHaveValidConfigValues()
        {
            var apiKey = "writeKey";
            var services = new ServiceCollection();
            var config = new Config
            {
                FlushAt = 1,
                FlushInterval = TimeSpan.FromSeconds(1),
                Host = "test.host",
                MaxQueueSize = 11,
                MaxRetryTime = TimeSpan.FromSeconds(12),
                Threads = 13,
                Timeout = TimeSpan.FromDays(2),
                UserAgent = "test/agent"
            };

            services.AddPostHog(apiKey, c =>
            {
                c.FlushAt = config.FlushAt;
                c.FlushInterval = config.FlushInterval;
                c.Host = config.Host;
                c.MaxQueueSize = config.MaxQueueSize;
                c.MaxRetryTime = config.MaxRetryTime;
                c.Threads = config.Threads;
                c.Timeout = config.Timeout;
                c.UserAgent = config.UserAgent;
            });

            var provider = services.BuildServiceProvider();
            var analyticsInstance = provider.GetRequiredService(typeof(IPostHogClient));

            analyticsInstance.Should().NotBeNull();

            var typedClient = (PostHogClient)analyticsInstance;
            typedClient.Config.Should().BeEquivalentTo(config);
        }
    }
}