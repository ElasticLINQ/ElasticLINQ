// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;
using System.Collections;

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
        private readonly Func<Hit, object> itemCreator;
        private readonly Func<IEnumerable, object> resultCreator; 

        public ElasticTranslateResult(ElasticSearchRequest searchRequest, Func<Hit, object> itemCreator, Func<IEnumerable, object> resultCreator)
        {
            this.searchRequest = searchRequest;
            this.itemCreator = itemCreator;
            this.resultCreator = resultCreator;
        }

        public ElasticSearchRequest SearchRequest
        {
            get { return searchRequest; }
        }

        public Func<Hit, object> ItemCreator
        {
            get { return itemCreator; }
        }

        public Func<IEnumerable, object> ResultCreator
        {
            get { return resultCreator; }
        }
    }
}