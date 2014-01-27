// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void SearchRequestTypeIsSetFromType()
        {
            var actual = Mapping.GetTypeName(typeof(Robot));

            var translation = ElasticQueryTranslator.Translate(Mapping, Robots.Expression);

            Assert.Equal(actual, translation.SearchRequest.Type);
        }

        [Fact]
        public void TypeSelectionCriteriaIsAddedWhenNoOtherCriteria()
        {
            var mapping = new CouchbaseElasticMapping();

            var translation = ElasticQueryTranslator.Translate(mapping, Robots.Expression);

            Assert.IsType<ExistsCriteria>(translation.SearchRequest.Filter);
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
        public void SimpleSelectProducesValidMaterializer()
        {
            var translation = ElasticQueryTranslator.Translate(Mapping, Robots.Expression);
            var response = new ElasticResponse { hits = new Hits { hits = new List<Hit>() } };

            Assert.NotNull(translation.Materializer);
            var materialized = translation.Materializer.Materialize(response);
            Assert.IsAssignableFrom<IEnumerable<Robot>>(materialized);
            Assert.Empty((IEnumerable<Robot>)materialized);
        }

        [Theory]
        [InlineData(42, 2112)]
        [InlineData(2112, 42)]
        public void TakeTranslatesToSmallestSize(int size1, int size2)
        {
            var expectedSize = Math.Min(size1, size2);

            var taken = Robots.Take(size1).Take(size2);
            var translation = ElasticQueryTranslator.Translate(Mapping, taken.Expression);

            Assert.Equal(expectedSize, translation.SearchRequest.Size);
        }
   }
}