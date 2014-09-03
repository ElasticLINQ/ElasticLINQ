// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a single termless facet from the ElasticResponse as the
    /// desired CLR object.
    /// </summary>
    internal class TermlessFacetElasticMaterializer : IElasticMaterializer
    {
        private static readonly string[] validFacetTypes = { "statistical", "filter" };

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;

        /// <summary>
        /// Create an instance of the ListTermFacetsElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired CLR object.</param>
        /// <param name="elementType">The type of CLR object being materialized.</param>
        public TermlessFacetElasticMaterializer(Func<AggregateRow, object> projector, Type elementType)
        {
            this.projector = projector;
            this.elementType = elementType;
        }

        public object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull("response", response);

            var facets = response.facets;
            if (facets != null && facets.Count > 0)
            {
                var validFacets = facets
                    .Values()
                    .Where(x => validFacetTypes.Contains(x["_type"].ToString()))
                    .ToList();

                if (validFacets.Any())
                    return projector(new AggregateStatisticalRow(null, facets));
            }

            return TypeHelper.CreateDefault(elementType);
        }
    }
}