using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace k8s.tests
{
    public class CertificateValidationTests
    {
        [Fact]
        public void ValidCert()
        {
            var caCert = new X509Certificate2("assets/ca.crt");
            var testCert = new X509Certificate2("assets/ca.crt");
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.True(result);
        }

        [Fact]
        public void InvalidCert()
        {
            var caCert = new X509Certificate2("assets/ca.crt");
            var testCert = new X509Certificate2("assets/ca2.crt");
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.False(result);
        }
    }
}
