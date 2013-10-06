// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Filters
{
    internal class AndFilter : CompoundFilter
    {
        public AndFilter(params IFilter[] filters)
            : base(filters)
        {
        }

        public AndFilter(IEnumerable<IFilter> filters)
            : this(filters.ToArray())
        {
        }

        public override string Name { get { return "and"; } }
    }
}