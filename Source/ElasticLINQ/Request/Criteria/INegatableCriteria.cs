// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface a Criteria may optionally support if it
    /// knows of a way to negate it's effects without being
    /// wrapped in a NotCriteria.
    /// </summary>
    internal interface INegatableCriteria
    {
        ICriteria Negate();
    }
}