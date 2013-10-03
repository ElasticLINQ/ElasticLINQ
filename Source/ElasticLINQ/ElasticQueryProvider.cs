// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

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
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (mapping == null)
                throw new ArgumentNullException("mapping");

            this.connection = connection;
            this.mapping = mapping;
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
           if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");                

            return new ElasticQuery<T>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

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
            if (expression == null)
                throw new ArgumentNullException("expression");

            return (TResult)ExecuteInternal(expression);
        }

        public object Execute(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return ExecuteInternal(expression);
        }

        private object ExecuteInternal(Expression expression)
        {
            var translateResult = Translate(expression);
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);

            var log = Log ?? new NullTextWriter();
            log.WriteLine("Type is " + elementType);

            var response = new ElasticRequestProcessor(connection, log)
                .Search(translateResult.SearchRequest)
                .GetAwaiter().GetResult();

            return new ElasticResponseMaterializer().Materialize(response, elementType, translateResult.Projector);
        }

        private ElasticTranslateResult Translate(Expression expression)
        {
            return new ElasticQueryTranslator(mapping).Translate(expression);
        }
    }
}