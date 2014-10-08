// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class ExistsCriteriaTests
    {
        [Fact]
        public void NamePropertyIsExists()
        {
            var criteria = new ExistsCriteria("something");

            Assert.Equal("exists", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            const string field = "myField";

            var criteria = new ExistsCriteria(field);

            Assert.Equal(field, criteria.Field);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExistsCriteria(null));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenFieldIsBlank()
        {
            Assert.Throws<ArgumentException>(() => new ExistsCriteria(" "));
        }

        [Fact]
        public void ToStringContainsFieldComparisonAndValue()
        {
            var criteria = new ExistsCriteria("thisIsAMissingField");
            var result = criteria.ToString();

            Assert.Contains(criteria.Field, result);
        }
   }
}