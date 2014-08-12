// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Materializers;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Represents the result of a translated query including the
    /// remote <see cref="Request.SearchRequest"/> to select the data
    /// and the local <see cref="IElasticMaterializer"/> necessary to
    /// instantiate objects.
    /// </summary>
    internal class ElasticTranslateResult
    {
        private readonly SearchRequest searchRequest;
        private readonly IElasticMaterializer materializer;

        public ElasticTranslateResult(SearchRequest searchRequest, IElasticMaterializer materializer)
        {
            this.searchRequest = searchRequest;
            this.materializer = materializer;
        }

        public SearchRequest SearchRequest
        {
            get { return searchRequest; }
        }

        public IElasticMaterializer Materializer
        {
            get { return materializer; }
        }
    }
}