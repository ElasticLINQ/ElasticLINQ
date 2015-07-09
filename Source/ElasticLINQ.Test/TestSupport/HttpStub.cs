// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Test.TestSupport
{
    public class HttpStub : IDisposable
    {
        const int MaximumBindAttempts = 5;
        static readonly Random random = new Random();

        readonly List<HttpListenerRequest> requests = new List<HttpListenerRequest>();
        readonly List<HttpListenerResponse> responses = new List<HttpListenerResponse>();
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        readonly TaskCompletionSource<bool> responseCompletion = new TaskCompletionSource<bool>();
        readonly Action<HttpListenerContext> responder;
        readonly HttpListener listener;
        readonly int completeRequestCount;

        bool disposed;

        public Uri Uri { get { return new Uri(listener.Prefixes.Single()); } }

        public HttpStub(Action<HttpListenerContext> responder, int completeRequestCount)
        {
            this.responder = responder;
            this.completeRequestCount = completeRequestCount;

            var attemptsRemaining = MaximumBindAttempts;
            do
            {
                var randomPort = random.Next(49152, 65535);
                listener = new HttpListener();
                listener.Prefixes.Clear();
                listener.Prefixes.Add(string.Format("http://localhost:{0}/", randomPort));
                try
                {
                    listener.Start();
                    Task.Factory.StartNew(BackgroundLoop, cancellationTokenSource.Token);
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

        public Task Completion
        {
            get { return responseCompletion.Task; }
        }

        public ReadOnlyCollection<HttpListenerRequest> Requests
        {
            get { return requests.AsReadOnly(); }
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
                foreach (var response in responses)
                    response.Close();
                cancellationTokenSource.Dispose();
            }

            disposed = true;
        }

        void BackgroundLoop()
        {
            while (listener.IsListening && !cancellationTokenSource.IsCancellationRequested)
            {
                var contextAsync = listener.GetContextAsync();

                try
                {
                    contextAsync.Wait(cancellationTokenSource.Token);
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

                    if (responses.Count >= completeRequestCount)
                        responseCompletion.SetResult(true);
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