// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Diagnostics;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Shard statistics response from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{failed} failed, {successful} success")]
    public class ShardStatistics
    {
        public int total;
        public int successful;
        public int failed;
    }
}