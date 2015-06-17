// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface a criteria may optionally support if it knows of a way to 
    /// negate its effects without being wrapped in a NotCriteria.
    /// </summary>
    public interface INegatableCriteria
    {
        /// <summary>
        /// Provide a negative representation of this criteria.
        /// </summary>
        /// <returns>Negative represenation of this criteria.</returns>
        ICriteria Negate();
    }
}