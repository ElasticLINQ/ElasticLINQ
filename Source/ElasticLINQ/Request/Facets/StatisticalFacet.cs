// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Request.Facets
{
    internal class StatisticalFacet : IFacet
    {
        private readonly string name;
        private readonly List<string> fields;

        public StatisticalFacet(string name, params string[] fields)
        {
            this.name = name;
            this.fields = fields.ToList();
        }

        public string Name { get { return name; } }

        public string Type { get { return "statistical"; } }

        public IReadOnlyList<string> Fields { get { return fields; } }
    }
}