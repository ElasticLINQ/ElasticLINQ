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
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringCriteria"/> class.
        /// </summary>
        /// <param name="value">Value to be found within the fields.</param>
        /// <param name="fields">Colleciton of fields to be searched.</param>
        public QueryStringCriteria(string value, params string[] fields)
        {
            Argument.EnsureNotBlank(nameof(value), value);

            Value = value;
            Fields = new ReadOnlyCollection<string>(fields ?? new string[0]);
        }

        /// <summary>
        /// Collection of fields to be searched.
        /// </summary>
        public ReadOnlyCollection<string> Fields { get; }

        /// <summary>
        /// Value to be found within the fields.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public string Name { get { return "query_string"; } }
    }
}