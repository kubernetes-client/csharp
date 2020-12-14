using System;
using System.Collections.Generic;
using k8s;
using k8s.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;


namespace customResource
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("strating main()...");

            // creating the k8s client
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(k8SClientConfig);

            // creating a K8s client for the CRD
            var myCRD = Utils.MakeCRD();
            Console.WriteLine("working with CRD: {0}.{1}", myCRD.PluralName, myCRD.Group);
            var generic = new GenericClient(k8SClientConfig, myCRD.Group, myCRD.Version, myCRD.PluralName);

            // creating a sample custom resource content
            var myCr = Utils.MakeCResource();

            try
            {
                Console.WriteLine("creating CR {0}", myCr.Metadata.Name);
                var response = await client.CreateNamespacedCustomObjectWithHttpMessagesAsync(
                    myCr,
                    myCRD.Group, myCRD.Version,
                    myCr.Metadata.NamespaceProperty ?? "default",
                    myCRD.PluralName).ConfigureAwait(false);
            }
            catch (Microsoft.Rest.HttpOperationException httpOperationException) when (httpOperationException.Message.Contains("422"))
            {
                var phase = httpOperationException.Response.ReasonPhrase;
                var content = httpOperationException.Response.Content;
                Console.WriteLine("response content: {0}", content);
                Console.WriteLine("response phase: {0}", phase);
            }
            catch (Microsoft.Rest.HttpOperationException)
            {
            }

            // listing the cr instances
            Console.WriteLine("CR list:");
            var crs = await generic.ListNamespacedAsync<CustomResourceList<CResource>>(myCr.Metadata.NamespaceProperty ?? "default").ConfigureAwait(false);
            foreach (var cr in crs.Items)
            {
                Console.WriteLine("- CR Item {0} = {1}", crs.Items.IndexOf(cr), cr.Metadata.Name);
            }

            // updating the custom resource
            myCr.Metadata.Labels.TryAdd("newKey", "newValue");
            var patch = new JsonPatchDocument<CResource>();
            patch.Replace(x => x.Metadata.Labels, myCr.Metadata.Labels);
            patch.Operations.ForEach(x => x.path = x.path.ToLower());
            var crPatch = new V1Patch(patch, V1Patch.PatchType.JsonPatch);
            try
            {
                var patchResponse = await client.PatchNamespacedCustomObjectAsync(
                    crPatch,
                    myCRD.Group,
                    myCRD.Version,
                    myCr.Metadata.NamespaceProperty ?? "default",
                    myCRD.PluralName,
                    myCr.Metadata.Name).ConfigureAwait(false);
            }
            catch (Microsoft.Rest.HttpOperationException httpOperationException)
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
                myCr = await generic.DeleteNamespacedAsync<CResource>(
                   myCr.Metadata.NamespaceProperty ?? "default",
                   myCr.Metadata.Name).ConfigureAwait(false);

                Console.WriteLine("Deleted the CR");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception type {0}", exception);
            }
        }
    }
}
