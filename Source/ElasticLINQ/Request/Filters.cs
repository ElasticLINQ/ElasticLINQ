// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;

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
