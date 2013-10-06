// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Filters
{
    internal class OrFilter : CompoundFilter
    {
        public static IFilter Combine(params IFilter[] filters)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");

            return CombineTerms(filters) ?? new OrFilter(filters);
        }

        private static IFilter CombineTerms(ICollection<IFilter> filters)
        {
            if (filters.Count > 1)
            {
                var termFilters = filters.OfType<TermFilter>().ToArray();
                var areAllSameTerm = termFilters.Length == filters.Count
                                     && termFilters.Select(f => f.Field).Distinct().Count() == 1;

                if (areAllSameTerm)
                    return new TermFilter(termFilters[0].Field, termFilters.SelectMany(f => f.Values).Distinct());
            }

            return null;
        }

        public OrFilter(params IFilter[] filters)
            : base(filters)
        {
        }

        public OrFilter(IEnumerable<IFilter> filters)
            : this(filters.ToArray())
        {
        }

        public override string Name { get { return "or"; } }
    }
}