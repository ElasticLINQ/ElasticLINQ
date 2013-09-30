// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Diagnostics;

namespace IQToolkit.Data.ElasticSearch.Response
{
    [DebuggerDisplay("{id} rev {rev}")]
    public class Meta
    {
        public string id;
        public string rev;
        public int? expiration;
        public int? flags;
    }
}