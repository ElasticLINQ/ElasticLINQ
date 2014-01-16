// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes multiple ElasticSearch hits into the required
    /// C# list of objects.
    /// </summary>
    internal class ElasticFacetsMaterializer : IElasticMaterializer
    {
        private readonly List<Func<ElasticResponse, object>> itemCreators;

        public ElasticFacetsMaterializer(List<Func<ElasticResponse, object>> itemCreators)
        {
            this.itemCreators = itemCreators;
        }

        public object Materialize(ElasticResponse elasticResponse)
        {
            return itemCreators.Select(i => i(elasticResponse)).ToList();
        }
    }
}