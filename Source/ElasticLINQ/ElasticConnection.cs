// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Diagnostics;
using ElasticLinq.Utility;

namespace ElasticLinq
{
    /// <summary>
    /// Specifies connection parameters for ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{Endpoint.ToString(),nq}{Index,nq}")]
    public class ElasticConnection
    {
        private readonly Uri endpoint;
        private readonly TimeSpan timeout;
        private readonly string index;
        private readonly bool preferGetRequests;

        public ElasticConnection(Uri endpoint, TimeSpan timeout, string index = null, bool preferGetRequests = false)
        {
            Argument.EnsureNotNull("endpoint", endpoint);
            Argument.EnsureNotNull("timeout", timeout);

            this.endpoint = endpoint;
            this.timeout = timeout;
            this.index = index ?? "";
            this.preferGetRequests = preferGetRequests;
        }

        public Uri Endpoint { get { return endpoint; } }

        public TimeSpan Timeout { get { return timeout; } }

        public string Index { get { return index; } }

        public bool PreferGetRequests { get { return preferGetRequests; } }
    }
}