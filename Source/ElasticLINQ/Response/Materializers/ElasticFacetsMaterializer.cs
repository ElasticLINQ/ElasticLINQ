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
    internal class ElasticFacetsMaterializer : IElasticMaterializer
    {
        private static readonly MethodInfo manyMethodInfo = typeof(ElasticFacetsMaterializer).GetMethod("Many", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;

        public ElasticFacetsMaterializer(Func<AggregateRow, object> projector, Type elementType)
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


        private static readonly string[] termsFacetTypes = { "terms_stats", "terms" };

        internal static List<T> Many<T>(JObject facets, Func<AggregateRow, object> projector)
        {
            if (facets == null || facets.Count == 0)
                return new List<T>();

            var termsStats = facets.Values().Where(x => termsFacetTypes.Contains(x["_type"].ToString())).ToList();
            if (termsStats.Any())
                return FlattenTermsStatsToAggregateRows(termsStats).Select(projector).Cast<T>().ToList();

            var statistical = facets.Values().Where(x => x["_type"].ToString() == "statistical").ToList();
            if (statistical.Any())
                return FlattenStatisticalToAggregateRows(statistical).Select(projector).Cast<T>().ToList();

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
        internal static IEnumerable<AggregateRow> FlattenTermsStatsToAggregateRows(IEnumerable<JToken> termsStats)
        {
            return termsStats
                .SelectMany(t => t["terms"])
                .GroupBy(t => t["term"])
                .Select(g => new AggregateRow(g.Key,
                    g.SelectMany(v => v.Cast<JProperty>().Select(z => new AggregateField(((JProperty)v.Parent.Parent.Parent.Parent).Name, z.Name, z.Value)))));
        }

        /// <summary>
        /// statistical facets don't have terms - they are without a groupby. We map constant GroupBy's to this
        /// for efficiency and ease of use.
        /// </summary>
        /// <param name="statistical">Facets of type statistical.</param>
        /// <returns>An enumeration of AggregateRows containing appropriate keys and fields.</returns>
        internal static IEnumerable<AggregateRow> FlattenStatisticalToAggregateRows(IEnumerable<JToken> statistical)
        {
            var x = statistical
                .SelectMany(t => t.Values())
                .Select(s => new AggregateRow("", s.OfType<JProperty>().Select(z => new AggregateField(s.Parent.Parent.Path, z.Name, z.Value))))
                .ToList();

            return x;
        }
    }
}