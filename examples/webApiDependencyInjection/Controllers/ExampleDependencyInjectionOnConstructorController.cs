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
        /// Inject the kubernets class in the constructor.
        /// </summary>
        /// <param name="kubernetesClient"></param>
        public ExampleDependencyInjectionOnConstructorController(IKubernetes kubernetesClient)
        {
            this.kubernetesClient = kubernetesClient;
        }

        /// <summary>
        /// Example using the kubernetes client obtained from the constructor (this.kubernetesClient).
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IEnumerable<string> GetPods()
        {
            // Read the list of pods contained in default namespace
            var podList = this.kubernetesClient.CoreV1.ListNamespacedPod("default");

            // Return names of pods
            return podList.Items.Select(pod => pod.Metadata.Name);
        }
    }
}
