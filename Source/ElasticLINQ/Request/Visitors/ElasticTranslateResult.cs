// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Response.Model;
using System;

namespace ElasticLinq.Request.Visitors
{
    internal class ElasticTranslateResult
    {
        private readonly ElasticSearchRequest searchRequest;
        private readonly Func<Hit, object> projector;

        public ElasticTranslateResult(ElasticSearchRequest searchRequest, Func<Hit, object> projector)
        {
            this.searchRequest = searchRequest;
            this.projector = projector;
        }

        public ElasticSearchRequest SearchRequest
        {
            get { return searchRequest; }
        }

        public Func<Hit, object> Projector
        {
            get { return projector; }
        }
    }
}
