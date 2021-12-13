using Prometheus;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Monitoring
{
    public class PrometheusHandler : DelegatingHandler
    {
        private const string PREFIX = "k8s_dotnet";
        private readonly Counter requests = Metrics.CreateCounter(
            $"{PREFIX}_request_total", "Number of requests sent by this client",
            new CounterConfiguration
            {
                LabelNames = new[] { "method" },
            });

        private readonly Histogram requestLatency = Metrics.CreateHistogram(
            $"{PREFIX}_request_latency_seconds", "Latency of requests sent by this client",
            new HistogramConfiguration
            {
                LabelNames = new[] { "verb", "group", "version", "kind" },
            });

        private readonly Counter responseCodes = Metrics.CreateCounter(
            $"{PREFIX}_response_code_total", "Number of response codes received by the client",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "code" },
            });

        private readonly Gauge activeRequests = Metrics.CreateGauge(
            $"{PREFIX}_active_requests", "Number of requests currently in progress",
            new GaugeConfiguration
            {
                LabelNames = new[] { "method" },
            });

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var digest = KubernetesRequestDigest.Parse(request);
            requests.WithLabels(digest.Verb).Inc();
            using (activeRequests.WithLabels(digest.Verb).TrackInProgress())
            using (requestLatency.WithLabels(digest.Verb, digest.ApiGroup, digest.ApiVersion, digest.Kind).NewTimer())
            {
                var resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                responseCodes.WithLabels(request.Method.ToString(), ((int)resp.StatusCode).ToString()).Inc();
                return resp;
            }
        }
    }
}
