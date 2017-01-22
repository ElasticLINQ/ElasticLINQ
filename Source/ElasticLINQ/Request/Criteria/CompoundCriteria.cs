// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Base class for any criteria wanting to have criteria of its
    /// own such as AndCriteria and OrCriteria.
    /// </summary>
    abstract class CompoundCriteria : ICriteria
    {
        readonly ReadOnlyCollection<ICriteria> criteria;

        /// <summary>
        /// Create a criteria that has subcriteria. The exact semantics of
        /// the subcriteria are controlled by subclasses of CompoundCriteria.
        /// </summary>
        /// <param name="criteria"></param>
        protected CompoundCriteria(IEnumerable<ICriteria> criteria)
        {
            Argument.EnsureNotNull(nameof(criteria), criteria);
            this.criteria = new ReadOnlyCollection<ICriteria>(criteria.ToArray());
        }

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <summary>
        /// Criteria that is compounded by this criteria in some way (as determined by the subclass).
        /// </summary>
        public ReadOnlyCollection<ICriteria> Criteria
        {
            get { return criteria; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, string.Join(", ", Criteria.Select(f => f.ToString()).ToArray()));
        }
    }
}