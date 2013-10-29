// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;

namespace ElasticLinq.Request
{
    internal class SortOption
    {
        private readonly string name;
        private readonly bool ascending;
        private readonly bool ignoreUnmapped;

        public SortOption(string name, bool ascending, bool ignoreUnmapped = false)
        {
            Argument.EnsureNotBlank("name", name);
            this.name = name;
            this.ascending = ascending;
            this.ignoreUnmapped = ignoreUnmapped;
        }

        public string Name
        {
            get { return name; }
        }

        public bool Ascending
        {
            get { return ascending; }
        }

        public bool IgnoreUnmapped
        {
            get { return ignoreUnmapped; }
        }
    }
}