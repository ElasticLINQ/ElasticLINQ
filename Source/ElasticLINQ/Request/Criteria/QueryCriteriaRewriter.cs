// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Query DSL is slightly different from Filter DSL. In order to keep
    /// code paths simple we always build as if doing a filter. Here we
    /// rewrite the ICriteria designed for a filter into a query.
    /// </summary>
    internal static class QueryCriteriaRewriter
    {
        /// <summary>
        /// Rewrite the 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static ICriteria Compensate(ICriteria criteria)
        {
            if (criteria is OrCriteria)
                return Rewrite((OrCriteria)criteria);

            if (criteria is AndCriteria)
                return Rewrite((AndCriteria)criteria);

            if (criteria is NotCriteria)
                return Rewrite((NotCriteria)criteria);

            if (criteria is ConstantCriteria)
                return Rewrite((ConstantCriteria)criteria);

            return criteria;
        }

        /// <summary>
        /// Rewrite a NotCriteria as a BoolCriteria.
        /// </summary>
        /// <param name="not">NotCriteria to rewrite.</param>
        /// <returns>BoolCriteria with the criteria from Not mapped into MustNot.</returns>
        private static BoolCriteria Rewrite(NotCriteria not)
        {
            var mustNotCriteria = (not.Criteria is OrCriteria)
                ? ((OrCriteria) not.Criteria).Criteria
                : Enumerable.Repeat(not.Criteria, 1);
            return new BoolCriteria(null, null, mustNotCriteria.Select(Compensate));
        }

        /// <summary>
        /// Rewrite an OrCriteria as a BoolCriteria.
        /// </summary>
        /// <param name="or">OrCriteria to rewrite.</param>
        /// <returns>BoolCriteria with the criteria from the Or mapped into Should.</returns>
        private static BoolCriteria Rewrite(OrCriteria or)
        {
            return new BoolCriteria(null, or.Criteria.Select(Compensate), null);
        }

        /// <summary>
        /// Rewrite an AndCriteria as a BoolCriteria.
        /// </summary>
        /// <param name="and">AndCriteria to rewrite.</param>
        /// <returns>BoolCriteria with the criteria from the And mapped into Must.</returns>
        private static BoolCriteria Rewrite(AndCriteria and)
        {
            var should = and.Criteria.OfType<OrCriteria>().ToList();
            var mustNot = and.Criteria.OfType<NotCriteria>().ToList();
            var must = and.Criteria.Except(should).Except(mustNot);

            return new BoolCriteria(must.Select(Compensate),
                should.SelectMany(c => c.Criteria).Select(Compensate),
                mustNot.Select(c => c.Criteria).Select(Compensate));
        }

        /// <summary>
        /// Rewrite an ConstantCriteria as either a MatchAllCriteria or NotCriteria depending on whether it is true or false respectively.
        /// </summary>
        /// <param name="constant">Constant crieteria to rewrite.</param>
        /// <returns>MatchAllCriteria or NotCriteria wrapped MatchAllCriteria.</returns>
        private static ICriteria Rewrite(ConstantCriteria constant)
        {
            return constant == ConstantCriteria.True ? MatchAllCriteria.Instance : NotCriteria.Create(MatchAllCriteria.Instance);
        }
    }
}