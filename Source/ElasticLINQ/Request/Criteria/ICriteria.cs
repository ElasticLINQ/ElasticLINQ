// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface that all Criteria must implement to be part of
    /// the query filter tree for ElasticSearch.
    /// </summary>
    internal interface ICriteria
    {
        string Name { get; }
    }
}