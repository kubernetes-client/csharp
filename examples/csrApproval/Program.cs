using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Json.Patch;
using k8s;
using k8s.Models;

string GenerateCertificate(string name)
{
    var sanBuilder = new SubjectAlternativeNameBuilder();
    sanBuilder.AddIpAddress(IPAddress.Loopback);
    sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
    sanBuilder.AddDnsName("localhost");
    sanBuilder.AddDnsName(Environment.MachineName);

    var distinguishedName = new X500DistinguishedName(name);

    using var rsa = RSA.Create(4096);
    var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256,RSASignaturePadding.Pkcs1);

    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature , false));
    request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new ("1.3.6.1.5.5.7.3.1") }, false));
    request.CertificateExtensions.Add(sanBuilder.Build());
    var csr = request.CreateSigningRequest();
    var pemKey = "-----BEGIN CERTIFICATE REQUEST-----\r\n" +
                 Convert.ToBase64String(csr) +
                 "\r\n-----END CERTIFICATE REQUEST-----";

    return pemKey;
}

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
IKubernetes client = new Kubernetes(config);
Console.WriteLine("Starting Request!");
var name = "demo";
var x509 = GenerateCertificate(name);
var encodedCsr= Encoding.UTF8.GetBytes(x509);

var request = new V1CertificateSigningRequest
{
    ApiVersion = "certificates.k8s.io/v1",
    Kind = "CertificateSigningRequest",
    Metadata = new V1ObjectMeta
    {
        Name = name
    },
    Spec = new V1CertificateSigningRequestSpec
    {
        Request = encodedCsr,
        SignerName = "kubernetes.io/kube-apiserver-client",
        Usages = new List<string> { "client auth" },
        ExpirationSeconds = 600 // minimum should be 10 minutes
    }
};

await client.CreateCertificateSigningRequestAsync(request);

var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};
var readCert = await client.ReadCertificateSigningRequestAsync(name);
var old = JsonSerializer.SerializeToDocument(readCert, serializeOptions);

var replace = new List<V1CertificateSigningRequestCondition>
{
    new("True", "Approved", DateTime.UtcNow, DateTime.UtcNow, "This certificate was approved by k8s client", "Approve")
};
readCert.Status.Conditions = replace;

var expected = JsonSerializer.SerializeToDocument(readCert, serializeOptions);

var patch = old.CreatePatch(expected);
await client.PatchCertificateSigningRequestApprovalAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name);
