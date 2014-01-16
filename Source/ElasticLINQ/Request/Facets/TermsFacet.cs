// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Facets
{
    internal class TermsFacet : IFacet
    {
        private readonly string name;
        private readonly string field;

        public TermsFacet(string name, string field)
        {
            this.name = name;
            this.field = field;
        }

        public string Name { get { return name; } }

        public string Type { get { return "terms"; } }

        public string Field { get { return field; } }
    }
}