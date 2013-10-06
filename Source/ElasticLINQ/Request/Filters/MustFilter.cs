// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.Collections.Generic;

namespace ElasticLinq.Request.Filters
{
    internal class MustFilter : IFilter
    {
        private readonly List<IFilter> entries;

        public MustFilter(IEnumerable<IFilter> entries)
        {
            this.entries = new List<IFilter>(entries);
        }

        public string Name { get { return "must"; } }

        public IReadOnlyList<IFilter> Entries
        {
            get { return entries.AsReadOnly(); }
        }
    }
}