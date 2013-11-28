// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Forces basic authorization values, since negotiation requires an extra round-trip, which significantly
    /// increases the time required to search the index.
    /// </summary>
    internal class ForcedAuthHandler : DelegatingHandler
    {
        private readonly string userName;
        private readonly string password;

        public ForcedAuthHandler(string userName, string password, HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new WebRequestHandler())
        {
            this.userName = userName;
            this.password = password;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                var credentials = String.Format("{0}:{1}", userName, password);
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
