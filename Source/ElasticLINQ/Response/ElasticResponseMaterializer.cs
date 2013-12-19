// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Response
{
    /// <summary>
    /// Materializes responses from ElasticSearch.
    /// </summary>
    public static class ElasticResponseMaterializer
    {
        private static readonly MethodInfo materializer = typeof(ElasticResponseMaterializer).GetMethod("Many", BindingFlags.NonPublic | BindingFlags.Static);

        public static IList Many(IEnumerable<Hit> hits, Type elementType, Func<Hit, object> projector)
        {
            return (IList)materializer
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { hits, projector });
        }

        internal static List<T> Many<T>(IEnumerable<Hit> hits, Func<Hit, object> projector)
        {
            return hits.Select(projector).Cast<T>().ToList();
        }

        internal static object First(IEnumerable hits, Func<Hit, object> projector, bool defaultIfNone, Type elementType)
        {
            var enumerator = hits.GetEnumerator();
            if (enumerator.MoveNext())
                return projector((Hit)enumerator.Current);

            if (!defaultIfNone)
                throw new InvalidOperationException("Sequence contains no elements");

            return Activator.CreateInstance(elementType);
        }

        internal static object Single(IEnumerable hits, Func<Hit, object> projector, bool defaultIfNone, Type elementType)
        {
            var enumerator = hits.GetEnumerator();

            if (!enumerator.MoveNext())
                if (defaultIfNone)
                    return Activator.CreateInstance(elementType);
                else
                    throw new InvalidOperationException("Sequence contains no elements");

            var single = enumerator.Current;

            if (enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains more than one element");

            return projector((Hit)single);
        }
    }
}