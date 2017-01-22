// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a range of desired values for a given
    /// field that need to be satisfied to select a document.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Field) + "}")]
    class RangeCriteria : ICriteria
    {
        readonly string field;
        readonly MemberInfo member;
        readonly ReadOnlyCollection<RangeSpecificationCriteria> specifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeCriteria"/> class.
        /// </summary>
        /// <param name="field">Field that must be within the specified ranges.</param>
        /// <param name="member">Property or field that this range criteria applies to.</param>
        /// <param name="specifications">Specifications (upper and lower bounds) that must be met.</param>
        public RangeCriteria(string field, MemberInfo member, IEnumerable<RangeSpecificationCriteria> specifications)
        {
            Argument.EnsureNotBlank(nameof(field), field);
            Argument.EnsureNotNull(nameof(member), member);
            Argument.EnsureNotNull(nameof(specifications), specifications);

            this.field = field;
            this.member = member;
            this.specifications = new ReadOnlyCollection<RangeSpecificationCriteria>(specifications.ToArray());
        }

        public RangeCriteria(string field, MemberInfo member, RangeComparison comparison, object value)
            : this(field, member, new[] { new RangeSpecificationCriteria(comparison, value) }) { }

        /// <summary>
        /// Property or field that this range criteria applies to.
        /// </summary>
        public MemberInfo Member => member;

        /// <inheritdoc/>
        public string Name => "range";

        /// <summary>
        /// Field that must be within the specified ranges.
        /// </summary>
        public string Field => field;

        /// <summary>
        /// Specifications (upper and lower bounds) that must be met.
        /// </summary>
        public ReadOnlyCollection<RangeSpecificationCriteria> Specifications => specifications;

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("range: {0}({1})", field, string.Join(",", specifications.Select(s => s.ToString())));
        }

        /// <summary>
        /// Determine whether a list of <see cref="RangeSpecificationCriteria" /> can be combined or not.
        /// </summary>
        /// <param name="specifications">List of <see cref="RangeSpecificationCriteria" />to be considered.</param>
        /// <returns><c>true</c> if they can be combined; otherwise <c>false</c>.</returns>
        internal static bool SpecificationsCanBeCombined(List<RangeSpecificationCriteria> specifications)
        {
            return specifications.Count(r => r.Comparison == RangeComparison.GreaterThan || r.Comparison == RangeComparison.GreaterThanOrEqual) < 2
                 && specifications.Count(r => r.Comparison == RangeComparison.LessThan || r.Comparison == RangeComparison.LessThanOrEqual) < 2;
        }
    }

    /// <summary>
    /// Type of RangeComparison operations.
    /// </summary>
    enum RangeComparison
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    [DebuggerDisplay("{Name,nq} {Value}")]
    class RangeSpecificationCriteria : ICriteria
    {
        static readonly Dictionary<RangeComparison, string> rangeComparisonValues = new Dictionary<RangeComparison, string>
        {
            { RangeComparison.GreaterThan, "gt" },
            { RangeComparison.GreaterThanOrEqual, "gte" },
            { RangeComparison.LessThan, "lt" },
            { RangeComparison.LessThanOrEqual, "lte" },
        };

        readonly RangeComparison comparison;
        readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSpecificationCriteria"/> class.
        /// </summary>
        /// <param name="comparison">Type of comparison for this range specification.</param>
        /// <param name="value">Constant value that this range specification tests against.</param>
        public RangeSpecificationCriteria(RangeComparison comparison, object value)
        {
            Argument.EnsureIsDefinedEnum(nameof(comparison), comparison);
            Argument.EnsureNotNull(nameof(value), value);

            this.comparison = comparison;
            this.value = value;
        }

        /// <summary>
        /// Type of comparison for this range specification.
        /// </summary>
        public RangeComparison Comparison => comparison;

        /// <inheritdoc/>
        public string Name => rangeComparisonValues[comparison];

        /// <summary>
        /// Constant value that this range specification tests against.
        /// </summary>
        public object Value => value;

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} {1}", comparison, value);
        }
    }
}