// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a list containing a single termless facet.
    /// </summary>
    internal class ListTermlessFacetsElasticMaterializer : IElasticMaterializer
    {
        private static readonly MethodInfo manyMethodInfo = typeof(ListTermlessFacetsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);
        private static readonly string[] termlessFacetTypes = { "statistical", "filter" };

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;
        private readonly Object keyValue;

        /// <summary>
        /// Create an instance of the ListTermlessFacetsElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired CLR object.</param>
        /// <param name="elementType">The type of CLR object being materialized.</param>
        /// <param name="keyValue">The type of the term/key being materialized.</param>
        public ListTermlessFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, object keyValue)
        {
            this.projector = projector;
            this.elementType = elementType;
            this.keyValue = keyValue;
        }

        public object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull("response", response);

            var facets = response.facets;
            if (facets == null || facets.Count == 0)
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new[] { response.facets, projector, keyValue });
        }

        internal static List<T> Many<T>(JObject facets, Func<AggregateRow, object> projector, object key)
        {
            var facetsWithoutTerms = facets.Values()
                .Where(x => termlessFacetTypes.Contains(x["_type"].ToString()))
                .ToList();

            var results = new List<T>();

            if (facetsWithoutTerms.Any())
            {
                var result = projector(new AggregateStatisticalRow(key, facets));
                results.Add((T)result);
            }

            return results;
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}