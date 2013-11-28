// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;
using System.Collections;

namespace ElasticLinq.Request.Visitors
{
    internal class ElasticTranslateResult
    {
        private readonly ElasticSearchRequest searchRequest;
        private readonly Func<Hit, object> projector;
        private readonly Func<IList, object> finalTransform; 

        public ElasticTranslateResult(ElasticSearchRequest searchRequest, Func<Hit, object> projector, Func<IList, object> finalTransform = null)
        {
            this.searchRequest = searchRequest;
            this.projector = projector;
            this.finalTransform = finalTransform ?? (o => o);
        }

        public ElasticSearchRequest SearchRequest
        {
            get { return searchRequest; }
        }

        public Func<Hit, object> Projector
        {
            get { return projector; }
        }

        public Func<IList, object> FinalTransform
        {
            get { return finalTransform; }
        }
    }
}