// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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