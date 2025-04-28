using k8s;
using Microsoft.AspNetCore.Mvc;

namespace webApiDependencyInjection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleDependencyInjectionOnConstructorController : ControllerBase
    {
        private readonly IKubernetes kubernetesClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleDependencyInjectionOnConstructorController"/> class.
        /// Injects the Kubernetes client into the controller.
        /// </summary>
        /// <param name="kubernetesClient">The Kubernetes client to interact with the Kubernetes API.</param>
        public ExampleDependencyInjectionOnConstructorController(IKubernetes kubernetesClient)
        {
            this.kubernetesClient = kubernetesClient;
        }

        /// <summary>
        /// Retrieves the names of all pods in the default namespace using the injected Kubernetes client.
        /// </summary>
        /// <returns>A collection of pod names in the default namespace.</returns>
        [HttpGet]
        public IEnumerable<string> GetPods()
        {
            // Read the list of pods contained in the default namespace
            var podList = this.kubernetesClient.CoreV1.ListNamespacedPod("default");

            // Return names of pods
            return podList.Items.Select(pod => pod.Metadata.Name);
        }
    }
}
