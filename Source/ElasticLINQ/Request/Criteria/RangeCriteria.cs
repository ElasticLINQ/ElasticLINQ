// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a range of desired values for a given
    /// field that need to be satisfied to select a document.
    /// </summary>
    [DebuggerDisplay("{Field}")]
    internal class RangeCriteria : ICriteria
    {
        private readonly string field;
        private readonly List<RangeSpecificationCriteria> specifications;

        public RangeCriteria(string field, IEnumerable<RangeSpecificationCriteria> specifications)
        {
            Argument.EnsureNotBlank("field", field);
            Argument.EnsureNotNull("specifications", specifications);

            this.field = field;
            this.specifications = new List<RangeSpecificationCriteria>(specifications);
        }

        public RangeCriteria(string field, RangeComparison comparison, object value)
            : this(field, new[] { new RangeSpecificationCriteria(comparison, value) })
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

        public IReadOnlyList<RangeSpecificationCriteria> Specifications
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

    [DebuggerDisplay("{Name,nq} {Value}")]
    internal class RangeSpecificationCriteria : ICriteria
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

        public RangeSpecificationCriteria(RangeComparison comparison, object value)
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