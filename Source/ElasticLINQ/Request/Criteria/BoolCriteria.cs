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
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolCriteria"/> class.
        /// </summary>
        /// <param name="must">Criteria that <b>must</b> be satisfied for this bool criteria to be satisfied.</param>
        /// <param name="should">Criteria that <b>should</b> be satisfied for this bool criteria to be satisfied.</param>
        /// <param name="mustNot">Criteria that <b>must not</b> be satisfied for this bool criteria to be satisfied.</param>
        public BoolCriteria(IEnumerable<ICriteria> must, IEnumerable<ICriteria> should, IEnumerable<ICriteria> mustNot)
        {
            Must = new ReadOnlyCollection<ICriteria>((must ?? Enumerable.Empty<ICriteria>()).ToList());
            Should = new ReadOnlyCollection<ICriteria>((should ?? Enumerable.Empty<ICriteria>()).ToList());
            MustNot = new ReadOnlyCollection<ICriteria>((mustNot ?? Enumerable.Empty<ICriteria>()).ToList());
        }

        /// <inheritdoc/>
        public string Name => "bool";

        /// <summary>
        /// Criteria that must be satisfied for this bool criteria to be satisfied.
        /// </summary>
        public IReadOnlyCollection<ICriteria> Must { get; }

        /// <summary>
        /// Criteria that should be satisfied for this bool criteria to be satisfied.
        /// </summary>
        public IReadOnlyCollection<ICriteria> Should { get; }

        /// <summary>
        /// Criteria that must not be satisfied for this bool criteria to be satisfied.
        /// </summary>
        public IReadOnlyCollection<ICriteria> MustNot { get; }
    }
}