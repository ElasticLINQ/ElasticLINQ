using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;

namespace ElasticLinq.IntegrationTest
{
    class BreakOnInvalidQueryConnection : ElasticConnection
    {
        public BreakOnInvalidQueryConnection(Uri endpoint, string userName = null, string password = null, TimeSpan? timeout = null, string index = null, ElasticConnectionOptions options = null)
            : base(endpoint, userName, password, timeout, index, options)
        {
        }

        public override async Task<ElasticResponse> SearchAsync(string body, SearchRequest searchRequest, CancellationToken token, ILog log)
        {
            try
            {
                return await base.SearchAsync(body, searchRequest, token, log);
            }
            catch (Exception)
            {
                var url = GetSearchUri(searchRequest);
                Debugger.Break();
                throw;
            }
        }
    }
}
