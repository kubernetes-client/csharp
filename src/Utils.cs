using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using k8s.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace k8s
{
    public static class Utils
    {
        /// <summary>
        ///     Encode string in base64 format.
        /// </summary>
        /// <param name="text">string to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public static string Base64Encode(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        ///     Encode string in base64 format.
        /// </summary>
        /// <param name="text">string to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public static string Base64Decode(string text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

        /// <summary>
        /// Load pem encoded cert file
        /// </summary>
        /// <param name="file">Path to pem encoded cert file</param>
        /// <returns>x509 instance.</returns>
        public static X509Certificate2 LoadPemFileCert(string file)
        {
            var certdata = File.ReadAllText(file)
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", "")
                .Replace("\r", "")
                .Replace("\n", "");

            return new X509Certificate2(Convert.FromBase64String(certdata));
        }

        /// <summary>
        /// Generates pfx from client configuration
        /// </summary>
        /// <param name="config">Kuberentes Client Configuration</param>
        /// <returns>Generated Pfx Path</returns>
        public static X509Certificate2 GeneratePfx(KubernetesClientConfiguration config)
        {
            byte[] keyData = null;
            byte[] certData = null;

            if (!string.IsNullOrWhiteSpace(config.ClientCertificateKeyData))
            {
                keyData = Convert.FromBase64String(config.ClientCertificateKeyData);
            }
            if (!string.IsNullOrWhiteSpace(config.ClientKeyFilePath))
            {
                keyData = File.ReadAllBytes(config.ClientKeyFilePath);
            }

            if (keyData == null)
            {
                throw new KubeConfigException("certData is empty");
            }

            if (!string.IsNullOrWhiteSpace(config.ClientCertificateData))
            {
                certData = Convert.FromBase64String(config.ClientCertificateData);
            }
            if (!string.IsNullOrWhiteSpace(config.ClientCertificateFilePath))
            {
                certData = File.ReadAllBytes(config.ClientCertificateFilePath);
            }

            if (certData == null)
            {
                throw new KubeConfigException("certData is empty");
            }

            var cert = new X509CertificateParser().ReadCertificate(new MemoryStream(certData));

            object obj;
            using (var reader = new StreamReader(new MemoryStream(keyData)))
            {
                obj = new PemReader(reader).ReadObject();
                var key = obj as AsymmetricCipherKeyPair;
                if (key != null)
                {
                    var cipherKey = key;
                    obj = cipherKey.Private;
                }
            }

            var rsaKeyParams = (RsaPrivateCrtKeyParameters) obj;

            var store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry("K8SKEY", new AsymmetricKeyEntry(rsaKeyParams), new[] {new X509CertificateEntry(cert)});

            using (var pkcs = new MemoryStream())
            {
                store.Save(pkcs, new char[0], new SecureRandom());
                return new X509Certificate2(pkcs.ToArray());
            }
        }
    }
}
