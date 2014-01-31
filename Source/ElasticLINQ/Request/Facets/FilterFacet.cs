// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;

namespace ElasticLinq.Request.Facets
{
    internal class FilterFacet : IFacet
    {
        private readonly string name;

        public FilterFacet(string name)
        {
            this.name = name;
        }

        public string Type { get { return "filter"; } }
        public string Name { get { return name; } }
        public ICriteria Filter { get; set; }
    }
}