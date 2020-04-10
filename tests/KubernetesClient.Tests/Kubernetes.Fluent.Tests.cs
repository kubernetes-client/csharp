using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using k8s.Models;
using k8s.Tests.Mock;
using Newtonsoft.Json;
using Xunit;

namespace k8s.Tests
{
    public class FluentTests
    {
        [Fact]
        public void TestRequestProperties()
        {
            var testScheme = new KubernetesScheme();
            var r = new KubernetesRequest(new Uri("http://somewhere"), scheme: testScheme);

            // verify the initial values
            Assert.Equal("application/json", r.Accept());
            Assert.Null(r.Body());
            Assert.False(r.DryRun());
            Assert.Null(r.FieldManager());
            Assert.Null(r.FieldSelector());
            Assert.Null(r.Group());
            Assert.Null(r.LabelSelector());
            Assert.Equal("application/json", r.MediaType());
            Assert.Same(HttpMethod.Get, r.Method());
            Assert.Null(r.Name());
            Assert.Null(r.Namespace());
            Assert.Null(r.RawUri());
            Assert.Same(testScheme, r.Scheme());
            Assert.False(r.StreamResponse());
            Assert.Null(r.Type());
            Assert.Null(r.Version());
            Assert.Null(r.WatchVersion());

            // test basic value setters
            r.Accept("foo/bar")
                .Body("abc")
                .DryRun(true)
                .FieldManager("joe")
                .FieldSelector("fs")
                .Group("mygroup")
                .LabelSelector("x=y")
                .MediaType("bar/baz")
                .Method(HttpMethod.Post)
                .Name("name")
                .Namespace("ns")
                .RawUri("/anywhere")
                .StreamResponse(true)
                .Subresource("exec")
                .Type("mytype")
                .Version("v2")
                .WatchVersion("42");
            Assert.Equal("foo/bar", r.Accept());
            Assert.Equal("abc", (string)r.Body());
            Assert.True(r.DryRun());
            Assert.Equal("joe", r.FieldManager());
            Assert.Equal("fs", r.FieldSelector());
            Assert.Equal("mygroup", r.Group());
            Assert.Equal("x=y", r.LabelSelector());
            Assert.Equal("bar/baz", r.MediaType());
            Assert.Same(HttpMethod.Post, r.Method());
            Assert.Equal("name", r.Name());
            Assert.Equal("ns", r.Namespace());
            Assert.Equal("/anywhere", r.RawUri());
            Assert.True(r.StreamResponse());
            Assert.Equal("mytype", r.Type());
            Assert.Equal("v2", r.Version());
            Assert.Equal("42", r.WatchVersion());

            // test value normalization
            r.Accept("")
                .FieldManager("")
                .FieldSelector("")
                .Group("")
                .LabelSelector("")
                .MediaType("")
                .Method(null)
                .Name("")
                .Namespace("")
                .RawUri("")
                .Scheme(null)
                .Subresource("")
                .Type("")
                .Version("")
                .WatchVersion("");
            Assert.Null(r.Accept());
            Assert.Null(r.FieldManager());
            Assert.Null(r.FieldSelector());
            Assert.Null(r.Group());
            Assert.Null(r.LabelSelector());
            Assert.Null(r.MediaType());
            Assert.Same(HttpMethod.Get, r.Method());
            Assert.Null(r.Name());
            Assert.Null(r.Namespace());
            Assert.Null(r.RawUri());
            Assert.Same(KubernetesScheme.Default, r.Scheme());
            Assert.Null(r.Type());
            Assert.Null(r.Version());
            Assert.Equal("", r.WatchVersion());

            // test exceptions from property setters
            Assert.Throws<ArgumentException>(() => r.RawUri("foo"));

            // test more advanced/specific setters
            r.Delete();
            Assert.Same(HttpMethod.Delete, r.Method());
            r.Get();
            Assert.Same(HttpMethod.Get, r.Method());
            r.Patch();
            Assert.Equal(new HttpMethod("PATCH"), r.Method());
            r.Post();
            Assert.Same(HttpMethod.Post, r.Method());
            r.Put();
            Assert.Same(HttpMethod.Put, r.Method());

            r.Status();
            Assert.Equal("status", r.Subresource());
            r.Subresources("a", "b c");
            Assert.Equal("a/b%20c", r.Subresource());

            r.GVK<CustomNew>();
            Assert.Equal("v3", r.Version());
            Assert.Equal("cgrp", r.Group());
            Assert.Equal("newz", r.Type());

            r.GVK<CustomOld>();
            Assert.Equal("v0", r.Version());
            Assert.Equal("ogrp", r.Group());
            Assert.Equal("nos", r.Type());

            r.GVK("grp", "v1", "PigList");
            Assert.Equal("v1", r.Version());
            Assert.Equal("grp", r.Group());
            Assert.Equal("pigs", r.Type());

            r.GVK(new CustomOld());
            Assert.Equal("v0", r.Version());
            Assert.Equal("ogrp", r.Group());
            Assert.Equal("nos", r.Type());

            var res = new CustomNew() { ApiVersion = "coolstuff/v7", Kind = "yep", Metadata = new V1ObjectMeta() { Name = "name", NamespaceProperty = "ns" } };
            r.GVK(res);
            Assert.Equal("v7", r.Version());
            Assert.Equal("coolstuff", r.Group());
            Assert.Equal("newz", r.Type());
            Assert.Null(r.Name());
            Assert.Null(r.Namespace());

            res.ApiVersion = "v7";
            r.Body(null).Set(res, setBody: false);
            Assert.Equal("v7", r.Version());
            Assert.Null(r.Group());
            Assert.Equal("newz", r.Type());
            Assert.Null(r.Name());
            Assert.Equal("ns", r.Namespace());
            Assert.Null(r.Body());

            (res.ApiVersion, res.Metadata.Uid) = ("", "id");
            r.Set(res);
            Assert.Equal("v3", r.Version());
            Assert.Equal("cgrp", r.Group());
            Assert.Equal("newz", r.Type());
            Assert.Equal("name", r.Name());
            Assert.Equal("ns", r.Namespace());
            Assert.Same(res, r.Body());

            // test with a custom scheme
            var scheme = new KubernetesScheme();
            scheme.SetGVK(typeof(CustomOld), "group", "version", "Custom", "customs");
            r.Scheme(scheme).GVK<CustomOld>();
            Assert.Equal("group", r.Group());
            Assert.Equal("version", r.Version());
            Assert.Equal("customs", r.Type());
        }

        [Fact]
        public void TestRequestQuery()
        {
            var r = new KubernetesRequest(new Uri("http://somewhere"));

            // test basic query-string operations
            Assert.Null(r.GetQuery("k"));
            r.AddQuery("k", "y");
            Assert.Equal("y", r.GetQuery("k"));
            r.AddQuery("k", "x");
            Assert.Throws<InvalidOperationException>(() => r.GetQuery("k"));
            Assert.Equal(new[] { "y", "x" }, r.GetQueryValues("k"));
            r.SetQuery("k", "z");
            Assert.Equal("z", r.GetQuery("k"));
            r.SetQuery("k2", "a").ClearQuery("k");
            Assert.Null(r.GetQuery("k"));
            Assert.Equal("a", r.GetQuery("k2"));
            r.ClearQuery();
            Assert.Null(r.GetQuery("k2"));
            r.AddQuery("k", "a", "b");
            Assert.Equal(new[] { "a", "b" }, r.GetQueryValues("k"));
            r.SetQuery("k", "x");
            Assert.Equal("x", r.GetQuery("k"));
            r.SetQuery("k", null);
            Assert.Null(r.GetQuery("k"));

            // test property setters that work via the query string
            r.DryRun(true).FieldManager("fm").FieldSelector("fs").LabelSelector("ls");
            Assert.Equal("All", r.GetQuery("dryRun"));
            Assert.Equal("fm", r.GetQuery("fieldManager"));
            Assert.Equal("fs", r.GetQuery("fieldSelector"));
            Assert.Equal("ls", r.GetQuery("labelSelector"));
        }

        [Fact]
        public void TestRequestHeaders()
        {
            var r = new KubernetesRequest(new Uri("http://somewhere"));

            // test basic header operations
            Assert.Null(r.GetHeader("k"));
            r.AddHeader("k", "y");
            Assert.Equal("y", r.GetHeader("k"));
            r.AddHeader("k", "x");
            Assert.Throws<InvalidOperationException>(() => r.GetHeader("k"));
            Assert.Equal(new[] { "y", "x" }, r.GetHeaderValues("k"));
            r.SetHeader("k", "z");
            Assert.Equal("z", r.GetHeader("k"));
            r.SetHeader("k2", "a").ClearHeader("k");
            Assert.Null(r.GetHeader("k"));
            Assert.Equal("a", r.GetHeader("k2"));
            r.ClearHeaders();
            Assert.Null(r.GetHeader("k2"));
            r.AddHeader("k", "a", "b");
            Assert.Equal(new[] { "a", "b" }, r.GetHeaderValues("k"));
            r.SetHeader("k", "x");
            Assert.Equal("x", r.GetHeader("k"));
            r.SetHeader("k", null);
            Assert.Null(r.GetHeader("k"));

            // test exceptions
            Assert.Null(r.GetHeader("Accept"));
            Assert.Throws<ArgumentException>(() => r.AddHeader("Accept", "text/plain"));
            Assert.Throws<ArgumentException>(() => r.AddHeader("Accept", Enumerable.Repeat("text/plain", 2)));
            Assert.Throws<ArgumentException>(() => r.AddHeader("Accept", "text/plain", "text/fancy"));
            Assert.Throws<ArgumentException>(() => r.ClearHeader("Content-Type"));
            Assert.Throws<ArgumentException>(() => r.SetHeader("Content-Type", "text/plain"));
        }

        [Fact]
        public async Task TestExecution()
        {
            var h = new MockHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"apiVersion\":\"xyz\"}") });
            var c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080" }, new HttpClient(h));

            await c.Request<V1Pod>().AddHeader("Test", "yes").AddQuery("x", "a", "b c").Body("hello").ExecuteAsync();
            Assert.Equal(HttpMethod.Get, h.Request.Method);
            Assert.Equal(new Uri("http://localhost:8080/api/v1/pods?x=a&x=b%20c"), h.Request.RequestUri);
            Assert.Equal("yes", h.Request.Headers.GetValues("Test").Single());
            Assert.Equal("application/json", h.Request.Headers.Accept.ToString());
            Assert.Equal("application/json; charset=UTF-8", h.Request.Content.Headers.ContentType.ToString());
            Assert.Equal("hello", await h.Request.Content.ReadAsStringAsync());

            var res = new CustomNew() { ApiVersion = "abc" };
            await c.Request<CustomNew>("ns", "name")
                .Accept("text/plain").MediaType("text/rtf").Delete().DryRun(true).Body(res).Status().ExecuteAsync();
            Assert.Equal(HttpMethod.Delete, h.Request.Method);
            Assert.Equal(new Uri("http://localhost:8080/apis/cgrp/v3/namespaces/ns/newz/name/status?dryRun=All"), h.Request.RequestUri);
            Assert.Equal("text/plain", h.Request.Headers.Accept.ToString());
            Assert.Equal("text/rtf; charset=UTF-8", h.Request.Content.Headers.ContentType.ToString());
            Assert.Equal("{\"apiVersion\":\"abc\"}", await h.Request.Content.ReadAsStringAsync());

            await c.Request().RawUri("/foobar").Post().LabelSelector("ls").WatchVersion("3").Body(Encoding.UTF8.GetBytes("bytes")).ExecuteAsync();
            Assert.Equal(HttpMethod.Post, h.Request.Method);
            Assert.Equal(new Uri("http://localhost:8080/foobar?labelSelector=ls&watch=1&resourceVersion=3"), h.Request.RequestUri);
            Assert.Equal("bytes", await h.Request.Content.ReadAsStringAsync());

            await c.Request().RawUri("/foobar/").WatchVersion("").Body(new MemoryStream(Encoding.UTF8.GetBytes("streaming"))).ExecuteAsync();
            Assert.Equal(new Uri("http://localhost:8080/foobar/?watch=1"), h.Request.RequestUri);
            Assert.Equal("streaming", await h.Request.Content.ReadAsStringAsync());

            await Assert.ThrowsAsync<InvalidOperationException>(() => c.Request().Name("x").RawUri("/y").ExecuteAsync()); // can't use raw and non-raw

            c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080", AccessToken = "token" }, new HttpClient(h));
            await c.Request().ExecuteAsync();
            Assert.Equal(new Uri("http://localhost:8080/api/v1/"), h.Request.RequestUri);
            Assert.Equal("Bearer token", h.Request.Headers.Authorization.ToString());

            c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080", Username = "joe" }, new HttpClient(h));
            await c.Request().ExecuteAsync();
            Assert.Equal("Basic am9lOg==", h.Request.Headers.Authorization.ToString());

            res = await c.Request().ExecuteAsync<CustomNew>();
            Assert.Equal("xyz", res.ApiVersion);

            h = new MockHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("") });
            c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080" }, new HttpClient(h));
            res = await c.Request().ExecuteAsync<CustomNew>();
            Assert.Null(res);
            res = await c.Request().ExecuteAsync<CustomNew>(failIfMissing: true); // missing only refers to 404 Not Found
            Assert.Null(res);

            h = new MockHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("{}") });
            c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080" }, new HttpClient(h));
            res = await c.Request().ExecuteAsync<CustomNew>();
            Assert.Null(res);
            await Assert.ThrowsAsync<KubernetesException>(() => c.Request().ExecuteAsync<CustomNew>(failIfMissing: true));

            h = new MockHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("{}") });
            c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080" }, new HttpClient(h));
            await Assert.ThrowsAsync<KubernetesException>(() => c.Request().ExecuteAsync<CustomNew>());
        }

        [Fact]
        public void TestNewRequest()
        {
            var c = new Kubernetes(new Uri("http://somewhere"), new Microsoft.Rest.TokenCredentials("token"), new MockHttpHandler(null));
            c.Scheme = new KubernetesScheme();
            c.Scheme.SetGVK(typeof(CustomOld), "group", "version", "Custom", "customs");

            // test c.Request(HttpMethod = null)
            var r = c.Request();
            Assert.Same(HttpMethod.Get, r.Method());
            r = c.Request(HttpMethod.Delete);
            Assert.Same(HttpMethod.Delete, r.Method());

            // test c.Request(Type)
            r = c.Request(typeof(V1MutatingWebhookConfiguration));
            Assert.Equal("admissionregistration.k8s.io", r.Group());
            Assert.Equal("v1", r.Version());
            Assert.Equal("mutatingwebhookconfigurations", r.Type());
            r = c.Request(typeof(CustomOld)); // ensure it defaults to the scheme from the client
            Assert.Same(c.Scheme, r.Scheme());
            Assert.Equal("group", r.Group());
            Assert.Equal("version", r.Version());
            Assert.Equal("customs", r.Type());

            // test c.Request(obj, bool)
            var res = new CustomNew() { ApiVersion = "coolstuff/v7", Kind = "yep", Metadata = new V1ObjectMeta() { Name = "name", NamespaceProperty = "ns" } };
            r = c.Request(res);
            Assert.Equal("coolstuff", r.Group());
            Assert.Equal("v7", r.Version());
            Assert.Equal("newz", r.Type());
            Assert.Null(r.Name());
            Assert.Equal("ns", r.Namespace());
            Assert.Same(res, r.Body());

            res.Metadata.Uid = "id";
            r = c.Request(res, setBody: false);
            Assert.Equal("name", r.Name());
            Assert.Null(r.Body());

            // test c.Request(HttpMethod, Type, string, string)
            r = c.Request(null, typeof(V1PodList), "ns", "name");
            Assert.Same(HttpMethod.Get, r.Method());
            Assert.Null(r.Group());
            Assert.Equal("v1", r.Version());
            Assert.Equal("pods", r.Type());
            Assert.Equal("name", r.Name());
            Assert.Equal("ns", r.Namespace());
            r = c.Request(HttpMethod.Delete, typeof(V1PodList), "ns", "name");
            Assert.Same(HttpMethod.Delete, r.Method());

            // test c.Request(HttpMethod, string, string, string, string, string)
            r = c.Request(HttpMethod.Put, "type", "ns", "name", "group", "version");
            Assert.Same(HttpMethod.Put, r.Method());
            Assert.Equal("type", r.Type());
            Assert.Equal("ns", r.Namespace());
            Assert.Equal("name", r.Name());
            Assert.Equal("group", r.Group());
            Assert.Equal("version", r.Version());

            // test c.Request<T>(string, string)
            c.Scheme = KubernetesScheme.Default;
            r = c.Request<CustomOld>("ns", "name");
            Assert.Equal("v0", r.Version());
            Assert.Equal("ogrp", r.Group());
            Assert.Equal("nos", r.Type());
            Assert.Equal("ns", r.Namespace());
            Assert.Equal("name", r.Name());
        }

        [Fact]
        public async Task TestReplace()
        {
            string value = "{}";
            bool conflict = true;
            var h = new MockHttpHandler(req =>
            {
                if(value == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                else if(req.Method == HttpMethod.Get)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(value) };
                }
                else if(req.Method == HttpMethod.Put)
                {
                    if(conflict)
                    {
                        conflict = false;
                        return new HttpResponseMessage(HttpStatusCode.Conflict);
                    }
                    value = req.Content.ReadAsStringAsync().Result;
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(value) };
                }
                throw new Exception("i shouldn't exist");
            });
            var c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost:8080" }, new HttpClient(h));

            int i = 0;
            var res = await c.Request().ReplaceAsync<CustomNew>(r => { r.SetAnnotation("a", (i++).ToString(CultureInfo.InvariantCulture)); return true; });
            Assert.Equal("1", res.GetAnnotation("a"));

            res = await c.Request().ReplaceAsync<CustomNew>(r => { r.SetAnnotation("b", "x"); return true; });
            Assert.Equal("1", res.GetAnnotation("a"));
            Assert.Equal("x", res.GetAnnotation("b"));

            res = await c.Request().ReplaceAsync<CustomNew>(r => { r.SetAnnotation("c", "y"); return false; });
            Assert.Equal("x", res.GetAnnotation("b"));
            Assert.Equal("y", res.GetAnnotation("c"));

            res = await c.Request().ReplaceAsync<CustomNew>(r => false);
            Assert.Equal("x", res.GetAnnotation("b"));
            Assert.Null(res.GetAnnotation("c"));

            value = null;
            res = await c.Request().ReplaceAsync<CustomNew>(r => { r.SetAnnotation("a", "x"); return true; });
            Assert.Null(res);
        }

        [Fact]
        public async Task TestResponse()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.Ambiguous) { Content = new StringContent("{\"ApiVersion\":\"123\"}") };
            var resp = new KubernetesResponse(msg);
            Assert.False(resp.IsError);
            Assert.False(resp.IsNotFound);
            Assert.Same(msg, resp.Message);
            Assert.Equal(HttpStatusCode.Ambiguous, resp.StatusCode);
            Assert.Equal("{\"ApiVersion\":\"123\"}", await resp.GetBodyAsync());
            var cn = await resp.GetBodyAsync<CustomNew>();
            Assert.Equal("123", cn.ApiVersion);
            var status = await resp.GetStatusAsync();
            Assert.Equal("Success", status.Status);
            Assert.Equal((int)resp.StatusCode, status.Code.Value);
            Assert.Equal("{\"ApiVersion\":\"123\"}", status.Message);

            msg = new HttpResponseMessage(HttpStatusCode.NotFound);
            resp = new KubernetesResponse(msg);
            Assert.True(resp.IsError);
            Assert.True(resp.IsNotFound);
            Assert.Equal("", await resp.GetBodyAsync());
            Assert.Null(await resp.GetBodyAsync<CustomNew>());
            await Assert.ThrowsAsync<InvalidOperationException>(() => resp.GetBodyAsync<CustomNew>(failIfEmpty: true));
            status = await resp.GetStatusAsync();
            Assert.Equal("Failure", status.Status);
            Assert.Equal((int)resp.StatusCode, status.Code.Value);
            Assert.Equal("", status.Message);

            msg = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("It's bad, yo.") };
            resp = new KubernetesResponse(msg);
            Assert.True(resp.IsError);
            Assert.False(resp.IsNotFound);
            Assert.Equal("It's bad, yo.", await resp.GetBodyAsync());
            await Assert.ThrowsAnyAsync<JsonException>(() => resp.GetBodyAsync<CustomNew>());
            status = await resp.GetStatusAsync();
            Assert.Equal("Failure", status.Status);
            Assert.Equal((int)resp.StatusCode, status.Code.Value);
            Assert.Equal("It's bad, yo.", status.Message);
        }

        [KubernetesEntity(ApiVersion = "v3", Group = "cgrp", Kind = "yes", PluralName = "newz")]
        class CustomNew : IKubernetesObject<V1ObjectMeta>
        {
            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public V1ObjectMeta Metadata { get; set; }
        }

        class CustomOld : IKubernetesObject<V1ObjectMeta>
        {
            public const string KubeApiVersion = "v0", KubeGroup = "ogrp", KubeKind = "no";
            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public V1ObjectMeta Metadata { get; set; }
        }
    }
}
