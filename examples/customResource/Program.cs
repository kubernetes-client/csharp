using Json.Patch;
using k8s;
using k8s.Autorest;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;


namespace customResource
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("starting main()...");

            // creating the k8s client
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(k8SClientConfig);

            // creating a K8s client for the CRD
            var myCRD = Utils.MakeCRD();
            Console.WriteLine("working with CRD: {0}.{1}", myCRD.PluralName, myCRD.Group);
            var generic = new GenericClient(client, myCRD.Group, myCRD.Version, myCRD.PluralName);

            // creating a sample custom resource content
            var myCr = Utils.MakeCResource();

            try
            {
                Console.WriteLine("creating CR {0}", myCr.Metadata.Name);
                var response = await client.CustomObjects.CreateNamespacedCustomObjectWithHttpMessagesAsync(
                    myCr,
                    myCRD.Group, myCRD.Version,
                    myCr.Metadata.NamespaceProperty ?? "default",
                    myCRD.PluralName).ConfigureAwait(false);
            }
            catch (HttpOperationException httpOperationException) when (httpOperationException.Message.Contains("422"))
            {
                var phase = httpOperationException.Response.ReasonPhrase;
                var content = httpOperationException.Response.Content;
                Console.WriteLine("response content: {0}", content);
                Console.WriteLine("response phase: {0}", phase);
            }
            catch (HttpOperationException)
            {
            }

            // listing the cr instances
            Console.WriteLine("CR list:");
            var crs = await generic.ListNamespacedAsync<CustomResourceList<CResource>>(myCr.Metadata.NamespaceProperty ?? "default").ConfigureAwait(false);
            foreach (var cr in crs.Items)
            {
                Console.WriteLine("- CR Item {0} = {1}", crs.Items.IndexOf(cr), cr.Metadata.Name);
            }

            var old = JsonSerializer.SerializeToDocument(myCr);
            myCr.Metadata.Labels.TryAdd("newKey", "newValue");

            var expected = JsonSerializer.SerializeToDocument(myCr);
            var patch = old.CreatePatch(expected);

            // updating the custom resource
            var crPatch = new V1Patch(patch, V1Patch.PatchType.JsonPatch);
            try
            {
                var patchResponse = await client.CustomObjects.PatchNamespacedCustomObjectAsync(
                    crPatch,
                    myCRD.Group,
                    myCRD.Version,
                    myCr.Metadata.NamespaceProperty ?? "default",
                    myCRD.PluralName,
                    myCr.Metadata.Name).ConfigureAwait(false);
            }
            catch (HttpOperationException httpOperationException)
            {
                var phase = httpOperationException.Response.ReasonPhrase;
                var content = httpOperationException.Response.Content;
                Console.WriteLine("response content: {0}", content);
                Console.WriteLine("response phase: {0}", phase);
            }

            // getting the updated custom resource
            var fetchedCR = await generic.ReadNamespacedAsync<CResource>(
                myCr.Metadata.NamespaceProperty ?? "default",
                myCr.Metadata.Name).ConfigureAwait(false);

            Console.WriteLine("fetchedCR = {0}", fetchedCR.ToString());

            // deleting the custom resource
            try
            {
                var status = await generic.DeleteNamespacedAsync<V1Status>(
                   myCr.Metadata.NamespaceProperty ?? "default",
                   myCr.Metadata.Name).ConfigureAwait(false);

                Console.WriteLine($"Deleted the CR status: {status}");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception type {0}", exception);
            }
        }
    }
}
