// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Facets;
using System.Collections.Generic;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Represents a search request to be sent to ElasticSearch.
    /// </summary>
    internal class ElasticSearchRequest
    {
        public ElasticSearchRequest()
        {
            Fields = new List<string>();
            SortOptions = new List<SortOption>();
            Facets = new List<IFacet>();
        }

        public long @From { get; set; }
        public long? Size { get; set; }
        public string Type { get; set; }
        public List<string> Fields { get; set; }
        public List<SortOption> SortOptions { get; set; }
        public ICriteria Filter { get; set; }
        public ICriteria Query { get; set; }
        public List<IFacet> Facets { get; set; }
    }
}