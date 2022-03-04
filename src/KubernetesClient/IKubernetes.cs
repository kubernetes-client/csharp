namespace k8s;

public partial interface IKubernetes : IVanillaKubernetes, IDisposable
{
    /// <summary>
    /// The base URI of the service.
    /// </summary>
    Uri BaseUri { get; set; }
}
