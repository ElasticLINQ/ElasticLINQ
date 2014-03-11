// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies one or more possible values that a
    /// field must match in order to select a document.
    /// </summary>
    internal class TermCriteria : ICriteria
    {
        private readonly string field;
        private readonly HashSet<object> values;

        public static TermCriteria FromIEnumerable(string field, IEnumerable<object> values)
        {
            return new TermCriteria(field, values);
        }

        public static TermCriteria FromValue(string field, object value)
        {
            return new TermCriteria(field, value);
        }

        public TermCriteria(string field, params object[] values)
            : this(field, values.AsEnumerable())
        {
        }

        private TermCriteria(string field, IEnumerable<object> values)
        {
            Argument.EnsureNotNull("value", values);
            this.field = field;
            this.values = new HashSet<object>(values);
        }

        public string Field
        {
            get { return field; }
        }

        public bool IsOrCriteria
        {
            get
            {
                // ExecutionMode is irrelevant if this is a "term" request, so we say that
                // it's Or-compatible so that OrCriteria will merge it.
                return values.Count == 1
                    || !ExecutionMode.HasValue
                    || ExecutionMode == TermsExecutionMode.@bool
                    || ExecutionMode == TermsExecutionMode.or
                    || ExecutionMode == TermsExecutionMode.plain;
            }
        }

        public TermsExecutionMode? ExecutionMode { get; set; }

        public IReadOnlyList<Object> Values
        {
            get { return values.ToList().AsReadOnly(); }
        }

        public string Name
        {
            get { return values.Count == 1 ? "term" : "terms"; }
        }

        public override string ToString()
        {
            var value = String.Format("{0} {1} [{2}]", Name, Field, String.Join(",", values.ToArray()));
            if (ExecutionMode.HasValue)
                value += String.Format(" (execution = {0})", ExecutionMode.GetValueOrDefault());

            return value;
        }
    }
}