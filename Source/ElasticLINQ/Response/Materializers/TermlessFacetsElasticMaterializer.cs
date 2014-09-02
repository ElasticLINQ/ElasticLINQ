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
    /// Materializes facets without terms from the ElasticResponse.
    /// </summary>
    internal class TermlessFacetsElasticMaterializer : IElasticMaterializer
    {
        private static readonly MethodInfo manyMethodInfo = typeof(TermlessFacetsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);
        private static readonly string[] termlessFacetTypes = { "statistical", "filter" };

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;
        private readonly Object keyValue;

        public TermlessFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, object keyValue)
        {
            this.projector = projector;
            this.elementType = elementType;
            this.keyValue = keyValue;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            Argument.EnsureNotNull("elasticResponse", elasticResponse);

            var facets = elasticResponse.facets;
            if (facets == null || facets.Count == 0)
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new[] { elasticResponse.facets, projector, keyValue });
        }

        internal static List<T> Many<T>(JObject facets, Func<AggregateRow, object> projector, object key)
        {
            var facetValues = facets.Values().ToList();

            var facetsWithoutTerms = facetValues.Where(x => termlessFacetTypes.Contains(x["_type"].ToString())).ToList();
            return facetsWithoutTerms.Any()
                ? Enumerable.Range(1, 1)
                    .Select(r => new AggregateStatisticalRow(key, facets))
                    .Select(projector)
                    .Cast<T>()
                    .ToList()
                : new List<T>();
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}