using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace k8s.Tests.Mock.Server
{
    /// <summary>
    ///     Startup logic for the KubeClient WebSockets test server.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Create a new <see cref="Startup"/>.
        /// </summary>
        public Startup()
        {
        }

        /// <summary>
        ///     Configure application services.
        /// </summary>
        /// <param name="services">
        ///     The service collection to configure.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddLogging(logging =>
            {
                logging.ClearProviders(); // Logger provider will be added by the calling test.
            });
            services.AddMvc();
        }

        /// <summary>
        ///     Configure the application pipeline.
        /// </summary>
        /// <param name="app">
        ///     The application pipeline builder.
        /// </param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5),
                ReceiveBufferSize = 2048
            });
            app.UseMvc();
        }
    }
}
