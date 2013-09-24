// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;

namespace IQToolkit.Data.ElasticSearch
{
    public class ElasticConnection
    {
        private readonly string host;
        private readonly int port;
        private readonly string path;
        private readonly bool secure;
        private readonly TimeSpan timeout;

        public ElasticConnection(string host, int port = 9200, string path = null, bool secure = false, TimeSpan? timeout = null)
        {
            this.host = host;
            this.port = port;
            this.path = path;
            this.secure = secure;
            this.timeout = timeout ?? TimeSpan.FromSeconds(10);
        }

        public ElasticConnection(Uri connection)
        {
            host = connection.Host;
            port = connection.Port;
            path = connection.AbsolutePath;
            secure = connection.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
        }

        public string Host { get { return host; } }

        public int Port { get { return port; } }

        public string Path { get { return path; } }

        public bool Secure { get { return secure; } }

        public TimeSpan Timeout { get { return timeout; } }
    }
}