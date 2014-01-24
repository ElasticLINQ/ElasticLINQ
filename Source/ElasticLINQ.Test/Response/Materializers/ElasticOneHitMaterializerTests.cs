// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Materializers;
using ElasticLinq.Response.Model;
using System;
using Xunit;

namespace ElasticLinq.Test.Response.Materializers
{
    public class ElasticOneHitMaterializerTests
    {
        [Fact]
        public void SingleThrowsInvalidOperationExceptionGivenTwoResults()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(2);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(SampleClass), throwIfMoreThanOne: true, defaultIfNone: false);

            Assert.Throws<InvalidOperationException>(() => materializer.Materialize(response));
        }

        [Fact]
        public void SingleOrDefaultReturnsNullGivenNoResultsForAReferenceType()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(0);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(SampleClass), throwIfMoreThanOne: true, defaultIfNone: true);

            var actual = materializer.Materialize(response);

            Assert.Null(actual);
        }

        [Fact]
        public void SingleOrDefaultReturnsDefaultGivenNoResultsForAValueType()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(0);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(int), throwIfMoreThanOne: true, defaultIfNone: true);

            var actual = materializer.Materialize(response);

            Assert.Equal(default(int), actual);
        }

        [Fact]
        public void SingleThrowsInvalidOperationExceptionGivenNoResults()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(0);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(SampleClass), throwIfMoreThanOne: true, defaultIfNone: false);

            Assert.Throws<InvalidOperationException>(() => materializer.Materialize(response));
        }

        [Fact]
        public void SingleReturnsOnlyResultGivenOneResult()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(1);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(SampleClass), throwIfMoreThanOne: true, defaultIfNone: true);

            var actual = materializer.Materialize(response);

            Assert.Same(response.hits.hits[0], actual);
        }

        [Fact]
        public void FirstReturnsFirstResultGivenTwoResults()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(2);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(SampleClass), throwIfMoreThanOne: false, defaultIfNone: false);

            var actual = materializer.Materialize(response);

            Assert.Same(response.hits.hits[0], actual);
        }

        [Fact]
        public void FirstOrDefaultReturnsDefaultGivenNoResults()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(0);
            var materializer = new ElasticOneHitMaterializer(MaterializerTestHelper.ItemCreator, typeof(SampleClass), throwIfMoreThanOne: false, defaultIfNone: true);

            var actual = materializer.Materialize(response);

            Assert.Null(actual);
        }

        [Fact]
        public void FirstThrowsInvalidOperationExceptionGivenNoResults()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(0);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(Hit), throwIfMoreThanOne: false, defaultIfNone: false);

            Assert.Throws<InvalidOperationException>(() => materializer.Materialize(response));
        }

        [Fact]
        public void FirstReturnsOnlyResultGivenOneResult()
        {
            var response = MaterializerTestHelper.CreateSampleResponse(1);
            var materializer = new ElasticOneHitMaterializer(o => o, typeof(Hit), throwIfMoreThanOne: false, defaultIfNone: true);

            var actual = materializer.Materialize(response);

            Assert.Same(response.hits.hits[0], actual);
        }
    }
}