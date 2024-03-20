using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http;

namespace k8s
{
    /// <summary>
    /// Implements legacy Prometheus metrics
    /// </summary>
    /// <remarks>Provided for compatibility for existing usages of PrometheusHandler. It is recommended
    /// to transition to using OpenTelemetry and the default HttpClient metrics.
    ///
    /// Note that the tags/labels are not appropriately named for some metrics. This
    /// incorrect naming is retained to maintain compatibility and won't be fixed on this implementation.
    /// Use OpenTelemetry and the standard HttpClient metrics instead.</remarks>
    public class PrometheusHandler : DelegatingHandler
    {
        private const string Prefix = "k8s_dotnet";
        private static readonly Meter Meter = new Meter("k8s.dotnet");

        private static readonly Counter<int> RequestsSent = Meter.CreateCounter<int>(
            $"{Prefix}_request_total",
            description: "Number of requests sent by this client");

        private static readonly Histogram<double> RequestLatency = Meter.CreateHistogram<double>(
            $"{Prefix}_request_latency_seconds", unit: "milliseconds",
            description: "Latency of requests sent by this client");

        private static readonly Counter<int> ResponseCodes = Meter.CreateCounter<int>(
            $"{Prefix}_response_code_total",
            description: "Number of response codes received by the client");

        private static readonly UpDownCounter<int> ActiveRequests =
            Meter.CreateUpDownCounter<int>(
                $"{Prefix}_active_requests",
                description: "Number of requests currently in progress");

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var digest = KubernetesRequestDigest.Parse(request);
            var timer = Stopwatch.StartNew();
            // Note that this is a tag called "method" but the value is the Verb.
            // This is incorrect, but existing behavior.
            var methodWithVerbValue = new KeyValuePair<string, object>("method", digest.Verb);
            try
            {
                ActiveRequests.Add(1, methodWithVerbValue);
                RequestsSent.Add(1, methodWithVerbValue);

                var resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                ResponseCodes.Add(
                    1,
                    new KeyValuePair<string, object>("method", request.Method.ToString()),
                    new KeyValuePair<string, object>("code", (int)resp.StatusCode));
                return resp;
            }
            finally
            {
                timer.Stop();
                ActiveRequests.Add(-1, methodWithVerbValue);
                var tags = new TagList
                    {
                        { "verb", digest.Verb },
                        { "group", digest.ApiGroup },
                        { "version", digest.ApiVersion },
                        { "kind", digest.Kind },
                    }
                    ;
                RequestLatency.Record(timer.Elapsed.TotalMilliseconds, tags);
            }
        }
    }
}
