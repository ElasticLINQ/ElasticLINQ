// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Expressions
{
    /// <summary>
    /// An expression node that represents ElasticSearch criteria.
    /// </summary>
    internal class CriteriaExpression : Expression
    {
        private readonly ICriteria criteria;

        public CriteriaExpression(ICriteria criteria)
        {
            this.criteria = criteria;
        }

        public ICriteria Criteria { get { return criteria; } }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)10000; }
        }

        public override Type Type
        {
            get { return typeof(bool); }
        }

        public override bool CanReduce
        {
            get { return false; }
        }

        public override string ToString()
        {
            return criteria.ToString();
        }
    }
}