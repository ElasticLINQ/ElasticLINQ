// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface that all criteria must implement to be part of
    /// the query tree.
    /// </summary>
    public interface ICriteria
    {
        /// <summary>
        /// Name of this criteria as specified in the Elasticsearch DSL.
        /// </summary>
        string Name { get; }
    }
}