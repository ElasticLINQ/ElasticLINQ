// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Reduces <see cref="ConstantCriteria" /> within criteria recursively.
    /// </summary>
    static class ConstantCriteriaFilterReducer
    {
        /// <summary>
        /// Reduce a <see cref="ICriteria" /> that might contain a <see cref="ConstantCriteria" />.
        /// </summary>
        /// <param name="criteria">Criteria to be reduced.</param>
        /// <returns>Reduced criteria.</returns>
        public static ICriteria Reduce(ICriteria criteria)
        {
            if (criteria is AndCriteria)
                return Reduce((AndCriteria)criteria);

            if (criteria is OrCriteria)
                return Reduce((OrCriteria)criteria);

            return criteria;
        }

        /// <summary>
        /// Reduce a <see cref="AndCriteria" /> that might contain a <see cref="ConstantCriteria" />.
        /// </summary>
        /// <param name="andCriteria"><see cref="AndCriteria" /> to be reduced.</param>
        /// <returns>Reduced criteria.</returns>
        /// <remarks>
        /// Trues will be removed, falses will replace the entire And with a false.
        /// </remarks>
        static ICriteria Reduce(AndCriteria andCriteria)
        {
            if (andCriteria.Criteria.Contains(ConstantCriteria.False))
                return ConstantCriteria.False;

            return AndCriteria
                .Combine(andCriteria
                    .Criteria
                    .Select(Reduce)
                    .Where(c => c != ConstantCriteria.True && c != null)
                .ToArray());
        }

        /// <summary>
        /// Reduce an <see cref="OrCriteria" /> that might contain a <see cref="ConstantCriteria" />.
        /// </summary>
        /// <param name="orCriteria"><see cref="OrCriteria" /> to be reduced.</param>
        /// <returns>Reduced criteria.</returns>
        /// <remarks>
        /// Falses will be removed, trues will replace the entire Or with a true.
        /// </remarks>
        static ICriteria Reduce(OrCriteria orCriteria)
        {
            if (orCriteria.Criteria.Any(c => c == ConstantCriteria.True))
                return ConstantCriteria.True;

            return OrCriteria
                .Combine(orCriteria
                    .Criteria
                    .Select(Reduce)
                    .Where(c => c != ConstantCriteria.False && c != null)
                .ToArray());
        }
    }
}