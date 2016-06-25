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
    class TermsFacet : IOrderableFacet
    {
        readonly string name;
        readonly ICriteria filter;
        readonly ReadOnlyCollection<string> fields;
        readonly int? size;

        public TermsFacet(string name, int? size, params string[] fields)
            : this(name, null, size, fields)
        {
        }

        public TermsFacet(string name, ICriteria filter, int? size, params string[] fields)
        {
            Argument.EnsureNotBlank(nameof(name), name);
            Argument.EnsureNotEmpty(nameof(fields), fields);

            this.name = name;
            this.filter = filter;
            this.size = size;
            this.fields = new ReadOnlyCollection<string>(fields);
        }

        public string Type { get { return "terms"; } }

        public string Name { get { return name; } }

        public ReadOnlyCollection<string> Fields { get { return fields; } }

        public ICriteria Filter { get { return filter; } }

        public int? Size { get { return size; } }
    }
}