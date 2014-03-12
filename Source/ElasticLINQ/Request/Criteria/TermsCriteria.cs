// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    public class TermsCriteria : ITermsCriteria
    {
        private readonly TermsExecutionMode? executionMode;
        private readonly string field;
        private readonly HashSet<object> values;

        private TermsCriteria(TermsExecutionMode? executionMode, string field, HashSet<object> values)
        {
            this.executionMode = executionMode;
            this.field = field;
            this.values = values;
        }

        public TermsExecutionMode? ExecutionMode
        {
            get { return executionMode; }
        }

        public string Field
        {
            get { return field; }
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

        public string Name
        {
            get { return "terms"; }
        }

        public IReadOnlyList<Object> Values
        {
            get { return values.ToList().AsReadOnly(); }
        }

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
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(string field, params object[] values)
        {
            return Build(null, field, values.AsEnumerable());
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(string field, IEnumerable<object> values)
        {
            return Build(null, field, values);
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="executionMode">The terms execution mode (optional). Only used when a <see cref="TermsCriteria"/> is returned.</param>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(TermsExecutionMode? executionMode, string field, params object[] values)
        {
            return Build(executionMode, field, values.AsEnumerable());
        }

        /// <summary>
        /// Builds a <see cref="TermCriteria"/> or <see cref="TermsCriteria"/>, depending on how many values are
        /// present in the <paramref name="values"/> collection.
        /// </summary>
        /// <param name="executionMode">The terms execution mode (optional). Only used when a <see cref="TermsCriteria"/> is returned.</param>
        /// <param name="field">The field that's being searched.</param>
        /// <param name="values">The values to be matched.</param>
        /// <returns>Either a <see cref="TermCriteria"/> object or a <see cref="TermsCriteria"/> object.</returns>
        internal static ITermsCriteria Build(TermsExecutionMode? executionMode, string field, IEnumerable<object> values)
        {
            Argument.EnsureNotNull("values", values);

            var hashValues = new HashSet<object>(values);
            if (hashValues.Count == 1)
                return new TermCriteria(field, hashValues.First());

            return new TermsCriteria(executionMode, field, hashValues);
        }
    }
}