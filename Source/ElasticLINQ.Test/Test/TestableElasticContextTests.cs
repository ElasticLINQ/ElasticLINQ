// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Test
{
    public class TestableElasticContextTests
    {
        [Fact]
        public void ConstructorWithNoArgsSetsDefaultProperties()
        {
            var context = new TestableElasticContext();

            Assert.NotNull(context.Connection);
            Assert.NotNull(context.Mapping);
            Assert.NotNull(context.Provider);
            Assert.NotNull(context.Requests);
            Assert.Equal(NullLog.Instance, context.Log);
            Assert.NotNull(context.RetryPolicy);
        }

        [Fact]
        public void ConstructorWithAllArgsSetsPropertiesFromParameters()
        {
            var expectedMapping = new ElasticMapping();
            var expectedLog = new SpyLog();
            const int expectedAttempts = 5;
            var expectedTimeout = TimeSpan.FromSeconds(21.3);

            var context = new TestableElasticContext(expectedMapping, expectedLog, expectedAttempts, expectedTimeout);

            Assert.NotNull(context.Connection);
            Assert.Equal(expectedMapping, context.Mapping);
            Assert.NotNull(context.Provider);
            Assert.NotNull(context.Requests);
            Assert.Equal(expectedLog, context.Log);
            var retryPolicy = Assert.IsType<RetryPolicy>(context.RetryPolicy);
            Assert.Equal(expectedAttempts, retryPolicy.MaxAttempts);
        }

        [Fact]
        public void SetDataWithIEnumerableSetsData()
        {
            var expected = new List<string> { "A", "B", "C" };
            var context = new TestableElasticContext();

            context.SetData(expected.AsEnumerable());

            var actual = context.Data<string>().ToList();

            Assert.Equal(expected.Count, actual.Count);

            foreach (var item in actual)
                Assert.Contains(item, actual);
        }

        [Fact]
        public void SetDataWithArraySetsData()
        {
            var expected = new List<string> { "A", "B", "C" };
            var context = new TestableElasticContext();

            context.SetData(expected.ToArray());

            var actual = context.Data<string>().ToList();

            Assert.Equal(expected.Count, actual.Count);

            foreach (var item in actual)
                Assert.Contains(item, actual);
        }

        [Fact]
        public void QueryOfTReturnsTestableQueryOfT()
        {
            var context = new TestableElasticContext();

            var query = context.Query<FakeClass>();

            Assert.IsType<TestableElasticQuery<FakeClass>>(query);
        }

        class FakeClass
        {            
        }
    }
}