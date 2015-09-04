// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Interface that all facets must implement to be part of
    /// the query facet tree.
    /// </summary>
    public interface IFacet
    {
        /// <summary>
        /// Name of this facet as specified in the Elasticsearch DSL
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type of this facet as specified in the Elasticsearch DSL
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Criteria of this facet
        /// </summary>
        ICriteria Filter { get; }
    }

    /// <summary>
    /// Interface that all orderable facets must implement to be part of
    /// the query facet tree.
    /// </summary>
    public interface IOrderableFacet : IFacet
    {
        /// <summary>
        /// Defines how many top terms should be returned out of the overall terms list
        /// </summary>
        int? Size { get; }
    }
}