// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Test.TestSupport
{
    [ExcludeFromCodeCoverage] // Fake for tests
    internal class FakeQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);
            var queryType = typeof(ElasticQuery<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(queryType, new object[] { this, expression });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new FakeQuery<TElement>(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>()
        {
            return new FakeQuery<TElement>(this);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)ExecuteInternal(expression);
        }

        public object Execute(Expression expression)
        {
            return ExecuteInternal(expression);
        }

        private object ExecuteInternal(Expression expression)
        {
            FinalExpression = expression;
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);
            var listType = typeof(List<>).MakeGenericType(elementType);
            return Activator.CreateInstance(listType);
        }

        public Expression FinalExpression { get; private set; }
    }
}