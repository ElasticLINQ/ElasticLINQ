// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Facet response from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{_type,nq} Facet")]
    internal class Facet
    {
        public string _type { get; set; }
        public long count { get; set; }
        public decimal total { get; set; }
        public decimal min { get; set; }
        public decimal max { get; set; }
        public decimal mean { get; set; }
    }
}