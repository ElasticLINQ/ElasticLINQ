// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Diagnostics;

namespace IQToolkit.Data.ElasticSearch.Response
{
    [DebuggerDisplay("{hits.hits.Count} hits in {took} ms")]
    public class ElasticResponse
    {
        public int took;
        public bool timed_out;
        public ShardStats _shards;
        public Hits hits;
    }
}