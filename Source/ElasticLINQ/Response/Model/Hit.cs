// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// An individual hit response from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{_type} in {_index} id {_id}")]
    public class Hit
    {
        /// <summary>
        /// The index of the document responsible for this hit.
        /// </summary>
        public string _index;

        /// <summary>
        /// The type of document used to create this hit.
        /// </summary>
        public string _type;

        /// <summary>
        /// Unique index of the document responsible for this hit.
        /// </summary>
        public string _id;

        /// <summary>
        /// The score this hit achieved based on the query criteria.
        /// </summary>
        public double? _score;

        /// <summary>
        /// Highlighting for this hit if highlighting was requested.
        /// </summary>
        public JObject highlight;

        /// <summary>
        /// The actual document for this hit (not supplied if fields requested).
        /// </summary>
        public JObject _source;

        /// <summary>
        /// The list of fields for this hit extracted from the document (if fields requested).
        /// </summary>
        public Dictionary<String, JToken> fields = new Dictionary<string, JToken>();
    }
}