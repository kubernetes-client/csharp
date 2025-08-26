using System;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace k8s.Tests
{
    public class CertificateValidationTests
    {
        [Fact]
        public void ShouldRejectCertFromDifferentCA()
        {
            // Load our "trusted" Kubernetes CA
            var trustedCaCert = CertUtils.LoadPemFileCert("assets/ca.crt");

            // Generate a completely different CA and server cert in memory
            var differentCA = CreateSelfSignedCA("CN=Different CA");
            var untrustedServerCert = CreateServerCert(differentCA, "CN=fake-server.com");

            var chain = new X509Chain();

            // Pre-populate the chain like SSL validation would do
            // This will likely succeed because we allow unknown CAs in the validation
            chain.Build(untrustedServerCert);

            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, trustedCaCert, untrustedServerCert, chain, errors);

            // This SHOULD be false because the server cert wasn't signed by our trusted CA
            // But the current K8s validation logic might incorrectly return true
            Assert.False(result, "Should reject certificates not signed by trusted CA");

            // Cleanup
            differentCA.Dispose();
            untrustedServerCert.Dispose();
        }

        // Helper methods to create test certificates
        private static X509Certificate2 CreateSelfSignedCA(string subject)
        {
            using (var rsa = RSA.Create(2048))
            {
                var req = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
                req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));

                return req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(365));
            }
        }

        private static X509Certificate2 CreateServerCert(X509Certificate2 issuerCA, string subject)
        {
            using (var rsa = RSA.Create(2048))
            {
                var req = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
                req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
                req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, true));

                return req.Create(issuerCA, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(90), new byte[] { 1, 2, 3, 4 });
            }
        }

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

        [Fact]
        public void ValidBundleWithMultipleCerts()
        {
            var caCert = CertUtils.LoadPemFileCert("assets/ca-bundle-correct.crt");

            var testCert = caCert[0];
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.True(result);
        }

        [Fact]
        public void InvalidBundleWithMultipleCerts()
        {
            var caCert = CertUtils.LoadPemFileCert("assets/ca-bundle-incorrect.crt");
            var testCert = caCert[0];
            var chain = new X509Chain();
            var errors = SslPolicyErrors.RemoteCertificateChainErrors;

            var result = Kubernetes.CertificateValidationCallBack(this, caCert, testCert, chain, errors);

            Assert.False(result);
        }
    }
}
