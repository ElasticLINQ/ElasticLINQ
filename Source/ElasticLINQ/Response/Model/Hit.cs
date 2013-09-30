// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Individual hit response from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{_type} in {_index} id {_id}")]
    public class Hit
    {
        public string _index;
        public string _type;
        public string _id;
        public double? _score;

        public JObject _source;
        public JObject fields;
    }
}