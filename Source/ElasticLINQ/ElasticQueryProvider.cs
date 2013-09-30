// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response;
using ElasticLinq.Utility;
using IQToolkit;
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
    public sealed class ElasticQueryProvider : IQueryProvider, IQueryText
    {
        private readonly ElasticConnection connection;
        private readonly IElasticMapping mapping;

        public TextWriter Log { get; set; }

        public ElasticQueryProvider(ElasticConnection connection, IElasticMapping mapping)
        {
            this.connection = connection;
            this.mapping = mapping;
        }

        public string GetQueryText(Expression expression)
        {
            var translateResult = Translate(expression);
            return ElasticRequestProcessor.BuildSearchUri(translateResult.SearchRequest, connection).ToString();
        }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new ElasticQuery<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type);
            var queryType = typeof(ElasticQuery<>).MakeGenericType(elementType);
            try
            {
                return (IQueryable)Activator.CreateInstance(queryType, new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        private object Execute(Expression expression)
        {
            var translateResult = Translate(expression);
            var elementType = TypeHelper.GetElementType(expression.Type);

            var log = Log ?? new NullTextWriter();
            log.WriteLine("Type is " + elementType);

            var response = new ElasticRequestProcessor(connection, log)
                .Search(translateResult.SearchRequest)
                .GetAwaiter().GetResult();

            return new ElasticResponseMaterializer().Materialize(response, elementType, translateResult.Projector);
        }

        private ElasticTranslateResult Translate(Expression expression)
        {
            expression = PartialEvaluator.Eval(expression, exp => PartialEvaluator.CanBeEvaluatedLocally(exp, this));
            return new ElasticQueryTranslator(mapping).Translate(expression);
        }
    }
}