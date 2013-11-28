// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Static constructors to infer type arguments for new instances of common
    /// classes.
    /// </summary>
    static class KeyValuePair
    {
        public static KeyValuePair<T1, T2> Create<T1, T2>(T1 key, T2 value)
        {
            return new KeyValuePair<T1, T2>(key, value);
        }
    }
}