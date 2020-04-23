using System;
using Xunit;
using k8s;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Linq;

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
            var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.PrivateKey);
        }

        /// <summary>
        /// Checks that a certificate can be loaded from files, in a scenario where the files are using relative paths.
        /// </summary>
        [Fact]
        public void LoadFromFilesRelativePath()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(KubeConfigWithRelativePathsFileName,
                "federal-context");

            // Just validate that this doesn't throw and private key is non-null
            var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.PrivateKey);
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
            var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.PrivateKey);
        }

        /// <summary>
        /// Checks that a certificate can be loaded from inline, in a scenario where the files are using relative paths..
        /// </summary>
        [Fact]
        public void LoadFromInlineDataRelativePath()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(KubeConfigWithRelativePathsFileName,
                "victorian-context");

            // Just validate that this doesn't throw and private key is non-null
            var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.PrivateKey);
        }

        /// <summary>
        /// Checks that the bundle certificate was loaded correctly
        /// </summary>
        [Fact]
        public void LoadPemWithMultiCert()
        {
            var certCollection = CertUtils.LoadPemFileCert("assets/ca-bundle.crt");

            var intermediateCert = new X509Certificate2("assets/ca-bundle-intermediate.crt");
            var rootCert = new X509Certificate2("assets/ca-bundle-root.crt");

            Assert.Equal(2, certCollection.Count);

            Assert.True(certCollection[0].RawData.SequenceEqual(intermediateCert.RawData));
            Assert.True(certCollection[1].RawData.SequenceEqual(rootCert.RawData));
        }
    }
}
