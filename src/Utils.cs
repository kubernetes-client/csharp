namespace k8s
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.OpenSsl;

    public static class Utils
    {
        /// <summary>
        /// Encode string in base64 format.
        /// </summary>
        /// <param name="text">string to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public static string Base64Encode(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Encode string in base64 format.
        /// </summary>
        /// <param name="text">string to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public static string Base64Decode(string text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

        /// <summary>
        /// Generates pfx from client configuration
        /// </summary>
        /// <param name="config">Kuberentes Client Configuration</param>
        /// <returns>Generated Pfx Path</returns>
        /// TODO: kabhishek8260 Remplace the method with X509 Certificate with private key(in dotnet 2.0)
        public static async Task<X509Certificate2> GeneratePfxAsync(KubernetesClientConfiguration config)
        {
            var keyData = new byte[]{};
            var certData = new byte[]{};

            var filePrefix = config.CurrentContext;
            if (!string.IsNullOrWhiteSpace(config.ClientCertificateKey))
            {
                keyData = Convert.FromBase64String(config.ClientCertificateKey);
            }
            if (!string.IsNullOrWhiteSpace(config.ClientKey))
            {
                keyData = File.ReadAllBytes(config.ClientKey);
            }

            if (!string.IsNullOrWhiteSpace(config.ClientCertificateData))
            {
                certData = Convert.FromBase64String(config.ClientCertificateData);
            }
            if (!string.IsNullOrWhiteSpace(config.ClientCertificate))
            {
                certData = File.ReadAllBytes(config.ClientCertificate);
            }

            var cert = new X509Certificate2(certData);
            return addPrivateKey(cert, keyData);
        }

        public static X509Certificate2 addPrivateKey(X509Certificate2 cert, byte[] keyData)
        {
            using (var reader = new StreamReader(new MemoryStream(keyData)))
            {
                var cipherKey = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
                var rsaKeyParams = (RsaPrivateCrtKeyParameters)cipherKey.Private;
                var rsaKey = RSA.Create(DotNetUtilities.ToRSAParameters(rsaKeyParams));
                return cert.CopyWithPrivateKey(rsaKey);
            }
        }
    }
}
