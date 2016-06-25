// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a list containing a single termless facet.
    /// </summary>
    class ListTermlessFacetsElasticMaterializer : TermlessFacetElasticMaterializer
    {
        static readonly MethodInfo manyMethodInfo = typeof(ListTermlessFacetsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);

        /// <summary>
        /// Create an instance of the ListTermlessFacetsElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired object.</param>
        /// <param name="elementType">The type of object being materialized.</param>
        /// <param name="key">The constant value for any key references during materialization.</param>
        public ListTermlessFacetsElasticMaterializer(Func<AggregateRow, object> projector, Type elementType, object key)
            : base(projector, elementType, key)
        {
        }

        /// <summary>
        /// Materialize the facets from an response into a List with a single object as determined
        /// by the projector.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to obtain the facets from.</param>
        /// <returns>List containing a single object with these facets projected onto them.</returns>
        public override object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull(nameof(response), response);

            var element = MaterializeSingle(response);
            var listType = typeof(List<>).MakeGenericType(ElementType);

            return element == null
                ? Activator.CreateInstance(listType)
                : manyMethodInfo.MakeGenericMethod(ElementType).Invoke(null, new[] { element });
        }

        /// <summary>
        /// Converts a single item into a generic list containing that item.
        /// </summary>
        /// <typeparam name="T">Type of the item and corresponding type of list.</typeparam>
        /// <param name="item">Item to be included in the new list.</param>
        /// <returns>New list containing just the item.</returns>
        internal static List<T> Many<T>(T item)
        {
            return new List<T> { item };
        }
    }
}