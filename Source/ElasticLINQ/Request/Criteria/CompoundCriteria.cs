// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Base class for any criteria wanting to have criteria of its
    /// own such as AndCriteria and OrCriteria.
    /// </summary>
    internal abstract class CompoundCriteria : ICriteria
    {
        private readonly List<ICriteria> criteria;

        protected CompoundCriteria(IEnumerable<ICriteria> criteria)
        {
            Argument.EnsureNotNull("criteria", criteria);
            this.criteria = new List<ICriteria>(criteria);
        }

        public abstract string Name { get; }

        public IReadOnlyList<ICriteria> Criteria
        {
            get { return criteria.AsReadOnly(); }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, String.Join(", ", Criteria.Select(f => f.ToString()).ToArray()));
        }
    }
}