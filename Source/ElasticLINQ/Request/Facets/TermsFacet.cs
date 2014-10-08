// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Represents a terms facet.
    /// Terms facets return count information for terms.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => a.Something).Select(a => a.Count())</remarks>
    [DebuggerDisplay("TermsFacet {Fields} {Filter}")]
    internal class TermsFacet : IOrderableFacet
    {
        private readonly string name;
        private readonly ICriteria criteria;
        private readonly ReadOnlyCollection<string> fields;
        private readonly int? size;

        public TermsFacet(string name, params string[] fields)
            : this(name, null, null, fields)
        {
        }

        public TermsFacet(string name, ICriteria criteria, int? size, params string[] fields)
        {
            Argument.EnsureNotBlank("name", name);
            Argument.EnsureNotEmpty("fields", fields);

            this.name = name;
            this.criteria = criteria;
            this.size = size;
            this.fields = new ReadOnlyCollection<string>(fields);
        }

        public string Type { get { return "terms"; } }
        public string Name { get { return name; } }
        public ReadOnlyCollection<string> Fields { get { return fields; } }
        public ICriteria Filter { get { return criteria; } }
        public int? Size { get { return size; } }
    }
}