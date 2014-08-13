// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Test.TestSupport
{
    public class HttpStub : IDisposable
    {
        private const int MaximumBindAttempts = 5;
        private static readonly Random random = new Random();

        private readonly List<HttpListenerRequest> requests = new List<HttpListenerRequest>();
        private readonly List<HttpListenerResponse> responses = new List<HttpListenerResponse>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Action<HttpListenerContext> responder;        
        private readonly HttpListener listener;

        private bool disposed;

        public Uri Uri { get { return new Uri(listener.Prefixes.Single()); } }

        public HttpStub(Action<HttpListenerContext> responder)
        {
            this.responder = responder;

            var attemptsRemaining = MaximumBindAttempts;
            do
            {
                var randomPort = random.Next(49152, 65535);
                listener = new HttpListener();
                listener.Prefixes.Clear();
                listener.Prefixes.Add(String.Format("http://localhost:{0}/", randomPort));
                try
                {
                    listener.Start();
                    Task.Factory.StartNew<Task>(BackgroundLoop, cancellationTokenSource.Token);
                    return;
                }
                catch (HttpListenerException)
                {
                    if (--attemptsRemaining == 0)
                        throw;
                }
            }
            while (true);
        }

        public IReadOnlyList<HttpListenerRequest> Requests
        {
            get { return requests.AsReadOnly(); }
        }

        public IReadOnlyList<HttpListenerResponse> Responses
        {
            get { return responses.AsReadOnly(); }
        }

        public void Dispose()
        {
            Dispose(true);           
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                cancellationTokenSource.Cancel();
                listener.Close();
                foreach(var response in responses)
                    response.Close();
                cancellationTokenSource.Dispose();
            }

            disposed = true;
        }

        private async Task BackgroundLoop()
        {
            while (listener.IsListening)
            {
                var contextAsync = listener.GetContextAsync();
                if (cancellationTokenSource.IsCancellationRequested)
                    return;

                try
                {
                    await contextAsync;
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (contextAsync.IsCompleted)
                {
                    var context = contextAsync.Result;
                    context.Response.StatusCode = 200;
                    responder(context);
                    requests.Add(context.Request);
                    responses.Add(context.Response);
                    // Closing response would dispose the request objects we need
                    context.Response.OutputStream.Close();
                }
            }
        }
    }

    public static class HttpStubExtensions
    {
        public static void Write(this HttpListenerResponse response, string output)
        {
            using (var writer = new StreamWriter(response.OutputStream))
                writer.Write(output);
        }
    }
}