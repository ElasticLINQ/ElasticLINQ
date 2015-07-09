// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Specifies subcriteria to be satisfied as must (and), should (or) and must_not (none).
    /// </summary>
    class BoolCriteria : ICriteria
    {
        readonly ReadOnlyCollection<ICriteria> must;
        readonly ReadOnlyCollection<ICriteria> should;
        readonly ReadOnlyCollection<ICriteria> mustNot;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolCriteria"/> class.
        /// </summary>
        /// <param name="must">Criteria that <b>must</b> be satisfied for this bool criteria to be satisfied.</param>
        /// <param name="should">Criteria that <b>should</b> be satisfied for this bool criteria to be satisfied.</param>
        /// <param name="mustNot">Criteria that <b>must not</b> be satisfied for this bool criteria to be satisfied.</param>
        public BoolCriteria(IEnumerable<ICriteria> must, IEnumerable<ICriteria> should, IEnumerable<ICriteria> mustNot)
        {
            this.must = new ReadOnlyCollection<ICriteria>((must ?? Enumerable.Empty<ICriteria>()).ToList());
            this.should = new ReadOnlyCollection<ICriteria>((should ?? Enumerable.Empty<ICriteria>()).ToList());
            this.mustNot = new ReadOnlyCollection<ICriteria>((mustNot ?? Enumerable.Empty<ICriteria>()).ToList());
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "bool"; }
        }

        /// <summary>
        /// Criteria that must be satisfied for this bool criteria to be satisfied.
        /// </summary>
        public IReadOnlyCollection<ICriteria> Must { get { return must; } }

        /// <summary>
        /// Criteria that should be satisfied for this bool criteria to be satisfied.
        /// </summary>
        public IReadOnlyCollection<ICriteria> Should { get { return should; } }

        /// <summary>
        /// Criteria that must not be satisfied for this bool criteria to be satisfied.
        /// </summary>
        public IReadOnlyCollection<ICriteria> MustNot { get { return mustNot; } }
    }
}