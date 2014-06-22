// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple facet requests from the ElasticResponse.
    /// </summary>
    internal class ManyFacetsElasticMaterializer : IElasticMaterializer
    {
        private static readonly string[] termsFacetTypes = { "terms_stats", "terms" };
        private static readonly string[] termlessFacetTypes = { "statistical", "filter" };

        private static readonly MethodInfo manyMethodInfo =
            typeof(ManyFacetsElasticMaterializer).GetMethod("Many", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;

        public ManyFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType)
        {
            this.projector = projector;
            this.elementType = elementType;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { elasticResponse.facets, projector });
        }

        internal static List<T> Many<T>(JObject facets, Func<AggregateRow, object> projector)
        {
            if (facets != null && facets.Count != 0)
            {
                var termsStats = facets.Values().Where(x => termsFacetTypes.Contains(x["_type"].ToString())).ToList();
                if (termsStats.Any())
                    return FlattenTermsStatsToAggregateRows(termsStats).Select(projector).Cast<T>().ToList();

                var termlessStats = facets.Values().Where(x => termlessFacetTypes.Contains(x["_type"].ToString())).ToList();
                if (termlessStats.Any())
                    return Enumerable.Range(1, 1)
                        .Select(r => new AggregateStatisticalRow(facets))
                        .Select(projector)
                        .Cast<T>()
                        .ToList();
            }

            return new List<T>();
        }

        /// <summary>
        /// terms_stats and terms facet responses have each field in an independent object with all 
        /// possible operations for that field. Multiple fields means multiple objects
        /// each of which might not have all possible terms. Convert that structure into
        /// a SQL-style row with one term per row containing each aggregate field and operation combination.
        /// </summary>
        /// <param name="termsStats">Facets of type terms or terms_stats.</param>
        /// <returns>An enumeration of AggregateRows containing appropriate keys and fields.</returns>
        internal static IEnumerable<AggregateTermRow> FlattenTermsStatsToAggregateRows(IEnumerable<JToken> termsStats)
        {
            return termsStats
                .SelectMany(t => t["terms"])
                .GroupBy(t => t["term"])
                .Select(g => new AggregateTermRow((g.Key.ToObject<object>()),
                    g.SelectMany(v => v.Cast<JProperty>().Select(z =>
                        new AggregateField(((JProperty)v.Parent.Parent.Parent.Parent).Name, z.Name, z.Value)))));
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}