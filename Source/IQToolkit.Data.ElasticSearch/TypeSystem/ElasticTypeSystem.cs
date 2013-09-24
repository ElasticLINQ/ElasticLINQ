// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;
using System;

namespace IQToolkit.Data.ElasticSearch.TypeSystem
{
    // TODO: Develop a type system
    public class ElasticTypeSystem : QueryTypeSystem
    {
        public override QueryType Parse(string typeDeclaration)
        {
            return new ElasticQueryType(true, 4096, 18, 0);
        }

        public override QueryType GetColumnType(Type type)
        {
            return new ElasticQueryType(true, 4096, 18, 0);
        }

        public override string GetVariableDeclaration(QueryType type, bool suppressSize)
        {
            throw new NotImplementedException();
        }
    }
}
