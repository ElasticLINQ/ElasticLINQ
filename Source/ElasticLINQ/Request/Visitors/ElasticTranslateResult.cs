// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Represents the result of a query transation including both
    /// the remote <see cref="ElasticSearchRequest"/> and the local
    /// projection and transform functions.
    /// </summary>
    internal class ElasticTranslateResult
    {
        private readonly ElasticSearchRequest searchRequest;
        private readonly Func<ElasticResponse, object> materializer;

        public ElasticTranslateResult(ElasticSearchRequest searchRequest, Func<ElasticResponse, object> materializer)
        {
            this.searchRequest = searchRequest;
            this.materializer = materializer;
        }

        public ElasticSearchRequest SearchRequest
        {
            get { return searchRequest; }
        }

        public Func<ElasticResponse, object> Materializer
        {
            get { return materializer; }
        }
    }
}