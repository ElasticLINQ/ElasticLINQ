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
    /// Materializes facets with their terms from the ElasticResponse.
    /// </summary>
    class ListTermFacetsElasticMaterializer : IElasticMaterializer
    {
        static readonly MethodInfo manyMethodInfo = typeof(ListTermFacetsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && !f.IsStatic);
        static readonly string[] termsFacetTypes = { "terms_stats", "terms" };

        readonly Func<AggregateRow, object> projector;
        readonly Type elementType;
        readonly Type groupKeyType;

        /// <summary>
        /// Create an instance of the ListTermFacetsElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired CLR object.</param>
        /// <param name="elementType">The type of CLR object being materialized.</param>
        /// <param name="groupKeyType">The type of the term/group key field.</param>
        public ListTermFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, Type groupKeyType)
        {
            Argument.EnsureNotNull("projector", projector);
            Argument.EnsureNotNull("elementType", elementType);
            Argument.EnsureNotNull("groupKeyType", groupKeyType);

            this.projector = projector;
            this.elementType = elementType;
            this.groupKeyType = groupKeyType;
        }

        /// <summary>
        /// Materialize the facets from an ElasticResponse into a List of CLR objects as determined
        /// by the projector.
        /// </summary>
        /// <param name="response">ElasticResponse to obtain the facets from.</param>
        /// <returns>List of CLR objects with these facets projected onto them.</returns>
        public object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull("response", response);

            var facets = response.facets;
            if (facets == null || facets.Count == 0)
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(this, new object[] { response.facets });
        }

        /// <summary>
        /// Given a JObject of facets in an Elasticsearch structure materialize them as the
        /// desired CLR objects determined by the projector.
        /// </summary>
        /// <typeparam name="T">Type of CLR objects to be materialized.</typeparam>
        /// <param name="facets">Elasticsearch formatted list of facets.</param>
        /// <returns>List of materialized CLR objects using the projector.</returns>
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
        /// <returns>An enumeration of AggregateRows containing appropriate keys and fields.</returns>
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
        internal Type ElementType { get { return elementType; } }
    }
}