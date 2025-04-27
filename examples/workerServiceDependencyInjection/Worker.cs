using k8s;

namespace workerServiceDependencyInjection
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IKubernetes kubernetesClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// Inject in the constructor the IKubernetes interface.
        /// </summary>
        /// <param name="logger">The logger instance used for logging information.</param>
        /// <param name="kubernetesClient">The Kubernetes client used to interact with the Kubernetes API.</param>
        public Worker(ILogger<Worker> logger, IKubernetes kubernetesClient)
        {
            this.logger = logger;
            this.kubernetesClient = kubernetesClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // Read the list of pods contained in default namespace
                var podList = kubernetesClient.CoreV1.ListNamespacedPod("default");

                // Print pods names
                foreach (var pod in podList.Items)
                {
                    Console.WriteLine(pod.Metadata.Name);
                }

                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
