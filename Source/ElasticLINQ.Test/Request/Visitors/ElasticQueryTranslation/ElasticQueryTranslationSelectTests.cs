// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationSelectTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SelectAnonymousProjectionTranslatesToFields()
        {
            var selected = Robots.Select(r => new { r.Id, r.Cost });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("cost", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectAnonymousProjectionWithSomeIdentifiersTranslatesToFields()
        {
            var selected = Robots.Select(r => new { First = r.Id, Second = r.Started, r.Cost });
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
            var selected = Robots.Select(r => Tuple.Create(r.Id, r.Name));
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("name", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectSingleFieldTranslatesToField()
        {
            var selected = Robots.Select(r => r.Id);
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void SelectJustScoreTranslatesToField()
        {
            var selected = Robots.Select(r => ElasticFields.Score);
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("_score", fields);
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void SelectAnonymousScoreAndIdTranslatesToFields()
        {
            var selected = Robots.Select(r => new { ElasticFields.Id, ElasticFields.Score });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("_score", fields);
            Assert.Contains("_id", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectTupleScoreAndIdTranslatesToFields()
        {
            var selected = Robots.Select(r => Tuple.Create(ElasticFields.Id, ElasticFields.Score));
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("_score", fields);
            Assert.Contains("_id", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectAnonymousEntityDoesNotTranslateToFields()
        {
            var selected = Robots.Select(r => new { Robot = r, ElasticFields.Score });
            var translation = ElasticQueryTranslator.Translate(Mapping, selected.Expression);

            Assert.Empty(translation.SearchRequest.Fields);
        }

        [Fact]
        public void SelectTupleEntityDoesNotTranslateToFields()
        {
            var selected = Robots.Select(r => Tuple.Create(r, ElasticFields.Score));
            var translation = ElasticQueryTranslator.Translate(Mapping, selected.Expression);

            Assert.Empty(translation.SearchRequest.Fields);
        }
    }
}