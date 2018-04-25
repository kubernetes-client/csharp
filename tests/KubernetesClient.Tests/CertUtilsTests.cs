using System;
using Xunit;
using k8s;
using System.IO;

namespace k8s.Tests
{
    public class CertUtilsTests
    {
        /// <summary>
        /// This file contains a sample kubeconfig file. The paths to the certificate files are relative
        /// to the current working directly.
        /// </summary>
        private static readonly string kubeConfigFileName = "assets/kubeconfig.yml";

        /// <summary>
        /// This file contains a sample kubeconfig file. The paths to the certificate files are relative
        /// to the directory in which the kubeconfig file is located.
        /// </summary>
        private static readonly string kubeConfigWithRelativePathsFileName = "assets/kubeconfig.relative.yml";

        /// <summary>
        /// Checks that a certificate can be loaded from files.
        /// </summary>
        [Fact]
        public void LoadFromFiles() 
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFileName, "federal-context", useRelativePaths: false);

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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigWithRelativePathsFileName, "federal-context");

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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFileName, "victorian-context", useRelativePaths: false);

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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigWithRelativePathsFileName, "victorian-context");

            // Just validate that this doesn't throw and private key is non-null
            var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.PrivateKey);
        }
    }
}
