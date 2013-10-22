// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Contains a number of child filters all of which must be
    /// true for a document to be selected by this filter.
    /// </summary>
    internal class AndFilter : CompoundFilter
    {
        public AndFilter(params IFilter[] filters)
            : base(filters)
        {
        }

        public override string Name
        {
            get { return "and"; }
        }

        public static AndFilter Combine(params IFilter[] filters)
        {
            Argument.EnsureNotNull("filters", filters);

            var combinedFilters = new List<IFilter>(filters);
            CombineRanges(combinedFilters);
            return new AndFilter(combinedFilters.ToArray());
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
    }
}