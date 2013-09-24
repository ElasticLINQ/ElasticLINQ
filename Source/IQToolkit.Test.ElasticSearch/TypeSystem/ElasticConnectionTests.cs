// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using IQToolkit.Data.Common;
using IQToolkit.Data.ElasticSearch;
using IQToolkit.Data.Mapping;
using Xunit;

namespace IQToolkit.Test.ElasticSearch.TypeSystem
{
    public class ElasticConnectionTests
    {
        [Fact]
        public void FireOffConnectionSpike()
        {
            var connection = new ElasticConnection(new Uri("http://10.81.84.12:9200/"));
            var provider = new ElasticQueryProvider(connection, new ImplicitMapping(), new QueryPolicy());
        }
    }
}