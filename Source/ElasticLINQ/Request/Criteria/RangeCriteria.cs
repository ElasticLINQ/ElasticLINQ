// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
        private readonly MemberInfo member;
        private readonly ReadOnlyCollection<RangeSpecificationCriteria> specifications;

        public RangeCriteria(string field, MemberInfo member, IEnumerable<RangeSpecificationCriteria> specifications)
        {
            Argument.EnsureNotBlank("field", field);
            Argument.EnsureNotNull("member", member);
            Argument.EnsureNotNull("specifications", specifications);

            this.field = field;
            this.member = member;
            this.specifications = new ReadOnlyCollection<RangeSpecificationCriteria>(specifications.ToArray());
        }

        public RangeCriteria(string field, MemberInfo member, RangeComparison comparison, object value)
            : this(field, member, new[] { new RangeSpecificationCriteria(comparison, value) }) { }

        public MemberInfo Member
        {
            get { return member; }
        }

        public string Name
        {
            get { return "range"; }
        }

        public string Field
        {
            get { return field; }
        }

        public ReadOnlyCollection<RangeSpecificationCriteria> Specifications
        {
            get { return specifications; }
        }

        public override string ToString()
        {
            return String.Format("range: {0}({1})", field, String.Join(",", specifications.Select(s => s.ToString())));
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
            return String.Format("{0} {1}", comparison, value);
        }
    }
}