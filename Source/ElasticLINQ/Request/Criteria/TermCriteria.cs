// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using System.Reflection;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies one possible value that a
    /// field must match in order to select a document.
    /// </summary>
    class TermCriteria : SingleFieldCriteria, ITermsCriteria
    {
        readonly ReadOnlyCollection<object> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermCriteria"/> class.
        /// </summary>
        /// <param name="field">Field to be checked for this term.</param>
        /// <param name="member">Property or field being checked for this term.</param>
        /// <param name="value">Value to be checked for this term.</param>
        public TermCriteria(string field, MemberInfo member, object value)
            : base(field)
        {
            Member = member;
            values = new ReadOnlyCollection<object>(new[] { value });
        }

        // "term" is always implicitly combinable by OrCriteria.Combine
        bool ITermsCriteria.IsOrCriteria
        {
            get { return true; }
        }

        /// <summary>
        /// Property or field being checked for this term.
        /// </summary>
        public MemberInfo Member { get; }

        /// <inheritdoc/>
        public override string Name
        {
            get { return "term"; }
        }

        /// <summary>
        /// Constant value being checked.
        /// </summary>
        public object Value
        {
            get { return values[0]; }
        }

        /// <summary>
        /// List of constant values being checked for.
        /// </summary>
        ReadOnlyCollection<object> ITermsCriteria.Values
        {
            get { return values; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"term {Field} {Value}";
        }
    }
}