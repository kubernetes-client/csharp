using System;
using System.Linq;
using System.Threading;
using k8s;
using k8s.Informers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;

namespace informers
{
    public static class Extensions
    {
        public static IServiceCollection AddKubernetesClient(this IServiceCollection services, Func<KubernetesClientConfiguration> configProvider)
        {
            var config = configProvider();
            services.AddHttpClient("DefaultName")
                .AddTypedClient<IKubernetes>((httpClient, serviceProvider) =>
                {
                    httpClient.Timeout = Timeout.InfiniteTimeSpan;
                    return new Kubernetes(config, httpClient);
                })
                .AddHttpMessageHandler(() => new TimeoutHandler(TimeSpan.FromSeconds(100)))
                .AddHttpMessageHandler(() => new RetryDelegatingHandler { RetryPolicy = new RetryPolicy<HttpStatusCodeErrorDetectionStrategy>(new ExponentialBackoffRetryStrategy()) })
                .AddHttpMessageHandler(KubernetesClientConfiguration.CreateWatchHandler)
                .ConfigurePrimaryHttpMessageHandler(config.CreateDefaultHttpClientHandler);

            return services;
        }

        public static IServiceCollection AddKubernetesInformers(this IServiceCollection services)
        {
            services.AddTransient(typeof(KubernetesInformer<>));
            services.AddSingleton(typeof(IKubernetesInformer<>), typeof(SharedKubernetesInformer<>));
            return services;
        }
    }
}
