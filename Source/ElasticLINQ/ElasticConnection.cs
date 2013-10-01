// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;

namespace ElasticLinq
{
    /// <summary>
    /// Details of a connection to ElasticSearch.
    /// </summary>
    public class ElasticConnection
    {
        private readonly Uri endpoint;
        private readonly TimeSpan timeout;
        private readonly string index;
        private readonly bool preferGetRequests;

        public ElasticConnection(Uri endpoint, TimeSpan timeout, string index = null, bool preferGetRequests = false)
        {
            this.endpoint = endpoint;
            this.timeout = timeout;
            this.index = index;
            this.preferGetRequests = preferGetRequests;
        }

        public Uri Endpoint { get { return endpoint; } }

        public TimeSpan Timeout { get { return timeout; } }

        public string Index { get { return index; } }

        public bool PreferGetRequests { get { return preferGetRequests; } }
    }
}