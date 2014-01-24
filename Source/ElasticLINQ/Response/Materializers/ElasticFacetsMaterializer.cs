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
                .Invoke(null, new object[] { elasticResponse, projector });
        }

        internal static List<T> Many<T>(ElasticResponse elasticResponse, Func<AggregateRow, object> projector)
        {
            if (elasticResponse.facets == null || elasticResponse.facets.Count == 0)
                return new List<T>();

            var rows = FlattenToAggregateRows(elasticResponse.facets);
            return rows.Select(projector).Cast<T>().ToList();
        }

        /// <summary>
        /// A facet response from ElasticSearch comes back with each field in an independent
        /// block containing all possible aggregates for that field. Multiple fields requires
        /// multiple blocks each of which may or may not have all possible terms.
        /// This query converts that structure into something row-based that allows rebinding
        /// the select projection to it.
        /// </summary>
        /// <param name="facets">Facets JSON object as returned from ElasticSearch.</param>
        /// <returns>An enumeration of AggregateRows containing appropriate keys and fields.</returns>
        internal static IEnumerable<AggregateRow> FlattenToAggregateRows(JObject facets)
        {
            return facets
                .Values()
                .SelectMany(x => x["terms"])
                .GroupBy(x => x["term"])
                .Select(CreateRow)
                .ToList();
        }

        private static AggregateRow CreateRow(IGrouping<JToken, JToken> c)
        {
            return new AggregateRow(c.Key, c.SelectMany(v => v.Cast<JProperty>().Select(z => AggregateField(v, z))));
        }

        private static AggregateField AggregateField(JToken v, JProperty z)
        {
            return new AggregateField(CreateColumn(v, z), z.Value);
        }

        private static AggregateColumn CreateColumn(JToken v, JProperty z)
        {
            return new AggregateColumn(((JProperty)v.Parent.Parent.Parent.Parent).Name, z.Name);
        }
    }
}