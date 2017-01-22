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
    /// Materializes facets with their terms from the response.
    /// </summary>
    class ListTermFacetsElasticMaterializer : IElasticMaterializer
    {
        static readonly MethodInfo manyMethodInfo = typeof(ListTermFacetsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && !f.IsStatic);
        static readonly string[] termsFacetTypes = { "terms_stats", "terms" };

        readonly Func<AggregateRow, object> projector;
        readonly Type groupKeyType;

        /// <summary>
        /// Create an instance of the <see cref="ListTermFacetsElasticMaterializer"/> with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired object.</param>
        /// <param name="elementType">The type of object being materialized.</param>
        /// <param name="groupKeyType">The type of the term/group key field.</param>
        public ListTermFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, Type groupKeyType)
        {
            Argument.EnsureNotNull(nameof(projector), projector);
            Argument.EnsureNotNull(nameof(elementType), elementType);
            Argument.EnsureNotNull(nameof(groupKeyType), groupKeyType);

            this.projector = projector;
            ElementType = elementType;
            this.groupKeyType = groupKeyType;
        }

        /// <summary>
        /// Materialize the facets from an response into a list of objects.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> containing the facets to materialize.</param>
        /// <returns>List of <see cref="ElementType"/> objects with these facets projected onto them.</returns>
        public object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull(nameof(response), response);

            var facets = response.facets;
            if (facets == null || facets.Count == 0)
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(ElementType));

            return manyMethodInfo
                .MakeGenericMethod(ElementType)
                .Invoke(this, new object[] { response.facets });
        }

        /// <summary>
        /// Given a JObject of facets in an Elasticsearch structure materialize them as
        /// objects of type <typeparamref name="T"/> as created by the <see cref="projector"/>.
        /// </summary>
        /// <typeparam name="T">Type of objects to be materialized.</typeparam>
        /// <param name="facets">Elasticsearch formatted list of facets.</param>
        /// <returns>List of materialized <typeparamref name="T"/> objects.</returns>
        internal List<T> Many<T>(JObject facets)
        {
            var termFacetsValues = facets.Values()
                .Where(x => termsFacetTypes.Contains(x["_type"].ToString()))
                .ToList();

            return termFacetsValues.Any()
                ? FlattenTermsToAggregateRows(termFacetsValues).Select(projector).Cast<T>().ToList()
                : new List<T>();
        }

        /// <summary>
        /// terms_stats and terms facet responses have each field in an independent object with all 
        /// possible operations for that field. Multiple fields means multiple objects
        /// each of which might not have all possible terms. Convert that structure into
        /// an SQL-style row with one term per row containing each aggregate field and operation combination.
        /// </summary>
        /// <param name="termsStats">Facets of type terms or terms_stats.</param>
        /// <returns>An <see cref="IEnumerable{AggregateRow}"/> containing keys and fields representing 
        /// the terms and statistics.</returns>
        internal IEnumerable<AggregateTermRow> FlattenTermsToAggregateRows(IEnumerable<JToken> termsStats)
        {
            return termsStats
                .SelectMany(t => t["terms"])
                .GroupBy(t => t["term"])
                .Select(g => new AggregateTermRow(AggregateRow.ParseValue(g.Key, groupKeyType), g.SelectMany(CreateAggregateFields)));
        }

        static IEnumerable<AggregateField> CreateAggregateFields(JToken termFields)
        {
            var name = ((JProperty)termFields.Parent.Parent.Parent.Parent).Name;

            return termFields
                .Cast<JProperty>()
                .Where(z => z.Name != "term")
                .Select(z => new AggregateField(name, z.Name, z.Value));
        }

        /// <summary>
        /// Type of element being materialized.
        /// </summary>
        internal Type ElementType { get; }
    }
}