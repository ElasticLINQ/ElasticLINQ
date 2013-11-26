// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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

        private readonly HttpListener listener = new HttpListener();
        private readonly List<HttpListenerRequest> requests = new List<HttpListenerRequest>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Action<HttpListenerContext> responder;

        private bool disposed;

        public Uri Uri { get { return new Uri(listener.Prefixes.Single()); } }

        public HttpStub(Action<HttpListenerContext> responder)
        {
            this.responder = responder;

            var attemptsRemaining = MaximumBindAttempts;
            do
            {
                var randomPort = random.Next(49152, 65535);
                listener.Prefixes.Clear();
                listener.Prefixes.Add(String.Format("http://localhost:{0}/", randomPort));
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

        public IReadOnlyList<HttpListenerRequest> Requests
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
                listener.Stop();
                cancellationTokenSource.Dispose();
            }

            disposed = true;
        }

        private void BackgroundLoop()
        {
            while (listener.IsListening)
            {
                var contextAsync = listener.GetContextAsync();
                if (cancellationTokenSource.IsCancellationRequested)
                    return;

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
                    context.Response.Close();
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