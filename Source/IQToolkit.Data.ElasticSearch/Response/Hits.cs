// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;
using System.Diagnostics;

namespace IQToolkit.Data.ElasticSearch.Response
{
    [DebuggerDisplay("{hits.Count} hits of {total}")]
    public class Hits
    {
        public long total;
        public double max_score;
        public List<Hit> hits;
    }
}
