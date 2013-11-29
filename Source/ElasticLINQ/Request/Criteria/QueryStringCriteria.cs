// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a query string to be passed to ElasticSearch.
    /// </summary>
    public class QueryStringCriteria : ICriteria
    {
        private readonly string value;

        public QueryStringCriteria(string value)
        {
            Argument.EnsureNotBlank("value", value);
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public string Name
        {
            get { return "query_string"; }
        }
    }
}