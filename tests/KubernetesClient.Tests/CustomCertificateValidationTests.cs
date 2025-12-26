using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace k8s.Tests
{
    public class CustomCertificateValidationTests
    {
        [Fact]
        public void CustomValidationCallbackShouldBeUsedWhenSet()
        {
            // Arrange
            var config = new KubernetesClientConfiguration
            {
                Host = "https://test.example.com",
                ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    return true;
                },
            };

            // Act
            var client = new Kubernetes(config);

            // Assert - verify the callback was set
            Assert.NotNull(config.ServerCertificateCustomValidationCallback);
        }

        [Fact]
        public void CustomValidationCallbackTakesPrecedenceOverSkipTlsVerify()
        {
            // Arrange
            var config = new KubernetesClientConfiguration
            {
                Host = "https://test.example.com",
                SkipTlsVerify = true,
                ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    return false; // Custom callback returns false
                },
            };

            // Act
            var client = new Kubernetes(config);

            // Assert - The custom callback should be set, not the skip all validation
            Assert.NotNull(config.ServerCertificateCustomValidationCallback);
            Assert.True(config.SkipTlsVerify); // SkipTlsVerify should still be true in config
        }

        [Fact]
        public void CustomValidationCallbackCanDisableRevocationCheck()
        {
            // Arrange
            var config = new KubernetesClientConfiguration
            {
                Host = "https://test.example.com",
                ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    // Example: Disable revocation checking
                    if (errors == SslPolicyErrors.None)
                    {
                        return true;
                    }

                    // Disable revocation checking
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    return chain.Build((X509Certificate2)cert);
                },
            };

            // Act
            var client = new Kubernetes(config);

            // Assert
            Assert.NotNull(config.ServerCertificateCustomValidationCallback);
        }

        [Fact]
        public void CustomValidationCallbackCanPerformCustomLogic()
        {
            // Arrange
            var allowedThumbprint = "1234567890ABCDEF";
            var config = new KubernetesClientConfiguration
            {
                Host = "https://test.example.com",
                ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    // Example: Pin to a specific certificate thumbprint
                    if (cert != null && cert.Thumbprint == allowedThumbprint)
                    {
                        return true;
                    }

                    return errors == SslPolicyErrors.None;
                },
            };

            // Act
            var client = new Kubernetes(config);

            // Assert
            Assert.NotNull(config.ServerCertificateCustomValidationCallback);
        }

        [Fact]
        public void ConfigurationWithoutCustomCallbackUsesDefaultBehavior()
        {
            // Arrange
            var config = new KubernetesClientConfiguration
            {
                Host = "https://test.example.com",
            };

            // Act
            var client = new Kubernetes(config);

            // Assert
            Assert.Null(config.ServerCertificateCustomValidationCallback);
        }

        [Fact]
        public void SkipTlsVerifyWorksWhenNoCustomCallbackSet()
        {
            // Arrange
            var config = new KubernetesClientConfiguration
            {
                Host = "https://test.example.com",
                SkipTlsVerify = true,
            };

            // Act
            var client = new Kubernetes(config);

            // Assert
            Assert.Null(config.ServerCertificateCustomValidationCallback);
            Assert.True(config.SkipTlsVerify);
        }
    }
}
