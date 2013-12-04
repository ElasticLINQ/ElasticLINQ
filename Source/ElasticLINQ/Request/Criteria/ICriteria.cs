// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface that all Criteria must implement to be part of
    /// the query filter tree for ElasticSearch.
    /// </summary>
    public interface ICriteria
    {
        string Name { get; }
    }
}