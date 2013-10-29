// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using System.IO;
using ElasticLinq.Utility;

namespace ElasticLinq
{
    public class ElasticContext
    {
        private readonly ElasticConnection connection;
        private readonly ElasticQueryProvider provider;
        private readonly IElasticMapping mapping;

        public ElasticContext(ElasticConnection connection, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("connection", connection);
            Argument.EnsureNotNull("mapping", mapping);

            this.connection = connection;
            this.mapping = mapping;
            provider = new ElasticQueryProvider(connection, mapping);
        }

        public ElasticQuery<T> Query<T>()
        {
            return new ElasticQuery<T>(provider);
        }

        public ElasticConnection Connection
        {
            get { return connection; }
        }

        public IElasticMapping Mapping
        {
            get { return mapping; }
        }

        public TextWriter Log
        {
            get { return provider.Log; }
            set { provider.Log = value; }
        }
    }
}