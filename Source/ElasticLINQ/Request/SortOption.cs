// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request
{
    internal class SortOption
    {
        private readonly string name;
        private readonly bool ascending;
        private readonly bool ignoreUnmapped;

        public SortOption(string name, bool ascending, bool ignoreUnmapped = false)
        {
            Argument.EnsureNotBlank("name", name);
            this.name = name;
            this.ascending = ascending;
            this.ignoreUnmapped = ignoreUnmapped;
        }

        public string Name
        {
            get { return name; }
        }

        public bool Ascending
        {
            get { return ascending; }
        }

        public bool IgnoreUnmapped
        {
            get { return ignoreUnmapped; }
        }
    }
}