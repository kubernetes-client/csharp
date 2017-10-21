using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace k8s.Tests.Mock
{
    public class MockKubeApiServer : IDisposable
    {
        // paste from minikube /api/v1/namespaces/default/pods
        public const string MockPodResponse =
                "{\r\n  \"kind\": \"PodList\",\r\n  \"apiVersion\": \"v1\",\r\n  \"metadata\": {\r\n    \"selfLink\": \"/api/v1/namespaces/default/pods\",\r\n    \"resourceVersion\": \"1762810\"\r\n  },\r\n  \"items\": [\r\n    {\r\n      \"metadata\": {\r\n        \"name\": \"nginx-1493591563-xb2v4\",\r\n        \"generateName\": \"nginx-1493591563-\",\r\n        \"namespace\": \"default\",\r\n        \"selfLink\": \"/api/v1/namespaces/default/pods/nginx-1493591563-xb2v4\",\r\n        \"uid\": \"ac1abb94-9c58-11e7-aaf5-00155d744505\",\r\n        \"resourceVersion\": \"1737928\",\r\n        \"creationTimestamp\": \"2017-09-18T10:03:51Z\",\r\n        \"labels\": {\r\n          \"app\": \"nginx\",\r\n          \"pod-template-hash\": \"1493591563\"\r\n        },\r\n        \"annotations\": {\r\n          \"kubernetes.io/created-by\": \"{\\\"kind\\\":\\\"SerializedReference\\\",\\\"apiVersion\\\":\\\"v1\\\",\\\"reference\\\":{\\\"kind\\\":\\\"ReplicaSet\\\",\\\"namespace\\\":\\\"default\\\",\\\"name\\\":\\\"nginx-1493591563\\\",\\\"uid\\\":\\\"ac013b63-9c58-11e7-aaf5-00155d744505\\\",\\\"apiVersion\\\":\\\"extensions\\\",\\\"resourceVersion\\\":\\\"5306\\\"}}\\n\"\r\n        },\r\n        \"ownerReferences\": [\r\n          {\r\n            \"apiVersion\": \"extensions/v1beta1\",\r\n            \"kind\": \"ReplicaSet\",\r\n            \"name\": \"nginx-1493591563\",\r\n            \"uid\": \"ac013b63-9c58-11e7-aaf5-00155d744505\",\r\n            \"controller\": true,\r\n            \"blockOwnerDeletion\": true\r\n          }\r\n        ]\r\n      },\r\n      \"spec\": {\r\n        \"volumes\": [\r\n          {\r\n            \"name\": \"default-token-3zzcj\",\r\n            \"secret\": {\r\n              \"secretName\": \"default-token-3zzcj\",\r\n              \"defaultMode\": 420\r\n            }\r\n          }\r\n        ],\r\n        \"containers\": [\r\n          {\r\n            \"name\": \"nginx\",\r\n            \"image\": \"nginx\",\r\n            \"resources\": {},\r\n            \"volumeMounts\": [\r\n              {\r\n                \"name\": \"default-token-3zzcj\",\r\n                \"readOnly\": true,\r\n                \"mountPath\": \"/var/run/secrets/kubernetes.io/serviceaccount\"\r\n              }\r\n            ],\r\n            \"terminationMessagePath\": \"/dev/termination-log\",\r\n            \"terminationMessagePolicy\": \"File\",\r\n            \"imagePullPolicy\": \"Always\"\r\n          }\r\n        ],\r\n        \"restartPolicy\": \"Always\",\r\n        \"terminationGracePeriodSeconds\": 30,\r\n        \"dnsPolicy\": \"ClusterFirst\",\r\n        \"serviceAccountName\": \"default\",\r\n        \"serviceAccount\": \"default\",\r\n        \"nodeName\": \"ubuntu\",\r\n        \"securityContext\": {},\r\n        \"schedulerName\": \"default-scheduler\"\r\n      },\r\n      \"status\": {\r\n        \"phase\": \"Running\",\r\n        \"conditions\": [\r\n          {\r\n            \"type\": \"Initialized\",\r\n            \"status\": \"True\",\r\n            \"lastProbeTime\": null,\r\n            \"lastTransitionTime\": \"2017-09-18T10:03:51Z\"\r\n          },\r\n          {\r\n            \"type\": \"Ready\",\r\n            \"status\": \"True\",\r\n            \"lastProbeTime\": null,\r\n            \"lastTransitionTime\": \"2017-10-12T07:09:21Z\"\r\n          },\r\n          {\r\n            \"type\": \"PodScheduled\",\r\n            \"status\": \"True\",\r\n            \"lastProbeTime\": null,\r\n            \"lastTransitionTime\": \"2017-09-18T10:03:51Z\"\r\n          }\r\n        ],\r\n        \"hostIP\": \"192.168.188.42\",\r\n        \"podIP\": \"172.17.0.5\",\r\n        \"startTime\": \"2017-09-18T10:03:51Z\",\r\n        \"containerStatuses\": [\r\n          {\r\n            \"name\": \"nginx\",\r\n            \"state\": {\r\n              \"running\": {\r\n                \"startedAt\": \"2017-10-12T07:09:20Z\"\r\n              }\r\n            },\r\n            \"lastState\": {\r\n              \"terminated\": {\r\n                \"exitCode\": 0,\r\n                \"reason\": \"Completed\",\r\n                \"startedAt\": \"2017-10-10T21:35:51Z\",\r\n                \"finishedAt\": \"2017-10-12T07:07:37Z\",\r\n                \"containerID\": \"docker://94df3f3965807421ad6dc76618e00b76cb15d024919c4946f3eb46a92659c62a\"\r\n              }\r\n            },\r\n            \"ready\": true,\r\n            \"restartCount\": 7,\r\n            \"image\": \"nginx:latest\",\r\n            \"imageID\": \"docker-pullable://nginx@sha256:004ac1d5e791e705f12a17c80d7bb1e8f7f01aa7dca7deee6e65a03465392072\",\r\n            \"containerID\": \"docker://fa11bdd48c9b7d3a6c4c3f9b6d7319743c3455ab8d00c57d59c083b319b88194\"\r\n          }\r\n        ],\r\n        \"qosClass\": \"BestEffort\"\r\n      }\r\n    }\r\n  ]\r\n}"
            ;

        private readonly IWebHost _webHost;

        public MockKubeApiServer(Func<HttpContext, bool> shouldNext = null, Action<ListenOptions> listenConfigure = null,
            string resp = MockPodResponse)
        {
            shouldNext = shouldNext ?? (_ => true);
            listenConfigure = listenConfigure ?? (_ => { });

            _webHost = WebHost.CreateDefaultBuilder()
                .Configure(app => app.Run(httpContext =>
                {
                    if (shouldNext(httpContext))
                    {
                        httpContext.Response.WriteAsync(resp);
                    }

                    return Task.Delay(0);
                }))
                .UseKestrel(options => { options.Listen(IPAddress.Loopback, 0, listenConfigure); })
                .Build();

            _webHost.Start();
        }

        public Uri Uri => _webHost.ServerFeatures.Get<IServerAddressesFeature>().Addresses
            .Select(a => new Uri(a)).First();

        public void Dispose()
        {
            _webHost.StopAsync();
            _webHost.WaitForShutdown();
        }
    }
}