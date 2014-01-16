// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Facets
{
    internal class TermsStatsFacet : IFacet
    {
        private readonly string name;
        private readonly string key;
        private readonly string value;

        public TermsStatsFacet(string name, string key, string value)
        {
            this.name = name;
            this.key = key;
            this.value = value;
        }

        public string Name { get { return name; } }

        public string Type { get { return "terms_stats"; } }

        public string Key { get { return key; } }

        public string Value { get { return value; } }
    }
}