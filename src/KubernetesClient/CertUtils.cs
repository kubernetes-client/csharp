using k8s.Exceptions;
#if !NET5_0_OR_GREATER
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
#endif
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace k8s
{
    internal static class CertUtils
    {
        /// <summary>
        /// Load pem encoded cert file
        /// </summary>
        /// <param name="file">Path to pem encoded cert file</param>
        /// <returns>List of x509 instances.</returns>
        public static X509Certificate2Collection LoadPemFileCert(string file)
        {
            var certCollection = new X509Certificate2Collection();
            using (var stream = FileUtils.FileSystem().File.OpenRead(file))
            {
#if NET5_0_OR_GREATER
                certCollection.ImportFromPem(new StreamReader(stream).ReadToEnd());
#else
                var certs = new X509CertificateParser().ReadCertificates(stream);

                // Convert BouncyCastle X509Certificates to the .NET cryptography implementation and add
                // it to the certificate collection
                //
                foreach (Org.BouncyCastle.X509.X509Certificate cert in certs)
                {
                    certCollection.Add(new X509Certificate2(cert.GetEncoded()));
                }
#endif
            }

            return certCollection;
        }

        /// <summary>
        /// Generates pfx from client configuration
        /// </summary>
        /// <param name="config">Kubernetes Client Configuration</param>
        /// <returns>Generated Pfx Path</returns>
        public static X509Certificate2 GeneratePfx(KubernetesClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

#if NET5_0_OR_GREATER
            string keyData = null;
            string certData = null;

            if (!string.IsNullOrWhiteSpace(config.ClientCertificateKeyData))
            {
                keyData = Encoding.UTF8.GetString(Convert.FromBase64String(config.ClientCertificateKeyData));
            }

            if (!string.IsNullOrWhiteSpace(config.ClientKeyFilePath))
            {
                keyData = File.ReadAllText(config.ClientKeyFilePath);
            }

            if (keyData == null)
            {
                throw new KubeConfigException("keyData is empty");
            }

            if (!string.IsNullOrWhiteSpace(config.ClientCertificateData))
            {
                certData = Encoding.UTF8.GetString(Convert.FromBase64String(config.ClientCertificateData));
            }

            if (!string.IsNullOrWhiteSpace(config.ClientCertificateFilePath))
            {
                certData = File.ReadAllText(config.ClientCertificateFilePath);
            }

            if (certData == null)
            {
                throw new KubeConfigException("certData is empty");
            }


            var cert = X509Certificate2.CreateFromPem(certData, keyData);

            // see https://github.com/kubernetes-client/csharp/issues/737
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cert = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
            }

            return cert;
#else

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
                throw new KubeConfigException("keyData is empty");
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
            // key usage is a bit string, zero-th bit is 'digitalSignature'
            // See https://www.alvestrand.no/objectid/2.5.29.15.html for more details.
            if (cert != null && cert.GetKeyUsage() != null && !cert.GetKeyUsage()[0])
            {
                throw new Exception(
                    "Client certificates must be marked for digital signing. " +
                    "See https://github.com/kubernetes-client/csharp/issues/319");
            }

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

            var keyParams = (AsymmetricKeyParameter)obj;

            var store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry("K8SKEY", new AsymmetricKeyEntry(keyParams), new[] { new X509CertificateEntry(cert) });

            using (var pkcs = new MemoryStream())
            {
                store.Save(pkcs, new char[0], new SecureRandom());

                if (config.ClientCertificateKeyStoreFlags.HasValue)
                {
                    return new X509Certificate2(pkcs.ToArray(), "", config.ClientCertificateKeyStoreFlags.Value);
                }
                else
                {
                    return new X509Certificate2(pkcs.ToArray());
                }
            }
#endif
        }

        /// <summary>
        /// Retrieves Client Certificate PFX from configuration
        /// </summary>
        /// <param name="config">Kubernetes Client Configuration</param>
        /// <returns>Client certificate PFX</returns>
        public static X509Certificate2 GetClientCert(KubernetesClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if ((!string.IsNullOrWhiteSpace(config.ClientCertificateData) ||
                 !string.IsNullOrWhiteSpace(config.ClientCertificateFilePath)) &&
                (!string.IsNullOrWhiteSpace(config.ClientCertificateKeyData) ||
                 !string.IsNullOrWhiteSpace(config.ClientKeyFilePath)))
            {
                return GeneratePfx(config);
            }

            return null;
        }
    }
}
