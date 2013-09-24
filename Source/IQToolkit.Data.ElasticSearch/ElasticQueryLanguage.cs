// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;
using IQToolkit.Data.ElasticSearch.TypeSystem;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IQToolkit.Data.ElasticSearch
{
    public class ElasticQueryLanguage : QueryLanguage
    {
        readonly ElasticTypeSystem typeSystem = new ElasticTypeSystem();

        public override QueryTypeSystem TypeSystem
        {
            get { return typeSystem; }
        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            throw new NotImplementedException();
        }

        private static ElasticQueryLanguage _default;

        public static ElasticQueryLanguage Default
        {
            get
            {
                if (_default == null)
                    System.Threading.Interlocked.CompareExchange(ref _default, new ElasticQueryLanguage(), null);
                return _default;
            }
        }
    }
}