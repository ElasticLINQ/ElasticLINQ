// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using System;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Expressions
{
    internal class FilterExpression : Expression
    {
        private readonly IFilter filter;

        public FilterExpression(IFilter filter)
        {
            this.filter = filter;
        }

        public IFilter Filter { get { return filter; } }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)10000; }
        }

        public override Type Type
        {
            get { return typeof(bool); }
        }

        public override string ToString()
        {
            return filter.ToString();
        }
    }
}
