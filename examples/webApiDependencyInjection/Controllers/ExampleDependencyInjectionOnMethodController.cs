using k8s;
using Microsoft.AspNetCore.Mvc;

namespace webApiDependencyInjection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleDependencyInjectionOnMethodController : ControllerBase
    {
        /// <summary>
        /// Example using the kubernetes client injected directly into the method ([FromServices] IKubernetes kubernetesClient).
        /// </summary>
        /// <param name="kubernetes"></param>
        /// <returns></returns>
        [HttpGet()]
        public IEnumerable<string> GetPods([FromServices] IKubernetes kubernetesClient)
        {
            // Read the list of pods contained in default namespace
            var podList = kubernetesClient.CoreV1.ListNamespacedPod("default");

            // Return names of pods
            return podList.Items.Select(pod => pod.Metadata.Name);
        }
    }
}
