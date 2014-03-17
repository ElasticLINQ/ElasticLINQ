// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that requires one of the criteria to be
    /// satisfied in order to select the document.
    /// </summary>
    internal class OrCriteria : CompoundCriteria
    {
        public OrCriteria(params ICriteria[] criteria)
            : base(criteria)
        {
        }

        public override string Name
        {
            get { return "or"; }
        }

        public static ICriteria Combine(params ICriteria[] criteria)
        {
            // Combines ((a || b) || c) from expression tree into (a || b || c)
            criteria = UnwrapOrCriteria(criteria).ToArray();

            return CombineTermsForSameField(criteria) ?? new OrCriteria(criteria);
        }

        private static IEnumerable<ICriteria> UnwrapOrCriteria(IEnumerable<ICriteria> criteria)
        {
            foreach (var criterion in criteria)
            {
                if (criterion is OrCriteria)
                    foreach (var subCriterion in ((OrCriteria)criterion).Criteria)
                        yield return subCriterion;
                else
                    yield return criterion;
            }
        }

        private static ICriteria CombineTermsForSameField(ICollection<ICriteria> criteria)
        {
            if (criteria.Count <= 1) return null;

            var termCriteria = criteria.OfType<ITermsCriteria>().ToArray();
            var areAllSameTerm = termCriteria.Length == criteria.Count
                                 && termCriteria.Select(f => f.Field).Distinct().Count() == 1
                                 && termCriteria.All(f => f.IsOrCriteria);

            return areAllSameTerm
                ? TermsCriteria.Build(termCriteria[0].Field, termCriteria[0].Member, termCriteria.SelectMany(f => f.Values).Distinct())
                : null;
        }
    }
}