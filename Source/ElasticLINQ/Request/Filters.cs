// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request
{
    internal abstract class Filter
    {
        public abstract string Name { get; }
    }

    internal class CompoundFilter : Filter
    {
        private readonly string name;
        private readonly List<Filter> filters;

        public CompoundFilter(string name, IEnumerable<Filter> filters)
        {
            this.name = name;
            this.filters = new List<Filter>(filters);
        }

        public override string Name
        {
            get { return name; }
        }

        public IReadOnlyList<Filter> Filters
        {
            get { return filters.AsReadOnly(); }
        }

        public override string ToString()
        {
            return string.Format(" {0} ({1})", Name, String.Join(" ", Filters.Select(f => f.ToString()).ToArray()));
        }
    }

    internal class TermFilter : Filter
    {
        private readonly string field;
        private readonly List<object> values;

        public TermFilter(string field, IEnumerable<object> values)
        {
            this.field = field;
            this.values = new List<object>(values);
        }

        public TermFilter(string field, object value)
            : this(field, new[] { value })
        {
        }

        public string Field
        {
            get { return field; }
        }

        public IReadOnlyList<Object> Values
        {
            get { return values.AsReadOnly(); }
        }

        public override string Name
        {
            get { return Values.Count == 1 ? "term" : "terms"; }
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", Name, String.Join(",", values.ToArray()));
        }
    }

    internal class OrFilter : CompoundFilter
    {
        public static Filter Combine(params Filter[] filters)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");

            var termFilters = filters.OfType<TermFilter>().ToArray();
            var areAllSameTerm = filters.Length > 0 && termFilters.Select(f => f.Field).Distinct().Count() == 1;

            if (areAllSameTerm)
                return new TermFilter(termFilters[0].Field, termFilters.SelectMany(f => f.Values).Distinct());

            return new OrFilter(filters);
        }

        public OrFilter(params Filter[] filters)
            : base("or", filters)
        {
        }

        public OrFilter(IEnumerable<Filter> filters)
            : this(filters.ToArray())
        {            
        }
    }

    internal class AndFilter : CompoundFilter
    {
        public AndFilter(params Filter[] filters)
            : base("and", filters)
        {
        }

        public AndFilter(IEnumerable<Filter> filters)
            : this(filters.ToArray())
        {            
        }
    }

    internal class MustFilter : Filter
    {
        private readonly List<Filter> entries;

        public MustFilter(IEnumerable<Filter> entries)
        {
            this.entries = new List<Filter>(entries);
        }

        public override string Name
        {
            get { return "must"; }
        }

        public IReadOnlyList<Filter> Entries
        {
            get { return entries.AsReadOnly(); }
        }
    }
}
