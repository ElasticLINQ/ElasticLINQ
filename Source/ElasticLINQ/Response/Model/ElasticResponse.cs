// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Top-level response from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{hits.hits.Count} hits in {took} ms")]
    internal class ElasticResponse
    {
        public int took;
        public bool timed_out;
        public ShardStatistics _shards;
        public Hits hits;

        public JValue error;
        public HttpStatusCode status;

        public Dictionary<string, JToken> facets;
    }
}