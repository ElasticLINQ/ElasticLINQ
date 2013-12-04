// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface a Criteria may optionally support if it
    /// knows of a way to negate it's effects without being
    /// wrapped in a NotCriteria.
    /// </summary>
    public interface INegatableCriteria
    {
        ICriteria Negate();
    }
}