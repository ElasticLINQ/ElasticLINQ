// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

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
        private string index;
        private readonly string password;
        private TimeSpan timeout = defaultTimeout;
        private readonly string userName;

        public ElasticConnection(Uri endpoint, string userName = null, string password = null)
        {
            Argument.EnsureNotNull("endpoint", endpoint);

            this.endpoint = endpoint;
            this.userName = userName;
            this.password = password;
        }

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        public string Index
        {
            get { return index; }
            set
            {
                Argument.EnsureNotBlank("value", value);
                index = value;
            }
        }

        public string Password
        {
            get { return password; }
        }

        public TimeSpan Timeout
        {
            get { return timeout; }
            set
            {
                Argument.EnsurePositive("value", value);
                timeout = value;
            }
        }

        public string UserName
        {
            get { return userName; }
        }
    }
}