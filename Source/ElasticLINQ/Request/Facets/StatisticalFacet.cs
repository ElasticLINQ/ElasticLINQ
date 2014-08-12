// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Represents a stastical facet.
    /// Statistical facets return all statistical information such
    /// as counts, sums, mean etc. for a given number of fields
    /// within the documents specified by the filter criteria.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => 1).Select(a => a.Count(b => b.SomeField))</remarks>
    [DebuggerDisplay("StatisticalFacet {Fields} {Filter}")]
    internal class StatisticalFacet : IFacet
    {
        private readonly string name;
        private readonly ICriteria criteria;
        private readonly IReadOnlyList<string> fields;

        public StatisticalFacet(string name, params string[] fields)
            : this(name, null, fields)
        {
        }

        public StatisticalFacet(string name, ICriteria criteria, params string[] fields)
        {
            Argument.EnsureNotBlank("name", name);
            Argument.EnsureNotEmpty("fields", fields);

            this.name = name;
            this.criteria = criteria;
            this.fields = new ReadOnlyCollection<string>(fields);
        }

        public string Type { get { return "statistical"; } }
        public string Name { get { return name; } }
        public ICriteria Filter { get { return criteria; } }
        public IReadOnlyList<string> Fields { get { return fields; } }
    }
}