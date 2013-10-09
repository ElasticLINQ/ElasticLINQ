// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Filters
{
    internal class AndFilter : CompoundFilter
    {
        public static IFilter Combine(params IFilter[] filters)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");

            var combinedFilters = new List<IFilter>(filters);
            
            CombineRanges(combinedFilters);

            return new AndFilter(combinedFilters);
        }

        private static void CombineRanges(ICollection<IFilter> filters)
        {
            var combinableRanges = filters.OfType<RangeFilter>().GroupBy(r => r.Field).Where(g => g.Count() > 1).ToArray();
            foreach (var range in combinableRanges)
            {
                foreach (var filter in range)
                    filters.Remove(filter);
                filters.Add(new RangeFilter(range.Key, range.SelectMany(r => r.Specifications)));
            }
        }

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