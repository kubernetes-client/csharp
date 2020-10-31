using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace k8s.Tests
{
    public class CertificateValidationTests
    {
        [Fact]
        public void ValidCert()
        {
            var caCert = CertUtils.LoadPemFileCert("assets/ca.crt");
            var testCert = new X509Certificate2("assets/ca.crt");
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.True(result);
        }

        [Fact]
        public void InvalidCert()
        {
            var caCert = CertUtils.LoadPemFileCert("assets/ca.crt");
            var testCert = new X509Certificate2("assets/ca2.crt");
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.False(result);
        }

        [Fact]
        public void ValidBundleCert()
        {
            var caCert = CertUtils.LoadPemFileCert("assets/ca-bundle.crt");

            // Load the intermediate cert
            //
            var testCert = caCert[0];
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.True(result);
        }

        [Fact]
        public void InvalidBundleCert()
        {
            var caCert = CertUtils.LoadPemFileCert("assets/ca-bundle.crt");
            var testCert = new X509Certificate2("assets/ca2.crt");
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.False(result);
        }
    }
}
