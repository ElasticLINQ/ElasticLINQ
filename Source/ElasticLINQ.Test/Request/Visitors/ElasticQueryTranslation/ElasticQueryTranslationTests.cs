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
            var actual = Mapping.GetTypeName(typeof(Robot));

            var translation = ElasticQueryTranslator.Translate(Mapping, Robots.Expression);

            Assert.Equal(actual, translation.SearchRequest.Type);
        }

        [Fact]
        public void SkipTranslatesToFrom()
        {
            const int actual = 325;

            var skipped = Robots.Skip(actual);
            var translation = ElasticQueryTranslator.Translate(Mapping, skipped.Expression);

            Assert.Equal(actual, translation.SearchRequest.From);
        }

        [Fact]
        public void TakeTranslatesToSize()
        {
            const int actual = 73;

            var taken = Robots.Take(actual);
            var translation = ElasticQueryTranslator.Translate(Mapping, taken.Expression);

            Assert.Equal(actual, translation.SearchRequest.Size);
        }

        [Fact]
        public void SelectAnonymousProjectionTranslatesToFields()
        {
            var selected = Robots.Select(e => new { e.Id, e.Cost });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("cost", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectAnonymousProjectionWithSomeIdentifiersTranslatesToFields()
        {
            var selected = Robots.Select(e => new { First = e.Id, Second = e.Started, e.Cost });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("started", fields);
            Assert.Contains("cost", fields);
            Assert.Equal(3, fields.Count);
        }

        [Fact]
        public void SelectTupleProjectionWithIdentifiersTranslatesToFields()
        {
            var selected = Robots.Select(e => Tuple.Create(e.Id, e.Name));
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("name", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectSingleFieldTranslatesToField()
        {
            var selected = Robots.Select(e => e.Id);
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Equal(1, fields.Count);
        }
  }
}