// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple facet requests from the ElasticResponse.
    /// </summary>
    internal class ElasticFacetsMaterializer : IElasticMaterializer
    {
        private readonly List<Func<IDictionary<string, JToken>, object>> facetCreators;

        public ElasticFacetsMaterializer(List<Func<IDictionary<string, JToken>, object>> facetCreators, Type elementType)
        {
            this.facetCreators = facetCreators;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            return facetCreators.Select(i => i(elasticResponse.facets)).ToList();
        }
    }
}