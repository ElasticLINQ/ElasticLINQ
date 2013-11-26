// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System;
using System.Diagnostics;

namespace ElasticLinq
{
    /// <summary>
    /// Specifies connection parameters for ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{Endpoint.ToString(),nq}{Index,nq}")]
    public class ElasticConnection
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);

        private readonly Uri endpoint;
        private readonly TimeSpan timeout = defaultTimeout;
        private readonly string index;

        public ElasticConnection(Uri endpoint)
        {
            Argument.EnsureNotNull("endpoint", endpoint);

            this.endpoint = endpoint;
        }

        public ElasticConnection(Uri endpoint, TimeSpan timeout)
            : this(endpoint)
        {
            Argument.EnsurePositive("timeout", timeout);
            this.timeout = timeout;
        }

        public ElasticConnection(Uri endpoint, TimeSpan timeout, string index)
            : this(endpoint, timeout)
        {
            Argument.EnsureNotBlank("index", index);
            this.index = index;
        }

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        public TimeSpan Timeout
        {
            get { return timeout; }
        }

        public string Index
        {
            get { return index; }
        }
    }
}