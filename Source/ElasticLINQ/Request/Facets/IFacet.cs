// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;

namespace ElasticLinq.Request.Facets
{
    interface IFacet
    {
        string Name { get; }
        string Type { get; }
        ICriteria Filter { get; }
    }

    interface IOrderableFacet : IFacet
    {
        int? Size { get; }
    }
}