// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Visitors;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    class Robot2 : Robot
    {
        public Robot2(int id, DateTime started)
        {
            Id = id;
            Started = started;
        }          
    }

    class RobotWithOs : Robot
    {
        public OperatingSystem OperatingSystem { get; set; }
    }

    class OperatingSystem
    {
        public string Name { get; set; }
    }

    public class ElasticQueryTranslationSelectTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SelectAnonymousProjectionTranslatesToFields()
        {
            var selected = Robots.Select(r => new { r.Id, r.Cost });
            var searchRequest = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest;

            Assert.Equal("robots", searchRequest.DocumentType);
            Assert.NotNull(searchRequest.Fields);
            Assert.Contains("id", searchRequest.Fields);
            Assert.Contains("cost", searchRequest.Fields);
            Assert.Equal(2, searchRequest.Fields.Count);
        }

        [Fact]
        public void SelectAnonymousProjectionTranslatesToNestedField()
        {
            var RobotsWithOs = new ElasticQuery<RobotWithOs>(SharedProvider);

            var selected = RobotsWithOs.Select(r => new { r.OperatingSystem.Name });
            var searchRequest = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest;

            Assert.Equal("robotWithOs", searchRequest.DocumentType);
            Assert.NotNull(searchRequest.Fields);
            Assert.Contains("operatingSystem.name", searchRequest.Fields);
            Assert.Equal(1, searchRequest.Fields.Count);
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
        public void SelectNewObjectWithConstructorArgsTranslatesToFields()
        {
            var selected = Robots.Select(r => new Robot2(r.Id, r.Started));
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("started", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectNewObjectWithMemberInitializersTranslatesToFields()
        {
            var selected = Robots.Select(r => new Robot { Id = r.Id, Started = r.Started });
            var fields = ElasticQueryTranslator.Translate(Mapping, selected.Expression).SearchRequest.Fields;

            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("started", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectNewObjectWithConstructorArgsAndMemberInitializersTranslatesToFields()
        {
            var selected = Robots.Select(r => new Robot2(r.Id, r.Started) { Cost = r.Cost });
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