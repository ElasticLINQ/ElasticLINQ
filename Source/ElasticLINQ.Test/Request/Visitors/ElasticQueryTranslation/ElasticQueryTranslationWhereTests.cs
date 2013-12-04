// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationWhereTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void ObjectEqualsMethodFromConstantGeneratesTermCriteria()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void ObjectEqualsMethodFromMemberGeneratesTermCriteria()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void ObjectStaticEqualsMethodGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            // ReSharper disable once RedundantNameQualifier
            var where = Robots.Where(e => Object.Equals(e.Name, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringEqualsOperatorGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => e.Name == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringEqualsMethodFromMemberGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => e.Name.Equals(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringEqualsMethodFromConstantGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringEqualsOperatorReversedArgumentsGeneratesTermCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => expectedConstant == e.Name);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringStaticEqualsMethodGeneratesTermCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => String.Equals(e.Name, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringStaticEqualsMethodReversedArgumentsGeneratesTermCriteria()
        {
            const string expectedConstant = "IG-88";
            var where = Robots.Where(e => String.Equals(expectedConstant, e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void StringArrayExistingContainsGeneratesTermsCriteria()
        {
            var expectedNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => expectedNames.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedNames.Length, termCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void StringArrayInlineContainsGeneratesTermsCriteria()
        {
            var expectedNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => new[] { "Robbie", "Kryten", "IG-88", "Marvin" }.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedNames.Length, termCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void StringListExistingContainsGeneratesTermsCriteria()
        {
            var expectedNames = new List<string> { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => expectedNames.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedNames.Count, termCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void StringListInlineContainsGeneratesTermsCriteria()
        {
            var expectedNames = new List<string> { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => new List<string> { "Robbie", "Kryten", "IG-88", "Marvin" }.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedNames.Count, termCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void DecimalEqualsOperatorGeneratesTermCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("cost", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void DecimalStaticEqualsMethodGeneratesTermCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => decimal.Equals(e.Cost, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("cost", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void DecimalArrayContainsGeneratesTermsCriteria()
        {
            var expectedHourlyWages = new[] { 1.1m, 3.2m, 4.9m, 7.6m, 8.9m, 12345.678m };
            var where = Robots.Where(e => expectedHourlyWages.Contains(e.Cost));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("cost", termCriteria.Field);
            Assert.Equal(expectedHourlyWages.Length, termCriteria.Values.Count);
            foreach (var term in expectedHourlyWages)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public void DecimalLessThanOperatorGeneratesLessThanRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost < expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public void DecimalLessThanOrEqualsOperatorGeneratesLessThanOrEqualRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost <= expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public void DecimalGreaterThanOperatorGeneratesGreaterThanRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost > expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public void DecimalGreaterThanOrEqualsOperatorGeneratesGreaterThanOrEqualRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost >= expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanAndLessThanGeneratesGreaterThanOrLessThanRangeCriteria()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Robots.Where(e => e.Cost > expectedLower && e.Cost < expectedUpper);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedLower));
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedUpper));
            Assert.Equal(2, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOrEqualAndLessThanOrEqualGeneratesGreaterThanOrEqualLessThanOrEqualRangeCriteria()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Robots.Where(e => e.Cost >= expectedLower && e.Cost <= expectedUpper);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeCriteria>(criteria);
            var rangeCriteria = (RangeCriteria)criteria;
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedLower));
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedUpper));
            Assert.Equal(2, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void DoubleEqualsOperatorGeneratesTermCriteria()
        {
            const double expectedConstant = 710.956;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var where = Robots.Where(e => e.EnergyUse == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("energyUse", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void DoubleStaticEqualsMethodGeneratesTermCriteria()
        {
            const double expectedConstant = 710.956;
            var where = Robots.Where(e => double.Equals(e.EnergyUse, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("energyUse", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void DoubleArrayContainsGeneratesTermsCriteria()
        {
            var expectedValues = new[] { 1.1, 3.2, 4.9, 7.6, 8.9, 12345.678 };
            var where = Robots.Where(e => expectedValues.Contains(e.EnergyUse));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("energyUse", termCriteria.Field);
            Assert.Equal(expectedValues.Length, termCriteria.Values.Count);
            foreach (var term in expectedValues)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void Int32EqualsOperatorGeneratesTermCriteria()
        {
            const Int32 expectedConstant = 808;
            var where = Robots.Where(e => e.Id == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("id", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void Int32StaticEqualsMethodGeneratesTermCriteria()
        {
            const Int32 expectedConstant = 98004;
            var where = Robots.Where(e => int.Equals(e.Id, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("id", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void Int32ArrayContainsGeneratesTermsCriteria()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 45678 };
            var where = Robots.Where(e => expectedIds.Contains(e.Id));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("id", termCriteria.Field);
            Assert.Equal(expectedIds.Length, termCriteria.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void DateTimeEqualsOperatorGeneratesTermCriteria()
        {
            var expectedConstant = DateTime.Now;
            var where = Robots.Where(e => e.Started == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("started", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void DateTimeStaticEqualsMethodGeneratesTermCriteria()
        {
            var expectedConstant = DateTime.Now;
            var where = Robots.Where(e => DateTime.Equals(e.Started, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("started", termCriteria.Field);
            Assert.Equal(1, termCriteria.Values.Count);
            Assert.Equal(expectedConstant, termCriteria.Values[0]);
        }

        [Fact]
        public void DateTimeArrayContainsGeneratesTermsCriteria()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 4568 }.Select(d => DateTime.Now.AddDays(d)).ToArray();
            var where = Robots.Where(e => expectedIds.Contains(e.Started));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal("terms", termCriteria.Name);
            Assert.Equal("started", termCriteria.Field);
            Assert.Equal(expectedIds.Length, termCriteria.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termCriteria.Values);
        }

        [Fact]
        public void StringEqualsNullGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => e.Name == null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingCriteria>(criteria);
            var missingCriteria = (MissingCriteria)criteria;
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringEqualsNullNegatedGeneratesExistsCriteria()
        {
            // ReSharper disable once NegativeEqualityExpression
            var where = Robots.Where(e => !(e.Name == null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsCriteria>(criteria);
            var missingCriteria = (ExistsCriteria)criteria;
            Assert.Equal("exists", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringStaticEqualsNullMethodGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => String.Equals(e.Name, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingCriteria>(criteria);
            var missingCriteria = (MissingCriteria)criteria;
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringNotEqualsNullGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => e.Name != null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("name", existsCriteria.Field);
        }

        [Fact]
        public void StringNotEqualsNullNegatedGeneratesMissingCriteria()
        {
            // ReSharper disable once NegativeEqualityExpression
            var where = Robots.Where(e => !(e.Name != null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingCriteria>(criteria);
            var missingCriteria = (MissingCriteria)criteria;
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringStaticNotEqualsNullMethodGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => !String.Equals(e.Name, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("name", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntEqualsNullGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => e.Zone == null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingCriteria>(criteria);
            var missingCriteria = (MissingCriteria)criteria;
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("zone", missingCriteria.Field);
        }

        [Fact]
        public void NullableIntStaticEqualsNullMethodGeneratesMissingCriteria()
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            // ReSharper disable once RedundantNameQualifier
            var where = Robots.Where(e => Nullable<int>.Equals(e.Zone, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingCriteria>(criteria);
            var missingCriteria = (MissingCriteria)criteria;
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("zone", missingCriteria.Field);
        }

        [Fact]
        public void NullableIntNotEqualsNullGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => e.Zone != null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("zone", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntStaticNotEqualsNullMethodGeneratesExistsCriteria()
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            // ReSharper disable once RedundantNameQualifier
            var where = Robots.Where(e => !Nullable<int>.Equals(e.Zone, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("zone", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntNotValuePropertyGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => e.Zone.HasValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsCriteria>(criteria);
            var existsCriteria = (ExistsCriteria)criteria;
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("zone", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntNotHasValuePropertyGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => !e.Zone.HasValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingCriteria>(criteria);
            var missingCriteria = (MissingCriteria)criteria;
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("zone", missingCriteria.Field);
        }

        [Fact]
        public void OrOperatorGeneratesOrCriteria()
        {
            var where = Robots.Where(e => e.Name == "IG-88" || e.Cost > 1);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<OrCriteria>(criteria);
            var orCriteria = (OrCriteria)criteria;
            Assert.Equal("or", orCriteria.Name);
            Assert.Single(orCriteria.Criteria, f => f.Name == "term");
            Assert.Single(orCriteria.Criteria, f => f.Name == "range");
            Assert.Equal(2, orCriteria.Criteria.Count);
        }

        [Fact]
        public void OrOperatorGeneratesFlattenedOrCriteria()
        {
            var possibleIds = new[] { 1, 2, 3, 4 };
            var where = Robots.Where(e => e.Name == "IG-88" || e.Cost > 1 || e.Zone.HasValue || possibleIds.Contains(e.Id));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<OrCriteria>(criteria);
            var orCriteria = (OrCriteria)criteria;
            Assert.Equal("or", orCriteria.Name);
            Assert.Single(orCriteria.Criteria, f => f.Name == "term");
            Assert.Single(orCriteria.Criteria, f => f.Name == "range");
            Assert.Single(orCriteria.Criteria, f => f.Name == "exists");
            Assert.Single(orCriteria.Criteria, f => f.Name == "terms");
            Assert.Equal(4, orCriteria.Criteria.Count);
        }

        [Fact]
        public void AndOperatorGeneratesAndCriteria()
        {
            var where = Robots.Where(r => r.Name == "IG-88" && r.Cost > 1);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<AndCriteria>(criteria);
            var orCriteria = (AndCriteria)criteria;
            Assert.Equal("and", orCriteria.Name);
            Assert.Single(orCriteria.Criteria, f => f.Name == "term");
            Assert.Single(orCriteria.Criteria, f => f.Name == "range");
            Assert.Equal(2, orCriteria.Criteria.Count);
        }

        [Fact]
        public void AndOperatorChainsAndsTogetherIntoSingleAndCriteria()
        {
            var where = Robots.Where(r => r.Name == "IG-88" && r.Cost > 1 && r.Zone.HasValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<AndCriteria>(criteria);
            var andCriteria = (AndCriteria)criteria;
            Assert.Equal("and", andCriteria.Name);
            Assert.Single(andCriteria.Criteria, f => f.Name == "term");
            Assert.Single(andCriteria.Criteria, f => f.Name == "range");
            Assert.Single(andCriteria.Criteria, f => f.Name == "exists");
            Assert.Equal(3, andCriteria.Criteria.Count);
        }

        [Fact]
        public void RegexElasticMethodCreatesRegexWhereCriteria()
        {
            var where = Robots.Where(r => ElasticMethods.Regexp(r.Name, "r.*bot"));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RegexpCriteria>(criteria);
            var regexpCriteria = (RegexpCriteria)criteria;
            Assert.Equal("regexp", regexpCriteria.Name);
            Assert.Equal("name", regexpCriteria.Field);
            Assert.Equal("r.*bot", regexpCriteria.Regexp);
        }

        [Fact]
        public void QueryAndWhereGeneratesQueryAndFilterCriteria()
        {
            var query = Robots
                .Query(r => ElasticMethods.Regexp(r.Name, "r.*bot"))
                .Where(r => r.Zone.HasValue);

            var searchRequest = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<RegexpCriteria>(searchRequest.Query);
            Assert.IsType<ExistsCriteria>(searchRequest.Filter);
        }

        [Fact]
        public void QueryGeneratesQueryCriteria()
        {
            var where = Robots.Query(r => r.Name == "IG-88" && r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.IsType<AndCriteria>(request.Query);
            Assert.Null(request.Filter);
            var orCriteria = (AndCriteria)request.Query;
            Assert.Equal("and", orCriteria.Name);
            Assert.Single(orCriteria.Criteria, f => f.Name == "term");
            Assert.Single(orCriteria.Criteria, f => f.Name == "range");
            Assert.Equal(2, orCriteria.Criteria.Count);
        }

        [Fact]
        public void QueryStringGeneratesQueryStringCriteria()
        {
            const string expectedQueryStringValue = "Data";
            var where = Robots.QueryString(expectedQueryStringValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.Null(criteria.Filter);
            Assert.NotNull(criteria.Query);
            Assert.IsType<QueryStringCriteria>(criteria.Query);
            var queryStringCriteria = (QueryStringCriteria)criteria.Query;
            Assert.Equal(expectedQueryStringValue, queryStringCriteria.Value);
        }

        [Fact]
        public void QueryStringWithQueryCombinesToAndQueryCriteria()
        {
            const string expectedQueryStringValue = "Data";
            var where = Robots.QueryString(expectedQueryStringValue).Query(q => q.Cost > 0);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.Null(request.Filter);
            Assert.NotNull(request.Query);
            Assert.IsType<AndCriteria>(request.Query);
            var andCriteria = (AndCriteria)request.Query;
            Assert.Single(andCriteria.Criteria, a => a.Name == "query_string");
            Assert.Single(andCriteria.Criteria, a => a.Name == "range");
            Assert.Equal(2, andCriteria.Criteria.Count);
        }
    }
}