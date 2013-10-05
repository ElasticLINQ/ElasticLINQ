// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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