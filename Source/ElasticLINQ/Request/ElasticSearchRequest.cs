// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Represents a search request to ElasticSearch.
    /// </summary>
    internal class ElasticSearchRequest
    {
        private readonly string type;
        private readonly int @from;
        private readonly int? size;
        private readonly List<string> fields;
        private readonly List<SortOption> sortOptions;
        private readonly Dictionary<string, IReadOnlyList<object>> termCriteria;

        public ElasticSearchRequest(string type, int @from = 0, int? size = null, List<string> fields = null,
            List<SortOption> sortOptions = null, Dictionary<string, IReadOnlyList<object>> termCriteria = null)
        {
            this.type = type;
            this.@from = @from;
            this.size = size;
            this.fields = fields ?? new List<string>();
            this.sortOptions = sortOptions ?? new List<SortOption>();
            this.termCriteria = termCriteria ?? new Dictionary<string, IReadOnlyList<object>>();
        }

        public long @From { get { return @from; } }
        public long? Size { get { return size; } }
        public string Type { get { return type; } }

        public IReadOnlyList<string> Fields
        {
            get { return fields.AsReadOnly(); }
        }

        public IReadOnlyList<SortOption> SortOptions
        {
            get { return sortOptions.AsReadOnly(); }
        }

        public IReadOnlyDictionary<string, IReadOnlyList<object>> TermCriteria
        {
            get { return new ReadOnlyDictionary<string, IReadOnlyList<object>>(termCriteria); }
        }
    }

    internal class SortOption
    {
        private readonly string name;
        private readonly bool ascending;

        public SortOption(string name, bool ascending)
        {
            this.name = name;
            this.ascending = ascending;
        }

        public string Name { get { return name; } }
        public bool Ascending { get { return ascending; } }
    }
}
