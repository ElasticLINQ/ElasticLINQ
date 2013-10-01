// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Request.Formatter;
using ElasticLinq.Response.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Sends ElasticSearch requests and ensures the response is materialized.
    /// </summary>
    internal class ElasticRequestProcessor
    {
        private readonly ElasticConnection connection;
        private readonly TextWriter log;

        public ElasticRequestProcessor(ElasticConnection connection, TextWriter log)
        {
            this.connection = connection;
            this.log = log;
        }

        public async Task<ElasticResponse> Search(ElasticSearchRequest searchRequest)
        {
            var requestMessage = CreateRequestMessage(searchRequest);

            using (var response = await SendRequest(requestMessage))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
                return ParseResponse(responseStream);
        }

        private HttpRequestMessage CreateRequestMessage(ElasticSearchRequest searchRequest)
        {
            var formatter = SearchRequestFormatter.Create(connection, searchRequest);

            var postFormatter = formatter as PostBodySearchRequestFormatter;
            var requestMessage = new HttpRequestMessage(postFormatter != null ? HttpMethod.Post : HttpMethod.Get, formatter.Uri);
            if (postFormatter != null)
                requestMessage.Content = new StringContent(postFormatter.Body);
            return requestMessage;
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage)
        {
            using (var httpClient = new HttpClient { Timeout = connection.Timeout })
            {
                log.WriteLine("Request " + requestMessage.Method + " " + requestMessage.RequestUri);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var response = await httpClient.SendAsync(requestMessage);
                stopwatch.Stop();
                log.WriteLine("{0} response in {1}ms", response.StatusCode, stopwatch.ElapsedMilliseconds);
                return response;
            }
        }

        private ElasticResponse ParseResponse(Stream responseStream)
        {
            using (var streamReader = new StreamReader(responseStream))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var results = new JsonSerializer().Deserialize<ElasticResponse>(new JsonTextReader(streamReader));
                stopwatch.Stop();
                log.WriteLine("Deserialized {0} bytes into {1} objects in {2}ms",
                    responseStream.Length, results.hits.hits.Count, stopwatch.ElapsedMilliseconds);
                return results;
            }
        }

    }
}