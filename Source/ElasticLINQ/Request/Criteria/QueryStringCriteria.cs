// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System.Collections.Generic;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a query string to be passed to ElasticSearch.
    /// </summary>
    public class QueryStringCriteria : ICriteria
    {
        private readonly string value;
        private readonly IReadOnlyList<string> fields;

        public QueryStringCriteria(string value, params string[] fields)
        {
            Argument.EnsureNotBlank("value", value);

            this.value = value;
            this.fields = fields ?? new string[0];
        }

        public IReadOnlyList<string> Fields
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