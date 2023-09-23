using k8s.Exceptions;
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
            using (var stream = FileSystem.Current.OpenRead(file))
            {
                certCollection.ImportFromPem(new StreamReader(stream).ReadToEnd());
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
                // This null password is to change the constructor to fix this KB:
                // https://support.microsoft.com/en-us/topic/kb5025823-change-in-how-net-applications-import-x-509-certificates-bf81c936-af2b-446e-9f7a-016f4713b46b
                string nullPassword = null;

                if (config.ClientCertificateKeyStoreFlags.HasValue)
                {
                    cert = new X509Certificate2(cert.Export(X509ContentType.Pkcs12), nullPassword, config.ClientCertificateKeyStoreFlags.Value);
                }
                else
                {
                    cert = new X509Certificate2(cert.Export(X509ContentType.Pkcs12), nullPassword);
                }
            }

            return cert;
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
