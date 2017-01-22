// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple hits into a list of CLR objects.
    /// </summary>
    class ListHitsElasticMaterializer : IElasticMaterializer
    {
        static readonly MethodInfo manyMethodInfo = typeof(ListHitsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);

        readonly Func<Hit, object> projector;
        readonly Type elementType;

        /// <summary>
        /// Create an instance of the ListHitsElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired CLR object.</param>
        /// <param name="elementType">The type of CLR object being materialized.</param>
        public ListHitsElasticMaterializer(Func<Hit, object> projector, Type elementType)
        {
            this.projector = projector;
            this.elementType = elementType;
        }

        /// <summary>
        /// Materialize the hits from the response into desired CLR objects.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> containing the hits to materialize.</param>
        /// <returns>List of <see cref="elementType"/> objects as constructed by the <see cref="projector"/>.</returns>
        public object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull(nameof(response), response);

            var hits = response.hits;
            if (hits?.hits == null || !hits.hits.Any())
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { hits.hits, projector });
        }

        internal static IReadOnlyList<T> Many<T>(IEnumerable<Hit> hits, Func<Hit, object> projector)
        {
            return hits.Select(projector).Cast<T>().ToReadOnlyBatchedList();
        }
    }
}