// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Filter that requires one of the child filters to be
    /// true in order to select the document.
    /// </summary>
    internal class OrFilter : CompoundFilter
    {
        public OrFilter(params IFilter[] filters)
            : base(filters)
        {
        }

        public override string Name
        {
            get { return "or"; }
        }

        public static IFilter Combine(params IFilter[] filters)
        {
            // Combines ((a || b) || c) from expression tree into (a || b || c)
            filters = UnwrapOrSubfilters(filters).ToArray();

            return CombineTermsForSameField(filters) ?? new OrFilter(filters);
        }

        private static IEnumerable<IFilter> UnwrapOrSubfilters(IEnumerable<IFilter> filters)
        {
            foreach (var filter in filters)
            {
                if (filter is OrFilter)
                    foreach (var subFilter in ((OrFilter)filter).Filters)
                        yield return subFilter;
                else
                    yield return filter;
            }
        }

        private static IFilter CombineTermsForSameField(ICollection<IFilter> filters)
        {
            if (filters.Count <= 1) return null;

            var termFilters = filters.OfType<TermFilter>().ToArray();
            var areAllSameTerm = termFilters.Length == filters.Count
                                 && termFilters.Select(f => f.Field).Distinct().Count() == 1;

            return areAllSameTerm
                ? TermFilter.FromIEnumerable(termFilters[0].Field, termFilters.SelectMany(f => f.Values).Distinct())
                : null;
        }
    }
}