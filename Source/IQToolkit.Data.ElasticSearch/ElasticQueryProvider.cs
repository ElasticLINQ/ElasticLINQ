// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;
using System;

namespace IQToolkit.Data.ElasticSearch
{
    public class ElasticQueryProvider : EntityProvider
    {
        private readonly ElasticConnection connection;

        public ElasticQueryProvider(ElasticConnection connection, QueryMapping mapping, QueryPolicy policy)
            : base(ElasticQueryLanguage.Default, mapping, policy)
        {
            this.connection = connection;
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new ElasticQueryExecutor(connection);
        }

        public override void DoTransacted(Action action)
        {
            action(); // No transaction support
        }

        public override void DoConnected(Action action)
        {
            action(); // No concept of open connections
        }

        public override int ExecuteCommand(string commandText)
        {
            throw new NotImplementedException();
        }
    }
}