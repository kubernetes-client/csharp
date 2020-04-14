using System.Linq;
using System.Net;
using System.Threading.Tasks;
using k8s.Models;
using k8s.Versioning;
using Microsoft.Rest;

namespace k8s.CustomResources
{
    public static class KubernetesCustomResourcesExtensions
    {
        /// <summary>
        /// Installs custom resource definition into the Kubernetes API server
        /// </summary>
        /// <param name="kubernetes">Kubernetes client</param>
        /// <param name="crdRegistration">Custom resource definition</param>
        /// <returns></returns>
        public static async Task InstallCustomResourceDefinition(this IKubernetes kubernetes, V1CustomResourceDefinition crdRegistration)
        {
            var crdMetadata = typeof(V1CustomResourceDefinition).GetKubernetesTypeMetadata();
            var versions = await kubernetes.GetAPIVersions1Async();
            var serverCrdVersion = versions.Groups.First(x => x.Name == crdMetadata.Group).PreferredVersion.Version;
            var crdType = VersionConverter.GetTypeForVersion<V1CustomResourceDefinition>(serverCrdVersion);
            try
            {
                await kubernetes.ReadWithHttpMessagesAsync(crdType, crdRegistration.Metadata.Name);

                dynamic existingResponse = await kubernetes.ReadWithHttpMessagesAsync(crdType, crdRegistration.Metadata.Name);
                var existing = (V1CustomResourceDefinition)VersionConverter.ConvertToVersion(existingResponse.Body, "v1");
                var installedVersions = existing.Spec.Versions.Select(x => x.Name).ToList();
                var declaredVersions = crdRegistration.Spec.Versions.Select(x => x.Name).ToList();
                var requirePatch = declaredVersions.Any(x => !installedVersions.Contains(x));
                if (requirePatch)
                {
                    await kubernetes.ReplaceWithHttpMessagesAsync(VersionConverter.ConvertToVersion(crdRegistration, serverCrdVersion), crdRegistration.Metadata.Name);
                }
            }
            catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.NotFound)
            {
                await kubernetes.CreateWithHttpMessagesAsync(VersionConverter.ConvertToVersion(crdRegistration, serverCrdVersion));
            }
        }

        /// <summary>
        /// Uninstalls CRD from kubernetes server
        /// </summary>
        /// <param name="kubernetes">Kubernetes client</param>
        /// <param name="crdRegistration">Custom resource definition</param>
        /// <returns></returns>
        public static async Task UnInstallCustomResourceDefinition(this IKubernetes kubernetes, V1CustomResourceDefinition crdRegistration) =>
            await kubernetes.UnInstallCustomResourceDefinition(crdRegistration.Metadata.Name);

        /// <summary>
        /// Uninstalls CRD from kubernetes server
        /// </summary>
        /// <param name="kubernetes">Kubernetes client</param>
        /// <param name="name">The name of the custom resource definition. This takes form of pluralname.group</param>
        /// <returns></returns>
        public static async Task UnInstallCustomResourceDefinition(this IKubernetes kubernetes, string name)
        {
            var crdMetadata = typeof(V1CustomResourceDefinition).GetKubernetesTypeMetadata();
            var versions = await kubernetes.GetAPIVersions1Async();
            var serverCrdVersion = versions.Groups.First(x => x.Name == crdMetadata.Group).PreferredVersion.Version;
            var crdType = VersionConverter.GetTypeForVersion<V1CustomResourceDefinition>(serverCrdVersion);
            try
            {
                await kubernetes.DeleteWithHttpMessagesAsync(crdType, name);
            }
            catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.NotFound) // CRD doesn't exist
            {

            }
        }
    }
}
