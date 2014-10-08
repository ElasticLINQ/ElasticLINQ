// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// A top-level response from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{hits.hits.Count} hits in {took} ms")]
    public class ElasticResponse
    {
        public int took;
        public bool timed_out;
        public Hits hits;

        public JValue error;
        public HttpStatusCode status;
        public JObject facets;
    }
}