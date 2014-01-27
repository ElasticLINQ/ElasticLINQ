// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Test.Utility
{
    public class SpyMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage Request;
        public HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("") };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;

            return Task.FromResult(Response);
        }
    }
}