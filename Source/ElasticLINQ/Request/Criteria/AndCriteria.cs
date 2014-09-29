// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using ElasticLinq.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Contains criteria all of which must be satisfied
    /// for a document to be selected.
    /// </summary>
    internal class AndCriteria : CompoundCriteria
    {
        public AndCriteria(params ICriteria[] criteria)
            : base(criteria)
        {
        }

        public override string Name
        {
            get { return "and"; }
        }

        public static ICriteria Combine(params ICriteria[] criteria)
        {
            Argument.EnsureNotNull("criteria", criteria);

            // Strip out null args and handle cases where no combination required
            criteria = criteria.Where(c => c != null).ToArray();
            if (criteria.Length == 0)
                return null;
            if (criteria.Length == 1)
                return criteria[0];

            // MatchAll in an And is redundant when other more restrictive criteria present
            var strippedCriteria = criteria.Where(c => !(c is MatchAllCriteria)).ToArray();
            if (strippedCriteria.Length == 0)
                return criteria[0];
            criteria = strippedCriteria;

            // Unwrap and combine ANDs
            var combinedCriteria = criteria
                .SelectMany(c => c is AndCriteria ? ((AndCriteria)c).Criteria : new ReadOnlyCollection<ICriteria>(new[] { c }))
                .ToList();
            CombineRanges(combinedCriteria);

            return combinedCriteria.Count == 1
                ? combinedCriteria[0]
                : new AndCriteria(combinedCriteria.ToArray());
        }

        private static void CombineRanges(ICollection<ICriteria> criteria)
        {
            var combinableRanges = criteria.OfType<RangeCriteria>().GroupBy(r => r.Field).Where(g => g.Count() > 1).ToArray();
            foreach (var range in combinableRanges)
            {
                foreach (var rangeCriteria in range)
                    criteria.Remove(rangeCriteria);

                criteria.Add(new RangeCriteria(range.Key, range.First().Member, range.SelectMany(r => r.Specifications)));
            }
        }
    }
}