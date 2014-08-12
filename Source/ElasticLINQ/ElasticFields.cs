// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;

namespace ElasticLinq
{
    /// <summary>
    /// Provides properties that stand in for special fields in Elasticsearch.
    /// </summary>
    public static class ElasticFields
    {
        private static readonly Exception exception = new InvalidOperationException("This property is for mapping queries to Elasticsearch and should not be evaluated directly.");

        /// <summary>
        /// A property that stands in for the Elasticsearch _score field.
        /// </summary>
        public static double Score
        {
            get { throw exception; } 
        }

        /// <summary>
        /// A property that stands in for the Elasticsearch _id field.
        /// </summary>
        public static string Id
        {
            get { throw exception; }
        }
    }
}