// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Request.Filters
{
    internal class NotFilter : IFilter
    {
        private readonly IFilter subFilter;

        public NotFilter(IFilter subFilter)
        {
            this.subFilter = subFilter;
        }

        public string Name { get { return "not"; } }

        public IFilter SubFilter
        {
            get { return subFilter; }
        }
    }
}