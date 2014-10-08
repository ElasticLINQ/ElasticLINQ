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
        private static readonly string[] termlessFacetTypes = { "statistical", "filter" };

        private readonly Func<AggregateRow, object> projector;
        private readonly Type elementType;
        private readonly object key;

        /// <summary>
        /// Create an instance of the TermlessFacetElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired CLR object.</param>
        /// <param name="elementType">The type of CLR object being materialized.</param>
        /// <param name="key">The constant value for any key references during materialization.</param>
        public TermlessFacetElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, object key = null)
        {
            Argument.EnsureNotNull("projector", projector);
            Argument.EnsureNotNull("elementType", elementType);

            this.projector = projector;
            this.elementType = elementType;
            this.key = key;
        }

        /// <summary>
        /// Materialize a single CLR object from the ElasticResponse using the projector or
        /// return a default value based on the element type.
        /// </summary>
        /// <param name="response">ElasticResponse received from Elasticsearch.</param>
        /// <returns>CLR object materialized from the response using the projector or default if no corresponding facets.</returns>
        public virtual object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull("response", response);

            return MaterializeSingle(response) ?? TypeHelper.CreateDefault(elementType);
        }

        /// <summary>
        /// Materialize a single CLR object from the ElasticResponse using the projector
        /// or return null if there are no applicable facets.
        /// </summary>
        /// <param name="response">ElasticResponse received from Elasticsearch.</param>
        /// <returns>CLR object materialized from the response using the projector or null if no corresponding facets.</returns>
        public object MaterializeSingle(ElasticResponse response)
        {
            Argument.EnsureNotNull("response", response);

            var facets = response.facets;
            if (facets != null && facets.Count > 0)
            {
                var facetsWithoutTerms = facets
                    .Values()
                    .Where(x => termlessFacetTypes.Contains(x["_type"].ToString()))
                    .ToList();

                if (facetsWithoutTerms.Any())
                    return projector(new AggregateStatisticalRow(key, facets));
            }

            return null;
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}