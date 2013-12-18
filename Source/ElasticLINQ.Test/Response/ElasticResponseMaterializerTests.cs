// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response;
using ElasticLinq.Response.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Response
{
    public class ElasticResponseMaterializerTests
    {
        [Fact]
        public void SingleOrDefaultThrowsInvalidOperationExceptionGivenMoreThanOneResult()
        {
            var list = new List<SampleClass> { new SampleClass(), new SampleClass() };

            Assert.Throws<InvalidOperationException>(() => ElasticResponseMaterializer.Single(list, true));
        }

        [Fact]
        public void SingleOrDefaultReturnsDefaultGivenNoResults()
        {
            var list = new List<SampleClass>();

            var actual = ElasticResponseMaterializer.Single(list, true);

            Assert.IsType<SampleClass>(actual);
        }

        [Fact]
        public void SingleThrowsInvalidOperationExceptionGivenNoResults()
        {
            var list = new List<SampleClass>();

            Assert.Throws<InvalidOperationException>(() => ElasticResponseMaterializer.Single(list, false));
        }

        [Fact]
        public void SingleReturnsOnlyResultGivenOneResult()
        {
            var expected = new SampleClass();
            var list = new List<SampleClass> { expected };

            var actual = ElasticResponseMaterializer.Single(list, false);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void FirstOrDefaultReturnsDefaultGivenNoResults()
        {
            var list = new List<SampleClass>();

            var actual = ElasticResponseMaterializer.First(list, true);

            Assert.IsType<SampleClass>(actual);
        }

        [Fact]
        public void FirstThrowsInvalidOperationExceptionGivenNoResults()
        {
            var list = new List<SampleClass>();

            Assert.Throws<InvalidOperationException>(() => ElasticResponseMaterializer.First(list, false));
        }

        [Fact]
        public void FirstReturnsOnlyResultGivenOneResult()
        {
            var expected = new SampleClass();
            var list = new List<SampleClass> { expected };

            var actual = ElasticResponseMaterializer.First(list, false);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void FirstReturnsFirstResultGivenMoreThanOneResult()
        {
            var expected = new SampleClass();
            var list = new List<SampleClass> { expected, new SampleClass() };

            var actual = ElasticResponseMaterializer.First(list, false);

            Assert.Same(expected, actual);
        }

        private static Hit CreateHit(string sampleField)
        {
            return new Hit { fields = new Dictionary<string, JToken> { { "someField", new JProperty("a","b").Value } } };
        }

        private class SampleClass
        {
            public string SampleField { get; set; }
        }
    }
}