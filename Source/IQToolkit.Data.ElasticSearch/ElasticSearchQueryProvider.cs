// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;
using System;

namespace IQToolkit.Data.ElasticSearch
{
    public class ElasticSearchQueryProvider : EntityProvider
    {
        private readonly Uri connection;

        public ElasticSearchQueryProvider(Uri connection, QueryMapping mapping, QueryPolicy policy)
            : base(ElasticSearchQueryLanguage.Default, mapping, policy)
        {
            this.connection = connection;
        }

        protected override QueryExecutor CreateExecutor()
        {
            throw new NotImplementedException();
        }

        public override void DoTransacted(Action action)
        {
            throw new NotImplementedException();
        }

        public override void DoConnected(Action action)
        {
            throw new NotImplementedException();
        }

        public override int ExecuteCommand(string commandText)
        {
            throw new NotImplementedException();
        }
    }
}