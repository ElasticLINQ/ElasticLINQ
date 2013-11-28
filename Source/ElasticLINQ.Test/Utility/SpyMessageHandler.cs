using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Test
{
    public class SpyMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage Request;
        public HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("") };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;

            return Task.FromResult(Response);
        }
    }
}
