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
            return CombineTerms(filters) ?? new OrFilter(filters);
        }

        private static IFilter CombineTerms(ICollection<IFilter> filters)
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