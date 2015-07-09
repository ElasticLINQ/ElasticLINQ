using System;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Provides information about a query to be sent to Elasticsearch.
    /// </summary>
    public class QueryInfo
    {
        readonly string query;
        readonly Uri uri;

        /// <summary>
        /// Create a new instance of the QueryInfo class with a given query and Uri.
        /// </summary>
        /// <param name="query">Query body to be sent to Elasticsearch.</param>
        /// <param name="uri">Uri to be used to send the query to Elasticsearch.</param>
        internal QueryInfo(string query, Uri uri)
        {
            this.query = query;
            this.uri = uri;
        }

        /// <summary>
        /// Query JSON body to be sent to Elasticsearch.
        /// </summary>
        public string Query { get { return query; } }

        /// <summary>
        /// Uri to be used to send the query to Elasticsearch.
        /// </summary>
        public Uri Uri { get { return uri; } }
    }
}
