// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;

namespace ElasticLinq.Utility
{
    static class KeyValuePair
    {
        public static KeyValuePair<T1, T2> Create<T1, T2>(T1 key, T2 value)
        {
            return new KeyValuePair<T1, T2>(key, value);
        }
    }
}