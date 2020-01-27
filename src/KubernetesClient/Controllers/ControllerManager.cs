using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s.Informer;
using Microsoft.Extensions.Logging;

namespace k8s.Controllers
{
    public class ControllerManager : IController
    {
        private IController[] _controllers;
        private readonly ILogger<ControllerManager> _log;
        private SharedInformerFactory _informerFactory;

        public ControllerManager(SharedInformerFactory informerFactory, IController[] controllers, ILogger<ControllerManager> log)
        {
            _controllers = controllers;
            _log = log;
            _informerFactory = informerFactory;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_controllers.Select(x => x.StopAsync(cancellationToken)));
            await _informerFactory.StopAsync(cancellationToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_controllers.Length == 0) 
            {
                throw new Exception("no controller registered in the manager..");
            }

            await _informerFactory.StartAsync(cancellationToken);
            _log.LogDebug("Controller-Manager {} bootstrapping..");
            await Task.WhenAll(_controllers.Select(x => x.StartAsync(cancellationToken)));
        }
    }
}