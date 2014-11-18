// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Represents constant placeholders in the criteria tree caused by constant
    /// expressions in where trees until they can be optimized out.
    /// </summary>
    internal static class ConstantCriteriaFilterReducer
    {
        public static ICriteria Reduce(ICriteria criteria)
        {
            if (criteria is AndCriteria)
                return Reduce((AndCriteria)criteria);

            if (criteria is OrCriteria)
                return Reduce((OrCriteria)criteria);

            return criteria;
        }

        private static ICriteria Reduce(AndCriteria andCriteria)
        {
            if (andCriteria.Criteria.Contains(ConstantCriteria.False))
                return ConstantCriteria.False;

            return AndCriteria
                .Combine(andCriteria
                    .Criteria
                    .Where(c => c != ConstantCriteria.True)
                    .Select(Reduce)
                .Where(r => r != null)
                .ToArray());
        }

        private static ICriteria Reduce(OrCriteria orCriteria)
        {
            if (orCriteria.Criteria.Any(c => c == ConstantCriteria.True))
                return ConstantCriteria.True;

            return OrCriteria
                .Combine(orCriteria
                    .Criteria
                    .Where(c => c != ConstantCriteria.False)
                    .Select(Reduce)
                .Where(r => r != null)
                .ToArray());
        }
    }
}