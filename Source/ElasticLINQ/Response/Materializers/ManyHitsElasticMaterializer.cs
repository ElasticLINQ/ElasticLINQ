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
    /// Materializes multiple JSON hits into CLR objects.
    /// </summary>
    internal class ManyHitsElasticMaterializer : IElasticMaterializer
    {
        private static readonly MethodInfo manyMethodInfo = typeof(ManyHitsElasticMaterializer).GetMethodInfo(f => f.Name == "Many" && f.IsStatic);

        private readonly Func<Hit, object> projector;
        private readonly Type elementType;

        public ManyHitsElasticMaterializer(Func<Hit, object> projector, Type elementType)
        {
            this.projector = projector;
            this.elementType = elementType;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            Argument.EnsureNotNull("elasticResponse", elasticResponse);

            var hits = elasticResponse.hits;
            if (hits == null || hits.hits == null || !hits.hits.Any())
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { hits.hits, projector });
        }

        internal static List<T> Many<T>(IEnumerable<Hit> hits, Func<Hit, object> projector)
        {
            return hits.Select(projector).Cast<T>().ToList();
        }
    }
}