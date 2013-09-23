// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using IQToolkit.Data.Common;

namespace IQToolkit.Data.ElasticSearch.TypeSystem
{
    public class ElasticSearchQueryType : QueryType
    {
        private readonly bool notNull;
        private readonly int length;
        private readonly short precision;
        private readonly short scale;

        public ElasticSearchQueryType(bool notNull, int length, short precision, short scale)
        {
            this.notNull = notNull;
            this.length = length;
            this.precision = precision;
            this.scale = scale;
        }

        public override bool NotNull
        {
            get { return notNull; }
        }

        public override int Length
        {
            get { return length; }
        }

        public override short Precision
        {
            get { return precision; }
        }

        public override short Scale
        {
            get { return scale; }
        }
    }
}
