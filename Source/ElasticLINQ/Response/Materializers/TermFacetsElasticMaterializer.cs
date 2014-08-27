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
    /// Materializes facets with terms from the ElasticResponse.
    /// </summary>
    internal class TermFacetsElasticMaterializer : IElasticMaterializer
    {
        private static readonly MethodInfo manyMethodInfo = typeof(TermFacetsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);
        private static readonly string[] termsFacetTypes = { "terms_stats", "terms" };

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;
        private readonly Type groupKeyType;

        public TermFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, Type groupKeyType)
        {
            this.projector = projector;
            this.elementType = elementType;
            this.groupKeyType = groupKeyType;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            Argument.EnsureNotNull("elasticResponse", elasticResponse);

            var facets = elasticResponse.facets;
            if (facets == null || facets.Count == 0)
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { elasticResponse.facets, projector, groupKeyType });
        }

        internal static List<T> Many<T>(JObject facets, Func<AggregateRow, object> projector, Type groupType)
        {
            var facetValues = facets.Values().ToList();

            var facetsWithTerms = facetValues.Where(x => termsFacetTypes.Contains(x["_type"].ToString())).ToList();
            return facetsWithTerms.Any()
                ? FlattenTermsStatsToAggregateRows(facetsWithTerms, groupType).Select(projector).Cast<T>().ToList()
                : new List<T>();
        }

        /// <summary>
        /// terms_stats and terms facet responses have each field in an independent object with all 
        /// possible operations for that field. Multiple fields means multiple objects
        /// each of which might not have all possible terms. Convert that structure into
        /// a SQL-style row with one term per row containing each aggregate field and operation combination.
        /// </summary>
        /// <param name="termsStats">Facets of type terms or terms_stats.</param>
        /// <param name="groupKeyType">Type of the group key property.</param>
        /// <returns>An enumeration of AggregateRows containing appropriate keys and fields.</returns>
        internal static IEnumerable<AggregateTermRow> FlattenTermsStatsToAggregateRows(IEnumerable<JToken> termsStats, Type groupKeyType)
        {
            return termsStats
                .SelectMany(t => t["terms"])
                .GroupBy(t => t["term"])
                .Select(g => new AggregateTermRow((g.Key.ToObject(groupKeyType)),
                    g.SelectMany(v => v.Cast<JProperty>().Select(z =>
                        new AggregateField(((JProperty)v.Parent.Parent.Parent.Parent).Name, z.Name, z.Value)))));
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}