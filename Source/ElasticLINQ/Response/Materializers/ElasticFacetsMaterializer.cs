// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple facet requests from the ElasticResponse.
    /// </summary>
    internal class ElasticFacetsMaterializer : IElasticMaterializer
    {
        private readonly Func<AggregateRow, object> itemCreator;
        private readonly Type elementType;

        public ElasticFacetsMaterializer(Func<AggregateRow, object> itemCreator, Type elementType)
        {
            this.itemCreator = itemCreator;
            this.elementType = elementType;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            var rows = FlattenToAggregateRows(elasticResponse.facets);
            return rows.Select(itemCreator).ToList(); // TOOD: Cast dynamically with reflection
        }

        /// <summary>
        /// The results from ElasticSearch for facets come back in a structure where each facet
        /// is completely independent from each other and contains all possible operations for
        /// that facets. Terms may also be present in one facet but not another. This query
        /// converts that structure into something more row-based that allows the rebinding of the
        /// select projection to it.
        /// </summary>
        /// <param name="facets">Facets JSON object as returned from ElasticSearch.</param>
        /// <returns>An enumeration of AggregateRows containing appropriate keys and fields.</returns>
        internal static IEnumerable<AggregateRow> FlattenToAggregateRows(JObject facets)
        {
            return facets
                .Values()
                .SelectMany(x => x["terms"])
                .GroupBy(x => x["term"])
                .Select(c => new AggregateRow(c.Key, c.SelectMany(v => v.Cast<JProperty>()
                .Select(z => new AggregateField(new AggregateColumn(((JProperty)v.Parent.Parent.Parent.Parent).Name, z.Name), z.Value)))
            ));
        }
    }
}