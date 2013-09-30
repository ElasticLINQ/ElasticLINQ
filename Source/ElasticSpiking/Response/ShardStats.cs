﻿// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Diagnostics;

namespace IQToolkit.Data.ElasticSearch.Response
{
    [DebuggerDisplay("{failed} failed, {successful} success")]
    public class ShardStats
    {
        public int total;
        public int successful;
        public int failed;
    }
}