// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;
using System;
using System.Linq.Expressions;
using System.Reflection;
using IQToolkit.Data.ElasticSearch.TypeSystem;

namespace IQToolkit.Data.ElasticSearch
{
    public class ElasticSearchQueryLanguage : QueryLanguage
    {
        readonly ElasticSearchTypeSystem typeSystem = new ElasticSearchTypeSystem();

        public override QueryTypeSystem TypeSystem
        {
            get { return typeSystem; }
        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            throw new NotImplementedException();
        }

        private static ElasticSearchQueryLanguage _default;

        public static ElasticSearchQueryLanguage Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new ElasticSearchQueryLanguage(), null);
                }
                return _default;
            }
        }
    }
}