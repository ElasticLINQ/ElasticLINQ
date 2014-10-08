// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class MissingCriteriaTests
    {
        [Fact]
        public void NamePropertyIsMissing()
        {
            var criteria = new MissingCriteria("something");

            Assert.Equal("missing", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            const string field = "myField";

            var criteria = new MissingCriteria(field);

            Assert.Equal(field, criteria.Field);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MissingCriteria(null));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new MissingCriteria(" "));
        }

        [Fact]
        public void ToStringContainsFieldComparisonAndValue()
        {
            var criteria = new MissingCriteria("thisIsAMissingField");
            var result = criteria.ToString();

            Assert.Contains(criteria.Field, result);
        }
   }
}