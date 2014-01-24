// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Materializers;

namespace ElasticLinq.Request.Visitors
{
    /// <summary>
    /// Represents the result of a translated query including the
    /// remote <see cref="ElasticSearchRequest"/> to select the data
    /// and the local <see cref="IElasticMaterializer"/> necessary to
    /// instantiate objects.
    /// </summary>
    internal class ElasticTranslateResult
    {
        private readonly ElasticSearchRequest searchRequest;
        private readonly IElasticMaterializer materializer;

        public ElasticTranslateResult(ElasticSearchRequest searchRequest, IElasticMaterializer materializer)
        {
            this.searchRequest = searchRequest;
            this.materializer = materializer;
        }

        public ElasticSearchRequest SearchRequest
        {
            get { return searchRequest; }
        }

        public IElasticMaterializer Materializer
        {
            get { return materializer; }
        }
    }
}