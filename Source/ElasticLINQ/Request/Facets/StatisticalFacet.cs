// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Represents a stastical facet.
    /// Statistical facets return all statistical information such
    /// as counts, sums, mean etc. for a given number of fields
    /// within the documents specified by the filter criteria.
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => 1).Select(a => a.Count(b => b.SomeField))</remarks>
    [DebuggerDisplay("StatisticalFacet {DebugFieldList} {Filter}")]
    class StatisticalFacet : IFacet
    {
        public StatisticalFacet(string name, params string[] fields)
            : this(name, null, fields)
        {
        }

        public StatisticalFacet(string name, ICriteria criteria, params string[] fields)
        {
            Argument.EnsureNotBlank(nameof(name), name);
            Argument.EnsureNotEmpty(nameof(fields), fields);

            Name = name;
            Filter = criteria;
            Fields = new ReadOnlyCollection<string>(fields);
        }

        public string Type => "statistical";

        public string Name { get; }

        public ICriteria Filter { get; }

        public ReadOnlyCollection<string> Fields { get; }

        string DebugFieldList => String.Join(", ", Fields);
    }
}