// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void TypeIsSetFromType()
        {
            var actual = Mapping.GetTypeName(typeof(Employee));

            var query = Employees;
            var translation = ElasticQueryTranslator.Translate(Mapping, query.Expression);

            Assert.Equal(actual, translation.SearchRequest.Type);
        }

        [Fact]
        public void SkipTranslatesToFrom()
        {
            const int actual = 325;

            var skipped = Employees.Skip(actual);
            var translation = ElasticQueryTranslator.Translate(Mapping, skipped.Expression);

            Assert.Equal(actual, translation.SearchRequest.From);
        }

        [Fact]
        public void TakeTranslatesToSize()
        {
            const int actual = 73;

            var taken = Employees.Take(actual);
            var translation = ElasticQueryTranslator.Translate(Mapping, taken.Expression);

            Assert.Equal(actual, translation.SearchRequest.Size);
        }

        [Fact]
        public void SelectAnonymousProjectionTranslatesToFields()
        {
            var selected = Employees.Select(e => new { e.Id, e.HourlyWage });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("hourlyWage", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectAnonymousProjectionWithSomeIdentifiersTranslatesToFields()
        {
            var selected = Employees.Select(e => new { First = e.Id, Second = e.Hired, e.HourlyWage });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("hired", fields);
            Assert.Contains("hourlyWage", fields);
            Assert.Equal(3, fields.Count);
        }

        [Fact]
        public void SelectTupleProjectionWithIdentifiersTranslatesToFields()
        {
            var selected = Employees.Select(e => Tuple.Create(e.Id, e.Name));
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("name", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectSingleFieldTranslatesToField()
        {
            var selected = Employees.Select(e => e.Id);
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Equal(1, fields.Count);
        }
  }
}