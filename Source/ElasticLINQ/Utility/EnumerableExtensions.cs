// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;

namespace ElasticLinq.Utility
{
    static class EnumerableExtensions
    {
        public static ReadOnlyBatchedList<T> ToReadOnlyBatchedList<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyBatchedList<T>(enumerable);
        }
    }
}
