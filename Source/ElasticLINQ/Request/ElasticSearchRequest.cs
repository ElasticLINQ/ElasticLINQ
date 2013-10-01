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
        private readonly int skip;
        private readonly int? take;
        private readonly List<string> fields;
        private readonly List<SortOption> sortOptions;
        private readonly Dictionary<string, IReadOnlyList<object>> termCriteria;

        public ElasticSearchRequest(string type, int skip, int? take, List<string> fields,
            List<SortOption> sortOptions, Dictionary<string, IReadOnlyList<object>> termCriteria)
        {
            this.type = type;
            this.skip = skip;
            this.take = take;
            this.fields = fields;
            this.sortOptions = sortOptions;
            this.termCriteria = termCriteria;
        }

        public long Skip { get { return skip; } }
        public long? Take { get { return take; } }
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
