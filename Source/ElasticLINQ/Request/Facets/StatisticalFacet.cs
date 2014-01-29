// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElasticLinq.Request.Facets
{
    internal class StatisticalFacet : IFacet
    {
        private readonly string name;
        private readonly IReadOnlyList<string> fields;

        public StatisticalFacet(string name, params string[] fields)
        {
            this.name = name;
            this.fields = new ReadOnlyCollection<string>(fields);
        }

        public string Type { get { return "statistical"; } }
        public string Name { get { return name; } }
        public IReadOnlyList<string> Fields { get { return fields; } }
        public ICriteria Filter { get; set; }
    }
}