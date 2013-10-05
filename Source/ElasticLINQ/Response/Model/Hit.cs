// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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