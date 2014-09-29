// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a query string to be passed to Elasticsearch.
    /// </summary>
    public class QueryStringCriteria : ICriteria
    {
        private readonly string value;
        private readonly ReadOnlyCollection<string> fields;

        public QueryStringCriteria(string value, params string[] fields)
        {
            Argument.EnsureNotBlank("value", value);

            this.value = value;
            this.fields = new ReadOnlyCollection<string>(fields ?? new string[0]);
        }

        public ReadOnlyCollection<string> Fields
        {
            get { return fields; }
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