// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// A container of hit responses from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{hits.Count} hits of {total}")]
    public class Hits
    {
        /// <summary>
        /// The total number of hits available on the server.
        /// </summary>
        public long total;

        /// <summary>
        /// The highest score of a hit for the given query.
        /// </summary>
        public double? max_score;

        /// <summary>
        /// The list of hits received from the server.
        /// </summary>
        public List<Hit> hits;
    }
}