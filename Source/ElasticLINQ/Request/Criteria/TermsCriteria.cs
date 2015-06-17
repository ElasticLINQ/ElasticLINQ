// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies one or more possible values that a
    /// field must match in order to select a document.
    /// </summary>
    public class TermsCriteria : SingleFieldCriteria, ITermsCriteria
    {
        private readonly TermsExecutionMode? executionMode;
        private readonly MemberInfo member;
        private readonly ReadOnlyCollection<object> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsCriteria"/> class.
        /// </summary>
        /// <param name="executionMode">Type of execution mode this terms criteria will take.</param>
        /// <param name="field">Field to be checked for this term.</param>
        /// <param name="member">Property or field being checked for this term.</param>
        /// <param name="values">Constant values being searched for.</param>
        private TermsCriteria(TermsExecutionMode? executionMode, string field, MemberInfo member, IEnumerable<object> values)
            : base(field)
        {
            this.executionMode = executionMode;
            this.member = member;
            this.values = new ReadOnlyCollection<object>(values.ToArray());
        }

        /// <summary>
        /// Type of execution mode this terms criteria will take.
        /// </summary>
        public TermsExecutionMode? ExecutionMode
        {
            get { return executionMode; }
        }

        bool ITermsCriteria.IsOrCriteria
        {
            get
            {
                // "plain" is the default; "plain", "or", and "bool" are all or queries, but
                // with slightly different caching semantics.
                return !ExecutionMode.HasValue
                    || ExecutionMode == TermsExecutionMode.@bool
                    || ExecutionMode == TermsExecutionMode.or
                    || ExecutionMode == TermsExecutionMode.plain;
            }
        }

        /// <summary>
        /// Property or field being checked for this term.
        /// </summary>
        public MemberInfo Member
        {
            get { return member; }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get { return "terms"; }
        }

        /// <summary>
        /// Constant values being searched for.
        /// </summary>
        public ReadOnlyCollection<Object> Values
        {
            get { return values; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = String.Format("terms {0} [{1}]", Field, String.Join(", ", Values));
            if (ExecutionMode.HasValue)
                result += String.Format(" (execution: {0})", ExecutionMode);

            return result;
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="member">The member information for the field.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(string field, MemberInfo member, params object[] values)
        {
            return Build(null, field, member, values.AsEnumerable());
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="member">The member information for the field.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(string field, MemberInfo member, IEnumerable<object> values)
        {
            return Build(null, field, member, values);
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="executionMode">The terms execution mode (optional). Only used when a <see cref="TermsCriteria"/> is returned.</param>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="member">The member information for the field.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(TermsExecutionMode? executionMode, string field, MemberInfo member, params object[] values)
        {
            return Build(executionMode, field, member, values.AsEnumerable());
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="executionMode">The terms execution mode (optional). Only used when a <see cref="TermsCriteria"/> is returned.</param>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="member">The member information for the field.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(TermsExecutionMode? executionMode, string field, MemberInfo member, IEnumerable<object> values)
        {
            Argument.EnsureNotNull("values", values);

            var hashValues = new HashSet<object>(values);
            if (hashValues.Count == 1)
                return new TermCriteria(field, member, hashValues.First());

            return new TermsCriteria(executionMode, field, member, hashValues);
        }
    }
}
