// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElasticLinq.Request.Facets
{
    internal class TermsFacet : IOrderableFacet
    {
        private readonly string name;
        private readonly ICriteria criteria;
        private readonly IReadOnlyList<string> fields;
        private readonly int? size;

        public TermsFacet(string name, params string[] fields)
            : this(name, null, null, fields)
        {
        }

        public TermsFacet(string name, ICriteria criteria, int? size, params string[] fields)
        {
            this.name = name;
            this.criteria = criteria;
            this.size = size;
            this.fields = new ReadOnlyCollection<string>(fields);
        }

        public string Type { get { return "terms"; } }
        public string Name { get { return name; } }
        public IReadOnlyList<string> Fields { get { return fields; } }
        public ICriteria Filter { get { return criteria; } }
        public int? Size { get { return size; } }
    }
}