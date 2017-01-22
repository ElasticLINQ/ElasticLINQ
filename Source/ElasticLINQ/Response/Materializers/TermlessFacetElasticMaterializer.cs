// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a single termless facet from the response.
    /// </summary>
    class TermlessFacetElasticMaterializer : IElasticMaterializer
    {
        static readonly string[] termlessFacetTypes = { "statistical", "filter" };

        readonly Func<AggregateRow, object> projector;
        readonly Type elementType;
        readonly object key;

        /// <summary>
        /// Create an instance of the <see cref="TermlessFacetElasticMaterializer"/> with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired object.</param>
        /// <param name="elementType">The type of object being materialized.</param>
        /// <param name="key">The constant value for any key references during materialization.</param>
        public TermlessFacetElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, object key = null)
        {
            Argument.EnsureNotNull(nameof(projector), projector);
            Argument.EnsureNotNull(nameof(elementType), elementType);

            this.projector = projector;
            this.elementType = elementType;
            this.key = key;
        }

        /// <summary>
        /// Materialize a single object from the response using the <see cref="projector"/>
       ///  or return a default value based on the element type.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to materialize facets from.</param>
        /// <returns>Object materialized from the response using the projector or default if no corresponding facets.</returns>
        public virtual object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull(nameof(response), response);

            return MaterializeSingle(response) ?? TypeHelper.CreateDefault(elementType);
        }

        /// <summary>
        /// Materialize a single object from the response using the <see cref="projector"/>
        /// or return null if there are no applicable facets.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to materialize facets from.</param>
        /// <returns>Object materialized from the response using the projector or null if no corresponding facets.</returns>
        public object MaterializeSingle(ElasticResponse response)
        {
            Argument.EnsureNotNull(nameof(response), response);

            var facets = response.facets;
            if (facets == null || facets.Count <= 0) return null;

            var facetsWithoutTerms = facets
                .Values()
                .Where(x => termlessFacetTypes.Contains(x["_type"].ToString()))
                .ToList();

            return facetsWithoutTerms.Any()
                ? projector(new AggregateStatisticalRow(key, facets))
                : null;
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get { return elementType; } }
    }
}