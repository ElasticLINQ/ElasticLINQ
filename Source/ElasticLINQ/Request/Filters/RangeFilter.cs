// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Filter that specifies a range of desired values for a given
    /// field that need to be satisfied to select a document.
    /// </summary>
    [DebuggerDisplay("{field}")]
    internal class RangeFilter : IFilter
    {
        private readonly string field;
        private readonly List<RangeSpecificationFilter> specifications;

        public RangeFilter(string field, IEnumerable<RangeSpecificationFilter> specifications)
        {
            Argument.EnsureNotBlank("field", field);
            Argument.EnsureNotNull("specifications", specifications);

            this.field = field;
            this.specifications = new List<RangeSpecificationFilter>(specifications);
        }

        public RangeFilter(string field, RangeComparison comparison, object value)
            : this(field, new[] { new RangeSpecificationFilter(comparison, value) })
        {
        }

        public string Name
        {
            get { return "range"; }
        }

        public string Field
        {
            get { return field; }
        }

        public IReadOnlyList<RangeSpecificationFilter> Specifications
        {
            get { return specifications; }
        }

        public override string ToString()
        {
            return "range: " + field + "(" + String.Join(",", specifications.Select(s => s.ToString())) + ")";
        }
    }

    internal enum RangeComparison
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    [DebuggerDisplay("{name,nq} {value}")]
    internal class RangeSpecificationFilter : IFilter
    {
        private readonly Dictionary<RangeComparison, string> rangeComparisonValues = new Dictionary<RangeComparison, string>
        {
            { RangeComparison.GreaterThan, "gt" },
            { RangeComparison.GreaterThanOrEqual, "gte" },
            { RangeComparison.LessThan, "lt" },
            { RangeComparison.LessThanOrEqual, "lte" },
        };

        private readonly RangeComparison comparison;
        private readonly object value;

        public RangeSpecificationFilter(RangeComparison comparison, object value)
        {
            Argument.EnsureIsDefinedEnum("comparison", comparison);
            Argument.EnsureNotNull("value", value);

            this.comparison = comparison;
            this.value = value;
        }

        public RangeComparison Comparison
        {
            get { return comparison; }
        }

        public string Name
        {
            get { return rangeComparisonValues[comparison]; }
        }

        public object Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            return comparison + " " + value;
        }
    }
}