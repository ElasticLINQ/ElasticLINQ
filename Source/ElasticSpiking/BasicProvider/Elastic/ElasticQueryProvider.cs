using IQToolkit.Data.ElasticSearch.Response;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticSpiking.BasicProvider
{
    public class ElasticQueryProvider : QueryProvider
    {
        private readonly Uri endpoint;

        public ElasticQueryProvider(Uri endpoint)
        {
            this.endpoint = endpoint;
        }

        public override object Execute(Expression expression)
        {
            var translated = Translate(expression);
            var elementType = TypeSystem.GetElementType(expression.Type);

            var response = Execute();
            response.Wait();

            var materialized = response.Result.hits.hits
                .Select(h => h._source.doc)
                .ToList();

            return materialized;
        }

        public override string GetQueryText(Expression expression)
        {
            return Translate(expression).CommandText;
        }

        private static TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new DbQueryTranslator().Translate(expression);
        }

        private async Task<ElasticResponse> Execute()
        {
            using (var httpClient = new HttpClient())
            using (var responseStream = await httpClient.GetStreamAsync(endpoint))
            {
                var serializer = new JsonSerializer();
                var textReader = new JsonTextReader(new StreamReader(responseStream));
                return serializer.Deserialize<ElasticResponse>(textReader);
            }
        }
    }
}