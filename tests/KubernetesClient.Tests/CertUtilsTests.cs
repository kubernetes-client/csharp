using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace k8s.Tests
{
    public class CertUtilsTests
    {
        /// <summary>
        /// This file contains a sample kubeconfig file. The paths to the certificate files are relative
        /// to the current working directly.
        /// </summary>
        private const string KubeConfigFileName = "assets/kubeconfig.yml";

        /// <summary>
        /// This file contains a sample kubeconfig file. The paths to the certificate files are relative
        /// to the directory in which the kubeconfig file is located.
        /// </summary>
        private const string KubeConfigWithRelativePathsFileName = "assets/kubeconfig.relative.yml";

        /// <summary>
        /// Checks that a certificate can be loaded from files.
        /// </summary>
        [Fact]
        public void LoadFromFiles()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(KubeConfigFileName, "federal-context",
                useRelativePaths: false);

            // Just validate that this doesn't throw and private key is non-null
            using var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.GetRSAPrivateKey());
        }

        /// <summary>
        /// Checks that a certificate can be loaded from files, in a scenario where the files are using relative paths.
        /// </summary>
        [Fact]
        public void LoadFromFilesRelativePath()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(
                KubeConfigWithRelativePathsFileName,
                "federal-context");

            // Just validate that this doesn't throw and private key is non-null
            using var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.GetRSAPrivateKey());
        }

        /// <summary>
        /// Checks that a certificate can be loaded from inline.
        /// </summary>
        [Fact]
        public void LoadFromInlineData()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(KubeConfigFileName, "victorian-context",
                useRelativePaths: false);

            // Just validate that this doesn't throw and private key is non-null
            using var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.GetRSAPrivateKey());
        }

        /// <summary>
        /// Checks that a certificate can be loaded from inline, in a scenario where the files are using relative paths..
        /// </summary>
        [Fact]
        public void LoadFromInlineDataRelativePath()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(
                KubeConfigWithRelativePathsFileName,
                "victorian-context");

            // Just validate that this doesn't throw and private key is non-null
            using var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.GetRSAPrivateKey());
        }

        /// <summary>
        /// Checks that the bundle certificate was loaded correctly
        /// </summary>
        [Fact]
        public void LoadPemWithMultiCert()
        {
            var certCollection = CertUtils.LoadPemFileCert("assets/ca-bundle.crt");

#if NET9_0_OR_GREATER
            using var intermediateCert = X509CertificateLoader.LoadCertificateFromFile("assets/ca-bundle-intermediate.crt");
            using var rootCert = X509CertificateLoader.LoadCertificateFromFile("assets/ca-bundle-root.crt");
#else
            using var intermediateCert = new X509Certificate2("assets/ca-bundle-intermediate.crt");
            using var rootCert = new X509Certificate2("assets/ca-bundle-root.crt");
#endif

            Assert.Equal(2, certCollection.Count);

            Assert.True(certCollection[0].RawData.SequenceEqual(intermediateCert.RawData));
            Assert.True(certCollection[1].RawData.SequenceEqual(rootCert.RawData));
        }

        /// <summary>
        /// Checks that multiple certificates can be loaded from PEM text
        /// </summary>
        [Fact]
        public void LoadFromPemTextWithMultiCert()
        {
            // Read the PEM text from the ca-bundle file
            var pemText = System.IO.File.ReadAllText("assets/ca-bundle.crt");
            var certCollection = CertUtils.LoadFromPemText(pemText);

#if NET9_0_OR_GREATER
            using var intermediateCert = X509CertificateLoader.LoadCertificateFromFile("assets/ca-bundle-intermediate.crt");
            using var rootCert = X509CertificateLoader.LoadCertificateFromFile("assets/ca-bundle-root.crt");
#else
            using var intermediateCert = new X509Certificate2("assets/ca-bundle-intermediate.crt");
            using var rootCert = new X509Certificate2("assets/ca-bundle-root.crt");
#endif

            Assert.Equal(2, certCollection.Count);

            Assert.True(certCollection[0].RawData.SequenceEqual(intermediateCert.RawData));
            Assert.True(certCollection[1].RawData.SequenceEqual(rootCert.RawData));
        }

        /// <summary>
        /// Checks that a single certificate can be loaded from PEM text
        /// </summary>
        [Fact]
        public void LoadFromPemTextWithSingleCert()
        {
            // Read a single certificate PEM text
            var pemText = System.IO.File.ReadAllText("assets/ca-bundle-root.crt");
            var certCollection = CertUtils.LoadFromPemText(pemText);

#if NET9_0_OR_GREATER
            using var rootCert = X509CertificateLoader.LoadCertificateFromFile("assets/ca-bundle-root.crt");
#else
            using var rootCert = new X509Certificate2("assets/ca-bundle-root.crt");
#endif

            Assert.Single(certCollection);
            Assert.True(certCollection[0].RawData.SequenceEqual(rootCert.RawData));
        }
    }
}
