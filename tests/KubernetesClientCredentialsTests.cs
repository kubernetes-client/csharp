using System;
using Xunit;
using k8s;
using System.IO;

namespace k8s.Tests
{
    public class KubernetesClientCredentialsTests
    {
        /// <summary>
        /// Checks that a ArgumentNullException is thrown when trying to create a KubernetesClientCredentials with null token
        /// </summary>
        [Fact]
        public void TokenNull() 
        {
            Assert.Throws<ArgumentNullException>(() => new KubernetesClientCredentials(null));
        }

        /// <summary>
        /// Checks that a ArgumentNullException is thrown when trying to create a KubernetesClientCredentials with null username
        /// </summary>
        [Fact]
        public void UsernameNull() 
        {
            Assert.Throws<ArgumentNullException>(() => new KubernetesClientCredentials(null,"password"));
        }

        /// <summary>
        /// Checks that a ArgumentNullException is thrown when trying to create a KubernetesClientCredentials with null password
        /// </summary>
        [Fact]
        public void PasswordNull() 
        {
            Assert.Throws<ArgumentNullException>(() => new KubernetesClientCredentials("username", null));
        }

        /// <summary>
        /// Checks that the Token is set with no exceptions
        /// </summary>
        [Fact]
        public void ValidTokenIsSet() 
        {
            var token = "mytoken";
            var credentials = new KubernetesClientCredentials(token);
            Assert.NotNull(credentials);
        }

        /// <summary>
        ///  Checks that the Username and Password is set with no exceptions
        /// </summary>
        [Fact]
        public void ValidUserPasswordIsSet()
        {
            var username = "myuser";
            var password = "mypassword";
            var credentials = new KubernetesClientCredentials(username, password);
            Assert.NotNull(credentials);        
        }
    }
}