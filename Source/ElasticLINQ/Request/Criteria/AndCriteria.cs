// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Specifies that all subcriteria must be satisfied.
    /// </summary>
    class AndCriteria : CompoundCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndCriteria"/> class.
        /// </summary>
        /// <param name="criteria">Criteria to combine with 'and' semantics.</param>
        /// <remarks>Consider using <see cref="AndCriteria.Combine(ICriteria[])"/> instead.</remarks>
        public AndCriteria(params ICriteria[] criteria)
            : base(criteria)
        {
        }

        /// <inheritdoc/>
        public override string Name => "and";

        /// <summary>
        /// Combine a number of <see cref="ICriteria" /> with 'and' semantics.
        /// </summary>
        /// <param name="criteria">The <see cref="ICriteria" /> to be combined.</param>
        /// <returns><see cref="ICriteria" /> representing the original passed <see cref="ICriteria" /> with 'and' semantics.</returns>
        /// <remarks>This is usually an <see cref="AndCriteria" /> but might not be if the passed criteria can be collapsed into
        /// a single criteria.</remarks>
        public static ICriteria Combine(params ICriteria[] criteria)
        {
            Argument.EnsureNotNull(nameof(criteria), criteria);

            // Strip out null args and handle cases where no combination required
            criteria = criteria.Where(c => c != null).ToArray();
            if (criteria.Length == 0)
                return null;
            if (criteria.Length == 1)
                return criteria[0];

            // Unwrap and combine ANDs
            var combinedCriteria = criteria
                .SelectMany(c => c is AndCriteria ? ((AndCriteria)c).Criteria : new ReadOnlyCollection<ICriteria>(new[] { c }))
                .ToList();

            CombineRanges(combinedCriteria);

            return combinedCriteria.Count == 1
                ? combinedCriteria[0]
                : new AndCriteria(combinedCriteria.ToArray());
        }

        /// <summary>
        /// Combine range criteria for the same field into an upper-lower range for that criteria.
        /// </summary>
        /// <param name="criteria">Collection of <see cref="ICriteria"/> to have ranges combined.</param>
        static void CombineRanges(ICollection<ICriteria> criteria)
        {
            var candidates = criteria.OfType<RangeCriteria>().GroupBy(r => r.Field).Where(g => g.Count() > 1).ToArray();

            foreach (var range in candidates)
            {
                var specifications = range.SelectMany(r => r.Specifications).ToList();

                if (RangeCriteria.SpecificationsCanBeCombined(specifications))
                {
                    foreach (var rangeCriteria in range)
                        criteria.Remove(rangeCriteria);

                    criteria.Add(new RangeCriteria(range.Key, range.First().Member, specifications));
                }
            }
        }
    }
}