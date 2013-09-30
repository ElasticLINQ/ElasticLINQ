// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit;
using System;
using System.Linq.Expressions;

namespace ElasticLinq
{
    public class ElasticQuery<T> : Query<T>
    {
        public ElasticQuery(ElasticQueryProvider provider)
            : base(provider)
        {
        }

        public ElasticQuery(ElasticQueryProvider provider, Type staticType)
            : base(provider, staticType)
        {
        }

        public ElasticQuery(ElasticQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}
