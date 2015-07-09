using System;
using System.Collections.Generic;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;

namespace ElasticLinq.Response.Materializers
{
    class HighlightElasticMaterializer : ChainMaterializer
    {
        public HighlightElasticMaterializer(IElasticMaterializer previous):base(previous)
        {
        }

        /// <summary>
        /// Add to response fields that needs to read highlighted info.
        /// </summary>
        /// <param name="response">ElasticResponse to obtain the existence of a result.</param>
        /// <returns>Return result of next materializer</returns>
        public override object Materialize(ElasticResponse response)
        {
            foreach (var hit in response.hits.hits)
            {
                if (hit.highlight==null) continue;
                foreach (var prop in hit.highlight.Properties())
                {
                    hit._source.Add(string.Format("{0}_highlight",prop.Name),prop.Value);
                }
            }
            
            return base.Materialize(response);
        }
    }
}