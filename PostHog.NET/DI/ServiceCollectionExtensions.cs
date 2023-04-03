using System;
using Microsoft.Extensions.DependencyInjection;

namespace PostHog.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostHog(this IServiceCollection services, string apiKey)
        {
            return AddPostHog(services, apiKey, null);
        }

        public static IServiceCollection AddPostHog(this IServiceCollection services, string apiKey,
            Action<Config>? configuration)
        {
            var config = new Config();
            configuration?.Invoke(config);

            var client = new PostHogClient(apiKey, config);
            services.AddSingleton<IPostHogClient>(client);
            return services;
        }
    }
}