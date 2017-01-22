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
    class ElasticTranslateResult
    {
        public ElasticTranslateResult(SearchRequest searchRequest, IElasticMaterializer materializer)
        {
            SearchRequest = searchRequest;
            Materializer = materializer;
        }

        public SearchRequest SearchRequest { get; }

        public IElasticMaterializer Materializer { get; }
    }
}