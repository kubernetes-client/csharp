using k8s.Models;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    public class WatcherTests
    {
        [Fact]
        public void ReadError()
        {
            var data = Encoding.UTF8.GetBytes(
                "{\"type\":\"ERROR\",\"object\":{\"kind\":\"Status\",\"apiVersion\":\"v1\",\"metadata\":{},\"status\":\"Failure\",\"message\":\"too old resource version: 44982(53593)\",\"reason\":\"Gone\",\"code\":410}}");

            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream))
            {
                Exception recordedException = null;
                var mre = new ManualResetEvent(false);

                var watcher = new Watcher<V1Pod>(
                    () => Task.FromResult(reader),
                    null,
                    (exception) =>
                    {
                        recordedException = exception;
                        mre.Set();
                    });

                mre.WaitOne();

                Assert.NotNull(recordedException);

                var k8sException = recordedException as KubernetesException;

                Assert.NotNull(k8sException);
                Assert.NotNull(k8sException.Status);
                Assert.Equal("too old resource version: 44982(53593)", k8sException.Message);
                Assert.Equal("too old resource version: 44982(53593)", k8sException.Status.Message);
            }
        }
    }
}
