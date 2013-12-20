// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple ElasticSearch hits into the required
    /// C# list of objects.
    /// </summary>
    internal class ElasticManyHitsMaterializer : IElasticMaterializer
    {
        private static readonly MethodInfo manyMethodInfo = typeof(ElasticManyHitsMaterializer).GetMethod("Many", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly Func<Hit, object> itemCreator;
        private readonly Type elementType;

        public ElasticManyHitsMaterializer(Func<Hit, object> itemCreator, Type elementType)
        {
            this.itemCreator = itemCreator;
            this.elementType = elementType ?? itemCreator.Method.ReturnType;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            return manyMethodInfo
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { elasticResponse.hits.hits, itemCreator });
        }

        internal static List<T> Many<T>(IEnumerable<Hit> hits, Func<Hit, object> projector)
        {
            return hits.Select(projector).Cast<T>().ToList();
        }
    }
}