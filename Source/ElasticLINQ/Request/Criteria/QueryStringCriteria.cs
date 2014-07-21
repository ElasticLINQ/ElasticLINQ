// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a query string to be passed to ElasticSearch.
    /// </summary>
    public class QueryStringCriteria : ICriteria
    {
        private readonly string value;
        private readonly IEnumerable<string> fields = Enumerable.Empty<string>();

        public QueryStringCriteria(string value)
        {
            Argument.EnsureNotBlank("value", value);
            this.value = value;
        }

        public QueryStringCriteria(string value, IEnumerable<string> fields)
            : this(value)
        {
            this.fields = fields;
        }

        public IEnumerable<string> Fields
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