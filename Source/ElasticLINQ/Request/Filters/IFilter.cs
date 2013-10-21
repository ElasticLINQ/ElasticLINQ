// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Interface that all Filters must implement to be part of
    /// the filtering tree for ElasticSearch.
    /// </summary>
    internal interface IFilter
    {
        string Name { get; }
    }
}