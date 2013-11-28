// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ElasticLinq.Utility;

namespace ElasticLinq.Response
{
    /// <summary>
    /// Materializes responses from ElasticSearch.
    /// </summary>
    public static class ElasticResponseMaterializer
    {
        private static readonly MethodInfo materializer = typeof(ElasticResponseMaterializer).GetMethod("Materialize", BindingFlags.NonPublic | BindingFlags.Static);

        public static IList Materialize(IEnumerable<Hit> hits, Type elementType, Func<Hit, object> projector)
        {
            return (IList)materializer
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { hits, projector });
        }

        internal static List<T> Materialize<T>(IEnumerable<Hit> hits, Func<Hit, object> projector)
        {
            return hits.Select(projector).Cast<T>().ToList();
        }

        internal static object First(IList list, bool defaultIfNone)
        {
            if (list.Count != 0)
                return list[0];

            if (defaultIfNone)
                return Activator.CreateInstance(TypeHelper.GetSequenceElementType(list.GetType()));

            throw new InvalidOperationException("Sequence contains no elements");
        }

        internal static object Single(IList list, bool defaultIfNone)
        {
            if (list.Count > 1)
                throw new InvalidOperationException("Sequence contains more than one element");

            return First(list, defaultIfNone);
        }
    }
}