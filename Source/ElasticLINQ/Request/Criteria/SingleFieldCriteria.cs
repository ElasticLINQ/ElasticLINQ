// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Base class for any criteria that maps to a single field.
    /// </summary>
    internal abstract class SingleFieldCriteria : ICriteria
    {
        private readonly string field;

        protected SingleFieldCriteria(string field)
        {
            Argument.EnsureNotBlank("field", field);
            this.field = field;
        }

        public string Field
        {
            get { return field; }
        }

        public abstract string Name
        {
            get;
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", Name, Field);
        }
    }
}