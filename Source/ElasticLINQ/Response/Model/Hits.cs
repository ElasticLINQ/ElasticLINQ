// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Diagnostics;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Container of hit responses from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{hits.Count} hits of {total}")]
    public class Hits
    {
        public long total;
        public double? max_score;
        public List<Hit> hits;
    }
}
