using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public class TimeoutHandler : DelegatingHandler
    {
        private readonly TimeSpan _timeout;

        public TimeoutHandler(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var query = request.RequestUri.Query;
            var index = query.IndexOf("watch=true");
            var isWatch = index > 0 && (query[index - 1] == '&' || query[index - 1] == '?');

            if (isWatch)
            {
                var cts = new CancellationTokenSource(_timeout);
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token).Token;
            }

            var originResponse = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return originResponse;
        }
    }
}
