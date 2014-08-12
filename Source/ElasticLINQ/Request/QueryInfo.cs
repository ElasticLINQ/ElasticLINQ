using System;

namespace ElasticLinq.Request
{
    public class QueryInfo
    {
        private readonly string query;
        private readonly Uri uri;

        public QueryInfo(string query, Uri uri)
        {
            this.query = query;
            this.uri = uri;
        }

        public string Query { get { return query; } }

        public Uri Uri { get { return uri; } }
    }
}
