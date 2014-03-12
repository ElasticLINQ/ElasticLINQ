// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies one or more possible values that a
    /// field must match in order to select a document.
    /// </summary>
    internal class TermCriteria : ITermsCriteria
    {
        private readonly string field;
        private readonly object value;

        public TermCriteria(string field, object value)
        {
            this.field = field;
            this.value = value;
        }

        public string Field
        {
            get { return field; }
        }

        // "term" is always implicitly combinable by OrCriteria.Combine
        bool ITermsCriteria.IsOrCriteria
        {
            get { return true; }
        }

        public string Name
        {
            get { return "term"; }
        }

        public object Value
        {
            get { return value; }
        }

        IReadOnlyList<object> ITermsCriteria.Values
        {
            get { return new[] { Value }; }
        }

        public override string ToString()
        {
            return String.Format("term {0} {1}", Field, Value);
        }
    }
}