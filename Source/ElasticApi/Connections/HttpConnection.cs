namespace ElasticApi.Connections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Newtonsoft.Json;

    public class HttpConnection : IConnection
    {
        public Uri Endpoint { get; private set; }

        public HttpConnection(Uri endpoint)
        {
            this.Endpoint = endpoint;
        }

        public TResponse Head<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters)
        {
            Uri uri = MakeUri(this.Endpoint, path, parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Head, uri))
            {
                return SendRequest<TResponse>(request, null);
            }
        }

        public TResponse Get<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters)
        {
            Uri uri = MakeUri(this.Endpoint, path, parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                return SendRequest<TResponse>(request, null);
            }
        }

        public TResponse Post<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters, object body)
        {
            Uri uri = MakeUri(this.Endpoint, path, parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                return SendRequest<TResponse>(request, body);
            }
        }

        public TResponse Put<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters, object body)
        {
            Uri uri = MakeUri(this.Endpoint, path, parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Put, uri))
            {
                return SendRequest<TResponse>(request, body);
            }
        }

        public TResponse Delete<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters)
        {
            Uri uri = MakeUri(this.Endpoint, path, parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Delete, uri))
            {
                return SendRequest<TResponse>(request, null);
            }
        }

        private static Uri MakeUri(Uri endpoint, IEnumerable<string> path, IDictionary<string, object> parameters)
        {
            var builder = new UriBuilder(endpoint);

            builder.Path = string.Join("/", path);

            builder.Query = string.Join("&", parameters.Select(p => p.Value == null ? p.Key : p.Key + "=" + p.Value));

            return builder.Uri;
        }

        private static TResponse SendRequest<TResponse>(HttpRequestMessage request, object body)
        {
            if (body != null)
            {
                var bodyData = JsonConvert.SerializeObject(body);

                request.Content = new StringContent(bodyData);

                Debug.WriteLine(">>>> {0} - {1} - {2}", request.Method, request.RequestUri, bodyData);
            }
            else
            {
                Debug.WriteLine(">>>> {0} - {1}", request.Method, request.RequestUri);
            }

            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.SendAsync(request).Result)
                {
                    Debug.WriteLine("<<<< HTTP RESPONSE ({0}) {1}", response.StatusCode, response.ReasonPhrase);
                    
                    using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        return ParseResponse<TResponse>(responseStream);
                    }
                }
            }
        }

        private static TResponse ParseResponse<TResponse>(Stream responseStream)
        {
            using (var reader = new StreamReader(responseStream))
            {
                string response = reader.ReadToEnd();

                Debug.WriteLine(string.Format("<<<< {0}", response));

                return JsonConvert.DeserializeObject<TResponse>(response);
            }
        }
    }
}
