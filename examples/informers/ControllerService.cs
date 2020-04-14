using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace informers
{
    /// <summary>
    /// Starts all controllers registered in dependency injection container
    /// </summary>
    public class ControllerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ControllerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var controllers = _serviceProvider.GetServices<IController>();
            await Task.WhenAll(controllers.Select(x => x.Initialize(stoppingToken)));
        }

    }
}
