// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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
            var formatter = RequestFormatter.Create(connection, searchRequest);
            var postFormatter = formatter as PostBodyRequestFormatter;
            var isPost = postFormatter != null;

            var message = new HttpRequestMessage(isPost ? HttpMethod.Post : HttpMethod.Get, formatter.Uri);
            log.WriteLine("Request " + message.Method + " " + message.RequestUri);

            if (isPost)
            {
                var body = postFormatter.Body;
                message.Content = new StringContent(body);
                log.WriteLine("Body\n" + body);
            }

            return message;
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage)
        {
            using (var httpClient = new HttpClient { Timeout = connection.Timeout })
            {
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