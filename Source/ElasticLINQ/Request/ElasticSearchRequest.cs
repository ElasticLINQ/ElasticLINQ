// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System.Collections.Generic;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Represents a search request to be sent to ElasticSearch.
    /// </summary>
    internal class ElasticSearchRequest
    {
        private readonly string type;
        private readonly int @from;
        private readonly int? size;
        private readonly List<string> fields;
        private readonly List<SortOption> sortOptions;
        private readonly ICriteria filter;
        private readonly ICriteria query;

        public ElasticSearchRequest(string type, int @from = 0, int? size = null, List<string> fields = null,
            List<SortOption> sortOptions = null, ICriteria filter = null, ICriteria query = null)
        {
            this.type = type;
            this.@from = @from;
            this.size = size;
            this.fields = fields ?? new List<string>();
            this.sortOptions = sortOptions ?? new List<SortOption>();
            this.filter = filter;
            this.query = query;
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

        public ICriteria Filter
        {
            get { return filter; }
        }

        public ICriteria Query
        {
            get { return query; }
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
