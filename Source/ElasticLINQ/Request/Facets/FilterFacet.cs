// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Represents a filter facet.
    /// Filter facets return the number of documents that  match the specified filter criteria.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => 1).Select(a => a.Sum(b => b.Field))</remarks>
    [DebuggerDisplay("FilterFacet {" + nameof(Filter) + "}")]
    class FilterFacet : IFacet
    {
        readonly string name;
        readonly ICriteria filter;

        public FilterFacet(string name, ICriteria filter)
        {
            Argument.EnsureNotBlank(nameof(name), name);

            this.name = name;
            this.filter = filter;
        }

        public string Type => "filter";
        public string Name => name;
        public ICriteria Filter => filter;
    }
}