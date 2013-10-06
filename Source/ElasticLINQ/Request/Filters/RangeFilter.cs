// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;
using System.Diagnostics;

namespace ElasticLinq.Request.Filters
{
    [DebuggerDisplay("{field}")]
    internal class RangeFilter : IFilter
    {
        private readonly string field;
        private readonly List<RangeSpecificationFilter> specifications;

        public RangeFilter(string field, IEnumerable<RangeSpecificationFilter> specifications)
        {
            this.field = field;
            this.specifications = new List<RangeSpecificationFilter>(specifications);
        }

        public RangeFilter(string field, RangeSpecificationFilter specification)
            : this(field, new[] { specification })
        {
        }

        public string Name { get { return "range"; } }
        public string Field { get { return field; } }
        public IReadOnlyList<RangeSpecificationFilter> Specifications { get { return specifications; } }
    }

    [DebuggerDisplay("{name,nq} {value}")]
    internal class RangeSpecificationFilter : IFilter
    {
        private readonly string name;
        private readonly object value;

        public RangeSpecificationFilter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name { get { return name; } }
        public object Value { get { return value; } }
    }
}