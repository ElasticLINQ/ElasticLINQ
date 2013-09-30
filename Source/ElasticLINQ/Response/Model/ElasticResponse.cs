// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Top-level response from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{hits.hits.Count} hits in {took} ms")]
    public class ElasticResponse
    {
        public int took;
        public bool timed_out;
        public ShardStatistics _shards;
        public Hits hits;

        public JValue error;
        public HttpStatusCode status;
    }
}