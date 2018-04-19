using System;
using Xunit;
using k8s;
using System.IO;

namespace k8s.Tests
{
    public class CertUtilsTests
    {
        /// <summary>
        /// This file contains a sample kubeconfig file
        /// </summary>
        private static readonly string kubeConfigFileName = "assets/kubeconfig.yml";

        /// <summary>
        /// Checks that a certificate can be loaded from files.
        /// </summary>
        [Fact]
        public void LoadFromFiles() 
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFileName, "federal-context");

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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFileName, "victorian-context");

            // Just validate that this doesn't throw and private key is non-null
            var cert = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(cert.PrivateKey);
        }
    }
}
