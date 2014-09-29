// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies one or more possible values that a
    /// field must match in order to select a document.
    /// </summary>
    internal class TermCriteria : ITermsCriteria
    {
        private readonly string field;
        private readonly MemberInfo member;
        private readonly ReadOnlyCollection<object> values;

        public TermCriteria(string field, MemberInfo member, object value)
        {
            this.field = field;
            this.member = member;
            values = new ReadOnlyCollection<object>(new[] { value });
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

        public MemberInfo Member
        {
            get { return member; }
        }

        public string Name
        {
            get { return "term"; }
        }

        public object Value
        {
            get { return values[0]; }
        }

        ReadOnlyCollection<object> ITermsCriteria.Values
        {
            get { return values; }
        }

        public override string ToString()
        {
            return String.Format("term {0} {1}", Field, Value);
        }
    }
}