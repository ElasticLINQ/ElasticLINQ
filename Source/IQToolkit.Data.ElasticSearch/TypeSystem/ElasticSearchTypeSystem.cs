// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;
using System;

namespace IQToolkit.Data.ElasticSearch.TypeSystem
{
    public class ElasticSearchTypeSystem : QueryTypeSystem
    {
        public override QueryType Parse(string typeDeclaration)
        {
            // TODO: Develop a type system
            return new ElasticSearchQueryType(true, 4096, 18, 0);
        }

        public override QueryType GetColumnType(Type type)
        {
            throw new NotImplementedException();
        }

        public override string GetVariableDeclaration(QueryType type, bool suppressSize)
        {
            throw new NotImplementedException();
        }
    }
}
