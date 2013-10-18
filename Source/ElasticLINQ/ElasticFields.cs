// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;

namespace ElasticLinq
{
    public static class ElasticFields
    {
        public static double Score
        {
            get
            {
                throw new InvalidOperationException("This property is for mapping queries to ElasticSearch and should not be evaluated directly.");
            }            
        }
    }
}