// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;

namespace ElasticLinq
{
    /// <summary>
    /// Provides methods that stand in for additional operations available in Elasticsearch.
    /// </summary>
    public static class ElasticMethods
    {
        private static readonly Exception exception = new InvalidOperationException("This method is for mapping queries to Elasticsearch and should not be called directly.");

        /// <summary>
        /// Determines whether a sequence contains any of the specified items.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence in which to locate one of the items.</param>
        /// <param name="items">A sequence containing the items to be located.</param>
        /// <returns>true if the source sequence contains any of the items; otherwise, false.</returns>
        public static bool ContainsAny<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> items)
        {
            throw exception;
        }

        /// <summary>
        /// Determines whether a sequence contains all of the specified items.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence in which to locate all of the items.</param>
        /// <param name="items">A sequence containing all the items to be located.</param>
        /// <returns>true if the source sequence contains all of the items; otherwise, false.</returns>
        public static bool ContainsAll<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> items)
        {
            throw exception;
        }

        /// <summary>
        /// Specifies a regular expression term query for a field.
        /// </summary>
        /// <param name="field">Field name to be matched.</param>
        /// <param name="regexp">Regular expression to be matched against the field.</param>
        /// <returns>true if the regular expression matches the field startsWith; otherwise, false.</returns>
        public static bool Regexp(string field, string regexp)
        {
            throw exception;
        }

        /// <summary>
        /// Specifies a regular expression term query for a field.
        /// </summary>
        /// <param name="field">Field name to be matched.</param>
        /// <param name="startsWith">String the field must start with to match.</param>
        /// <returns>true if the field starts with the startsWith; otherwise, false.</returns>
        public static bool Prefix(string field, string startsWith)
        {
            throw exception;
        }
    }
}