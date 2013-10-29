// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response;
using ElasticLinq.Utility;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq
{
    /// <summary>
    /// Query provider implementation for ElasticSearch.
    /// </summary>
    public sealed class ElasticQueryProvider : IQueryProvider
    {
        private readonly ElasticConnection connection;
        private readonly IElasticMapping mapping;

        public TextWriter Log { get; set; }

        public ElasticQueryProvider(ElasticConnection connection, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("connection", connection);
            Argument.EnsureNotNull("mapping", mapping);

            this.connection = connection;
            this.mapping = mapping;
        }

        internal ElasticConnection Connection
        {
            get { return connection; }
        }

        internal IElasticMapping Mapping
        {
            get { return mapping; }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            Argument.EnsureNotNull("expresssion", expression);

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            return new ElasticQuery<T>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Argument.EnsureNotNull("expresssion", expression);

            var elementType = TypeHelper.GetSequenceElementType(expression.Type);
            var queryType = typeof(ElasticQuery<>).MakeGenericType(elementType);
            try
            {
                return (IQueryable)Activator.CreateInstance(queryType, new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Argument.EnsureNotNull("expresssion", expression);

            return (TResult)ExecuteInternal(expression);
        }

        public object Execute(Expression expression)
        {
            Argument.EnsureNotNull("expresssion", expression);

            return ExecuteInternal(expression);
        }

        private object ExecuteInternal(Expression expression)
        {
            var translation = ElasticQueryTranslator.Translate(mapping, expression);
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);

            var log = Log ?? new NullTextWriter();
            log.WriteLine("Type is " + elementType);

            var searchTask = new ElasticRequestProcessor(connection, log).Search(translation.SearchRequest);
            var response = searchTask.GetAwaiter().GetResult();

            return ElasticResponseMaterializer
                .Materialize(response.hits.hits, elementType, translation.Projector);
        }
    }
}