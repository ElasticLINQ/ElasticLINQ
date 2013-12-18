// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections;

namespace ElasticLinq.Response
{
    /// <summary>
    /// Materializes responses from ElasticSearch.
    /// </summary>
    internal static class ElasticResponseMaterializer
    {
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