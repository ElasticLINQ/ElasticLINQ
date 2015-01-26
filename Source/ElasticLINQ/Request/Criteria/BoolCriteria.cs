// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Specifies subcriteria to be satisfied as must (and), should (or) and must_not (none).
    /// </summary>
    internal class BoolCriteria : ICriteria
    {
        private readonly ReadOnlyCollection<ICriteria> must;
        private readonly ReadOnlyCollection<ICriteria> should;
        private readonly ReadOnlyCollection<ICriteria> mustNot;

        public BoolCriteria(IEnumerable<ICriteria> must, IEnumerable<ICriteria> should, IEnumerable<ICriteria> mustNot)
        {
            this.must = new ReadOnlyCollection<ICriteria>((must ?? Enumerable.Empty<ICriteria>()).ToList());
            this.should = new ReadOnlyCollection<ICriteria>((should ?? Enumerable.Empty<ICriteria>()).ToList());
            this.mustNot = new ReadOnlyCollection<ICriteria>((mustNot ?? Enumerable.Empty<ICriteria>()).ToList());
        }

        public string Name
        {
            get { return "bool"; }
        }

        public IReadOnlyCollection<ICriteria> Must { get { return must; } }
        public IReadOnlyCollection<ICriteria> Should { get { return should; } }
        public IReadOnlyCollection<ICriteria> MustNot { get { return mustNot; } }
    }
}