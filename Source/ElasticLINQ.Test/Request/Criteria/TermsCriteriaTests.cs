// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class TermsCriteriaTests
    {
        [Fact]
        public static void OneValue_CreatesTermCriteria()
        {
            var criteria = TermsCriteria.Build("field", 1);

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("field", termCriteria.Field);
            Assert.Equal(1, termCriteria.Value);
        }

        [Fact]
        public static void MultipleValues_CreatesTermsCriteria()
        {
            var criteria = TermsCriteria.Build("field", 1, 2);

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("field", termsCriteria.Field);
            Assert.Equal(2, termsCriteria.Values.Count);
            Assert.Equal(1, termsCriteria.Values[0]);
            Assert.Equal(2, termsCriteria.Values[1]);
            Assert.Null(termsCriteria.ExecutionMode);
        }

        [Fact]
        public static void PassesThroughExecutionMode()
        {
            var criteria = TermsCriteria.Build(TermsExecutionMode.and, "field", 1, 2);

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal(TermsExecutionMode.and, termsCriteria.ExecutionMode);
        }

        [Fact]
        public static void ValuesAreNormalized()
        {
            var criteria = TermsCriteria.Build("field", 1, 2, 1, 1, 2, 9);

            Assert.Equal(3, criteria.Values.Count);
            Assert.Equal(1, criteria.Values[0]);
            Assert.Equal(2, criteria.Values[1]);
            Assert.Equal(9, criteria.Values[2]);
        }

        [Fact]
        public static void OneValue_ToString()
        {
            var criteria = TermsCriteria.Build("field", "single");
            var result = criteria.ToString();

            Assert.Equal("term field single", result);
        }

        [Fact]
        public static void MultipleValues_ToString()
        {
            var criteria = TermsCriteria.Build("field", "value1", "value2");
            var result = criteria.ToString();

            Assert.Equal("terms field [value1, value2]", result);
        }

        [Fact]
        public static void MultipleValues_WithExecutionMode_ToString()
        {
            var criteria = TermsCriteria.Build(TermsExecutionMode.plain, "field", "value1", "value2");
            var result = criteria.ToString();

            Assert.Equal("terms field [value1, value2] (execution: plain)", result);
        }
    }
}