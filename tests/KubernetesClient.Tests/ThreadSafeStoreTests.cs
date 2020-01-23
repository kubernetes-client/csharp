using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.cache;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class ThreadSafeStoreTests
    {
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(150);

        private readonly ITestOutputHelper testOutput;

        public ThreadSafeStoreTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public void Simple()
        {
            var store = new ThreadSafeStore();
            var kObject = new KubernetesObject();

            var key = "default/kind4";
            var apiVersion = "v4";
            var kind = "kind4";
            kObject.ApiVersion = apiVersion;
            kObject.Kind = kind;

            // Simple add test
            store.Add(key, kObject);
            var val = store.Get(key);
            Assert.Equal(val.ApiVersion, apiVersion);
            Assert.Equal(val.Kind, kind);

            // Add again which should raise and exception
            var raisedExcection = false;
            try
            {
                store.Add(key, kObject);
            }
            catch (Exception)
            {
                raisedExcection = true;
            }
            Assert.True(raisedExcection);
            val = store.Get(key);
            Assert.Equal(val.ApiVersion, apiVersion);
            Assert.Equal(val.Kind, kind);

            // Update existing key
            var newApiVersion = "v4update";
            var newKind = "kind4update";
            var newKObject = new KubernetesObject();
            newKObject.ApiVersion = newApiVersion;
            newKObject.Kind = newKind;

            var oldVal = store.Update(key, newKObject);
            // Check if the object matches the new key.
            val = store.Get(key);
            Assert.Equal(val.ApiVersion, newApiVersion);
            Assert.Equal(val.Kind, newKind);
            // Check the old value
            Assert.Equal(oldVal.ApiVersion, apiVersion);
            Assert.Equal(oldVal.Kind, kind);

            key = "non existant key";
            store.Update(key, kObject);
            val = store.Get(key);
            // Check if the updated value (by add) is correct.
            Assert.Equal(val.ApiVersion, apiVersion);
            Assert.Equal(val.Kind, kind);

            // Delete
            store.Delete(key);
            val = store.Get(key);
            Assert.Null(val);

            raisedExcection = false;
            // Delete again 
            try
            {
                store.Delete(key);
            }
            catch (Exception)
            {
                raisedExcection = true;
            }

            val = store.Get(key);
            Assert.Null(val);
        }

        [Fact]
        public void MultiThread()
        {
            var store = new ThreadSafeStore();
            Action<object> action = (object o) =>
            {
                var i = (int)o;
                var key = string.Format("{0}", i);

                var k = new KubernetesObject();
                k.ApiVersion = "apiV" + key;
                k.Kind = "kind" + key;
                store.Add(key, k);
            };

            int max = 10;
            var tasks = new Task[max];
            var dict = new Dictionary<string, bool>();

            for (int i = 0; i < max; i++)
            {
                dict.Add(i.ToString(), false);
                tasks[i] = Task.Factory.StartNew(action, i);
            }
            Task.WaitAll(tasks);


            int count = 0;
            foreach (string key in store.ListKeys())
            {
                // Ensure we have not seen the key before.
                Assert.False(dict[key] == true);
                dict[key] = true;
                count++;
                var val = store.Get(key);
                Assert.NotNull(val);
                Assert.Equal(val.ApiVersion, "apiV" + key);
                Assert.Equal(val.Kind, "kind" + key);
            }
            Assert.Equal(count, max);
        }

    }
}
