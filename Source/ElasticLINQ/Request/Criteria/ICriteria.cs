// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface that all Criteria must implement to be part of
    /// the query filter tree.
    /// </summary>
    public interface ICriteria
    {
        /// <summary>
        /// Name of this criteria as specified in the Elasticsearch DSL.
        /// </summary>
        string Name { get; }
    }
}