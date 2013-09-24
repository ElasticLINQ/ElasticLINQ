// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using IQToolkit.Data.ElasticSearch;
using Xunit;

namespace IQToolkit.Test.ElasticSearch.TypeSystem
{
    public class ElasticConnectionTests
    {
        [Fact]
        public void FireOffConnectionSpike()
        {
            var connection = new ElasticConnection(new Uri("http://10.81.84.12:9200/"));
            var provider = new ElasticQueryProvider(connection, )

        }
    }

    public class SimpleModel
    {
        private readonly IEntityProvider provider;

        public SimpleModel(IEntityProvider provider)
        {
            this.provider = provider;
        }

        public IEntityProvider Provider
        {
            get { return this.provider; }
        }

        public virtual IEntityTable<Customer> Customers
        {
            get { return this.provider.GetTable<Customer>("Customers"); }
        }
    }
}
