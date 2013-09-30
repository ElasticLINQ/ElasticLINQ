// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace IQToolkit.Data.ElasticSearch.Response
{
    [DebuggerDisplay("{meta.id}")]
    public class Source
    {
        public JObject doc;
        public Meta meta;
    }
}