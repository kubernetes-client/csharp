using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace k8s;

public partial class Kubernetes
{
    partial void BeforeRequest()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
    }

    partial void AfterRequest()
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;
    }

    private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
    {
        if (SkipTlsVerify)
        {
            return true;
        }

        return CertificateValidationCallBack(sender, CaCerts, certificate, chain, sslPolicyErrors);
    }
}
