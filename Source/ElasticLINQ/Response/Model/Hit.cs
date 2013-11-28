// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public Dictionary<String, JToken> fields = new Dictionary<string, JToken>();
    }
}