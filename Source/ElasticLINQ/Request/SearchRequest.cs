// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System.Collections.Generic;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Represents a search request to be sent to Elasticsearch.
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Create a new SearchRequest.
        /// </summary>
        public SearchRequest()
        {
            Fields = new List<string>();
            SortOptions = new List<SortOption>();
        }

        /// <summary>
        /// Index to start taking the Elasticsearch documents from.
        /// </summary>
        /// <remarks>Maps to the Skip statement of LINQ.</remarks>
        public long @From { get; set; }

        /// <summary>
        /// Number of documents to return from Elasticsearch.
        /// </summary>
        /// <remarks>Maps to the Take statement of LINQ.</remarks>       
        public long? Size { get; set; }

        /// <summary>
        /// Type of documents to return from Elasticsearch.
        /// </summary>
        /// <remarks>Derived from the T specified in Query&lt;T&gt;.</remarks>
        public string DocumentType { get; set; }

        /// <summary>
        /// List of fields to return for each document instead of the
        /// </summary>
        public List<string> Fields { get; set; }
        
        /// <summary>
        /// Sort sequence for the documents. This affects From and Size.
        /// </summary>
        /// <remarks>Determined by the OrderBy/ThenBy LINQ statements.</remarks>
        public List<SortOption> SortOptions { get; set; }
        
        /// <summary>
        /// Filter criteria for the documents.
        /// </summary>
        /// <remarks>Determined by the Where LINQ statements.</remarks>
        public ICriteria Filter { get; set; }
        
        /// <summary>
        /// Query criteria for the documents.
        /// </summary>
        /// <remarks>Determined by the Query extension methods.</remarks>
        public ICriteria Query { get; set; }
                
        /// <summary>
        /// Type of search Elasticsearch should perform.
        /// </summary>
        /// <remarks>Is usually blank but can be set to Count when facets are required instead of hits.</remarks>
        public string SearchType { get; set; }

        /// <summary>
        /// Minimum score of results to be returned.
        /// </summary>
        public double? MinScore { get; set; }

        /// <summary>
        /// Specify the highlighting to be applied to the results.
        /// </summary>
        public Highlight Highlight { get; set; }
    }
}