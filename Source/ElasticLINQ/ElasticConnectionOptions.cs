// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq
{
    /// <summary>
    /// Connection options that can be specified to control how <see cref="ElasticContext"/> communicates with
    /// Elasticsearch.
    /// </summary>
    public class ElasticConnectionOptions
    {
        /// <summary>
        /// Whether the JSON should be prettified to make it more human-readable.
        /// </summary>
        /// <remarks>Defaults to false.</remarks>
        public bool Pretty { get; set; }

        /// <summary>
        /// The default size for searches to specify the maximum document count.
        /// </summary>
        /// <remarks>Defaults to null, resulting in Elasticseach defaulting to 10.</remarks>
        public long? SearchSizeDefault { get; set; }
    }
}