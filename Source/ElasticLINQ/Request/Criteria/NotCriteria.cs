// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that inverts the logic of criteria it wraps.
    /// </summary>
    internal class NotCriteria : ICriteria, INegatableCriteria
    {
        private readonly ICriteria criteria;

        public static ICriteria Create(ICriteria criteria)
        {
            Argument.EnsureNotNull("criteria", criteria);

            // Allow some criteria to provide their own negation instead
            if (criteria is INegatableCriteria)
                return ((INegatableCriteria) criteria).Negate();

            return new NotCriteria(criteria);
        }

        private NotCriteria(ICriteria criteria)
        {
            this.criteria = criteria;
        }

        public string Name
        {
            get { return "not"; }
        }

        public ICriteria Criteria
        {
            get { return criteria; }
        }

        public ICriteria Negate()
        {
            return criteria;
        }

        public override string ToString()
        {
            return "not " + criteria;
        }
    }
}