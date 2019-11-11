﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Test.TestSupport;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationWhereTests : ElasticQueryTranslationTestsBase
    {
        class Outer
        {
            public Guid Id { get; set; }
            public Inner Inner { get; set; }
        }

        class Inner
        {
            public Guid Id { get; set; }
        }

        [Fact]
        public void Where_On_Inner_Property_Generates_Correct_FieldName()
        {
            var where = new ElasticQuery<Outer>(SharedProvider).Where(o => o.Inner.Id == Guid.NewGuid());
            var criteria = ElasticQueryTranslator.Translate(new CouchbaseElasticMapping(), where.Expression).SearchRequest.Query;

            var andCriteria = Assert.IsType<AndCriteria>(criteria);
            var termsCriteria = andCriteria.Criteria.OfType<TermCriteria>().Single();
            Assert.Equal("doc.inner.id", termsCriteria.Field);
        }

        [Fact]
        public void EqualsTrueIsTranslatedToEquals()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name) == true);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void EqualsFalseIsTranslatedToNotEquals()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name) == false);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var notCriteria = Assert.IsType<NotCriteria>(criteria);
            var termCriteria = Assert.IsType<TermCriteria>(notCriteria.Criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void NegatedEqualsFalseIsTranslatedToEquals()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => !expectedConstant.Equals(e.Name) == false);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void NegatedEqualsTrueIsTranslatedToNotEquals()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => !expectedConstant.Equals(e.Name) == true);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var notCriteria = Assert.IsType<NotCriteria>(criteria);
            var termCriteria = Assert.IsType<TermCriteria>(notCriteria.Criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void NotEqualsFalseIsTranslatedToEquals()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name) != false);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void NotEqualsTrueIsTranslatedToNotEquals()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name) != true);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var notCriteria = Assert.IsType<NotCriteria>(criteria);
            var termCriteria = Assert.IsType<TermCriteria>(notCriteria.Criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void ObjectEqualsMethodFromConstantGeneratesTermCriteria()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void ObjectEqualsMethodFromMemberGeneratesTermCriteria()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void ObjectStaticEqualsMethodGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => Object.Equals(e.Name, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringEqualsOperatorGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => e.Name == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringEqualsMethodFromMemberGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => e.Name.Equals(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringEqualsMethodFromConstantGeneratesTermCriteria()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringEqualsOperatorReversedArgumentsGeneratesTermCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => expectedConstant == e.Name);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringContainsGeneratesQueryStringCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => e.Name.Contains(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria);
            Assert.Equal("name", queryStringCriteria.Fields.Single());
            Assert.Equal(string.Format("*{0}*", expectedConstant), queryStringCriteria.Value);
        }

        [Fact]
        public void StringStartsWithGeneratesQueryStringCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => e.Name.StartsWith(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria);
            Assert.Equal("name", queryStringCriteria.Fields.Single());
            Assert.Equal(string.Format("{0}*", expectedConstant), queryStringCriteria.Value);
        }

        [Fact]
        public void StringEndsWithGeneratesQueryStringCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => e.Name.EndsWith(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var queryStringCriteria = Assert.IsType<QueryStringCriteria>(criteria);
            Assert.Equal("name", queryStringCriteria.Fields.Single());
            Assert.Equal(string.Format("*{0}", expectedConstant), queryStringCriteria.Value);
        }

        [Fact]
        public void StringStaticEqualsMethodGeneratesTermCriteria()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => string.Equals(e.Name, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringStaticEqualsMethodReversedArgumentsGeneratesTermCriteria()
        {
            const string expectedConstant = "IG-88";
            var where = Robots.Where(e => string.Equals(expectedConstant, e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("name", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringArrayExistingContainsGeneratesTermsCriteria()
        {
            var expectedNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => expectedNames.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("name", termsCriteria.Field);
            Assert.Equal(expectedNames.Length, termsCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void StringArrayExistingContainsWithNullGeneratesOrWithTermsAndMissingCriteria()
        {
            const string expectedField = "name";
            var names = new[] { "Robbie", null, "IG-88", "Marvin" };
            var where = Robots.Where(e => names.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var orCriteria = Assert.IsType<OrCriteria>(criteria);
            Assert.Equal(2, orCriteria.Criteria.Count);

            var missingCriteria = orCriteria.Criteria.OfType<MissingCriteria>().Single();
            Assert.Equal(expectedField, missingCriteria.Field);

            var termsCriteria = orCriteria.Criteria.OfType<TermsCriteria>().Single();
            Assert.Equal(expectedField, termsCriteria.Field);
            Assert.Equal(names.Length - 1, termsCriteria.Values.Count);
            foreach (var term in names.Where(n => n != null))
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void StringArrayExistingContainsGeneratesTermCriteria()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => e.Aliases.Contains(expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("aliases", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void StringArrayInlineContainsGeneratesTermsCriteria()
        {
            var expectedNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => new[] { "Robbie", "Kryten", "IG-88", "Marvin" }.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("name", termsCriteria.Field);
            Assert.Equal(expectedNames.Length, termsCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void StringListExistingContainsGeneratesTermsCriteria()
        {
            var expectedNames = new List<string> { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => expectedNames.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("name", termsCriteria.Field);
            Assert.Equal(expectedNames.Count, termsCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void StringListInlineContainsGeneratesTermsCriteria()
        {
            var expectedNames = new List<string> { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => new List<string> { "Robbie", "Kryten", "IG-88", "Marvin" }.Contains(e.Name));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("name", termsCriteria.Field);
            Assert.Equal(expectedNames.Count, termsCriteria.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void DecimalEqualsOperatorGeneratesTermCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("cost", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void DecimalStaticEqualsMethodGeneratesTermCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => decimal.Equals(e.Cost, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("cost", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void DecimalArrayContainsGeneratesTermsCriteria()
        {
            var expectedHourlyWages = new[] { 1.1m, 3.2m, 4.9m, 7.6m, 8.9m, 12345.678m };
            var where = Robots.Where(e => expectedHourlyWages.Contains(e.Cost));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("cost", termsCriteria.Field);
            Assert.Equal(expectedHourlyWages.Length, termsCriteria.Values.Count);
            foreach (var term in expectedHourlyWages)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public void DecimalLessThanOperatorGeneratesLessThanRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost < expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public static void DecimalLessThanOperatorSupportsDeepProperties()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Stats.Pricing.InvoicePrice < expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("stats.pricing.invoicePrice", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        public void DecimalLessThanOrEqualsOperatorGeneratesLessThanOrEqualRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost <= expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        public static void DecimalLessThanOrEqualsOperatorSupportsDeepProperties()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Stats.Pricing.InvoicePrice <= expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("stats.pricing.invoicePrice", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        public void DecimalGreaterThanOperatorGeneratesGreaterThanRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost > expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public static void DecimalGreaterThanOperatorSupportsDeepProperties()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Stats.Pricing.InvoicePrice > expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("stats.pricing.invoicePrice", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public void DecimalGreaterThanOrEqualsOperatorGeneratesGreaterThanOrEqualRangeCriteria()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost >= expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression is not "executed"
        public static void DecimalGreaterThanOrEqualsOperatorSupportsDeepProperties()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Stats.Pricing.InvoicePrice >= expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("stats.pricing.invoicePrice", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Single(rangeCriteria.Specifications);
        }

        [Fact]
        public void DecimalGreaterThanAndLessThanGeneratesGreaterThanOrLessThanRangeCriteria()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Robots.Where(e => e.Cost > expectedLower && e.Cost < expectedUpper);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
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
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
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
            var where = Robots.Where(e => e.EnergyUse == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("energyUse", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void DoubleStaticEqualsMethodGeneratesTermCriteria()
        {
            const double expectedConstant = 710.956;
            var where = Robots.Where(e => double.Equals(e.EnergyUse, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("energyUse", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void DoubleArrayContainsGeneratesTermsCriteria()
        {
            var expectedValues = new[] { 1.1, 3.2, 4.9, 7.6, 8.9, 12345.678 };
            var where = Robots.Where(e => expectedValues.Contains(e.EnergyUse));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("energyUse", termsCriteria.Field);
            Assert.Equal(expectedValues.Length, termsCriteria.Values.Count);
            foreach (var term in expectedValues)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void Int32EqualsOperatorGeneratesTermCriteria()
        {
            const Int32 expectedConstant = 808;
            var where = Robots.Where(e => e.Id == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("id", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public static void Int32EqualsOperatorSupportsDeepQueries()
        {
            const Int32 expectedConstant = 2;
            var where = Robots.Where(e => e.Stats.Limbs.HandCount == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("stats.limbs.handCount", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public static void Int32NotEqualsOperatorSupportsDeepQueries()
        {
            const Int32 expectedConstant = 2;
            var where = Robots.Where(e => e.Stats.Limbs.HandCount != expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var notCriteria = Assert.IsType<NotCriteria>(criteria);
            var termCriteria = Assert.IsType<TermCriteria>(notCriteria.Criteria);
            Assert.Equal("stats.limbs.handCount", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void Int32StaticEqualsMethodGeneratesTermCriteria()
        {
            const Int32 expectedConstant = 98004;
            var where = Robots.Where(e => int.Equals(e.Id, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("id", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void Int32ArrayContainsGeneratesTermsCriteria()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 45678 };
            var where = Robots.Where(e => expectedIds.Contains(e.Id));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("id", termsCriteria.Field);
            Assert.Equal(expectedIds.Length, termsCriteria.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void DateTimeEqualsOperatorGeneratesTermCriteria()
        {
            var expectedConstant = DateTime.Now;
            var where = Robots.Where(e => e.Started == expectedConstant);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("started", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void DateTimeStaticEqualsMethodGeneratesTermCriteria()
        {
            var expectedConstant = DateTime.Now;
            var where = Robots.Where(e => DateTime.Equals(e.Started, expectedConstant));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("started", termCriteria.Field);
            Assert.Equal(expectedConstant, termCriteria.Value);
        }

        [Fact]
        public void DateTimeArrayContainsGeneratesTermsCriteria()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 4568 }.Select(d => DateTime.Now.AddDays(d)).ToArray();
            var where = Robots.Where(e => expectedIds.Contains(e.Started));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("terms", termsCriteria.Name);
            Assert.Equal("started", termsCriteria.Field);
            Assert.Equal(expectedIds.Length, termsCriteria.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termsCriteria.Values);
        }

        [Fact]
        public void StringEqualsNullGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => e.Name == null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<MissingCriteria>(criteria);
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringEqualsNullNegatedGeneratesExistsCriteria()
        {
            // ReSharper disable once NegativeEqualityExpression
            var where = Robots.Where(e => !(e.Name == null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<ExistsCriteria>(criteria);
            Assert.Equal("exists", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringStaticEqualsNullMethodGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => string.Equals(e.Name, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<MissingCriteria>(criteria);
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringNotEqualsNullGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => e.Name != null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var existsCriteria = Assert.IsType<ExistsCriteria>(criteria);
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("name", existsCriteria.Field);
        }

        [Fact]
        public void StringNotEqualsNullNegatedGeneratesMissingCriteria()
        {
            // ReSharper disable once NegativeEqualityExpression
            var where = Robots.Where(e => !(e.Name != null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<MissingCriteria>(criteria);
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("name", missingCriteria.Field);
        }

        [Fact]
        public void StringStaticNotEqualsNullMethodGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => !string.Equals(e.Name, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var existsCriteria = Assert.IsType<ExistsCriteria>(criteria);
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("name", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntEqualsNullGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => e.Zone == null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<MissingCriteria>(criteria);
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("zone", missingCriteria.Field);
        }

        [Fact]
        public void NullableIntStaticEqualsNullMethodGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => Nullable<int>.Equals(e.Zone, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<MissingCriteria>(criteria);
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("zone", missingCriteria.Field);
        }

        [Fact]
        public void NullableIntNotEqualsNullGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => e.Zone != null);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var existsCriteria = Assert.IsType<ExistsCriteria>(criteria);
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("zone", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntStaticNotEqualsNullMethodGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => !Nullable<int>.Equals(e.Zone, null));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var existsCriteria = Assert.IsType<ExistsCriteria>(criteria);
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("zone", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntNotValuePropertyGeneratesExistsCriteria()
        {
            var where = Robots.Where(e => e.Zone.HasValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var existsCriteria = Assert.IsType<ExistsCriteria>(criteria);
            Assert.Equal("exists", existsCriteria.Name);
            Assert.Equal("zone", existsCriteria.Field);
        }

        [Fact]
        public void NullableIntNotHasValuePropertyGeneratesMissingCriteria()
        {
            var where = Robots.Where(e => !e.Zone.HasValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var missingCriteria = Assert.IsType<MissingCriteria>(criteria);
            Assert.Equal("missing", missingCriteria.Name);
            Assert.Equal("zone", missingCriteria.Field);
        }

        [Fact]
        public void OrOperatorGeneratesOrCriteria()
        {
            var where = Robots.Where(e => e.Name == "IG-88" || e.Cost > 1);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var orCriteria = Assert.IsType<OrCriteria>(criteria);
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
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var orCriteria = Assert.IsType<OrCriteria>(criteria);
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
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var orCriteria = Assert.IsType<AndCriteria>(criteria);
            Assert.Equal("and", orCriteria.Name);
            Assert.Single(orCriteria.Criteria, f => f.Name == "term");
            Assert.Single(orCriteria.Criteria, f => f.Name == "range");
            Assert.Equal(2, orCriteria.Criteria.Count);
        }

        [Fact]
        public void AndOperatorChainsAndsTogetherIntoSingleAndCriteria()
        {
            var where = Robots.Where(r => r.Name == "IG-88" && r.Cost > 1 && r.Zone.HasValue);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var andCriteria = Assert.IsType<AndCriteria>(criteria);
            Assert.Equal("and", andCriteria.Name);
            Assert.Single(andCriteria.Criteria, f => f.Name == "term");
            Assert.Single(andCriteria.Criteria, f => f.Name == "range");
            Assert.Single(andCriteria.Criteria, f => f.Name == "exists");
            Assert.Equal(3, andCriteria.Criteria.Count);
        }

        [Fact]
        public static void ContainsAny_PropertyFirst_CreatesTermsQuery()
        {
            var matchNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(r => ElasticMethods.ContainsAny(r.Aliases, matchNames));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("aliases", termsCriteria.Field);
            Assert.Contains("Robbie", termsCriteria.Values);
            Assert.Contains("Kryten", termsCriteria.Values);
            Assert.Contains("IG-88", termsCriteria.Values);
            Assert.Contains("Marvin", termsCriteria.Values);
            Assert.Equal(TermsExecutionMode.@bool, termsCriteria.ExecutionMode);
        }

        [Fact]
        public static void ContainsAny_ListFirst_CreatesTermsQuery()
        {
            var matchNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(r => ElasticMethods.ContainsAny(matchNames, r.Aliases));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("aliases", termsCriteria.Field);
            Assert.Contains("Robbie", termsCriteria.Values);
            Assert.Contains("Kryten", termsCriteria.Values);
            Assert.Contains("IG-88", termsCriteria.Values);
            Assert.Contains("Marvin", termsCriteria.Values);
            Assert.Equal(TermsExecutionMode.@bool, termsCriteria.ExecutionMode);
        }

        [Fact]
        public static void ContainsAll_PropertyFirst_CreatesTermsQuery()
        {
            var matchNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(r => ElasticMethods.ContainsAll(r.Aliases, matchNames));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("aliases", termsCriteria.Field);
            Assert.Contains("Robbie", termsCriteria.Values);
            Assert.Contains("Kryten", termsCriteria.Values);
            Assert.Contains("IG-88", termsCriteria.Values);
            Assert.Contains("Marvin", termsCriteria.Values);
            Assert.Equal(TermsExecutionMode.and, termsCriteria.ExecutionMode);
        }

        [Fact]
        public static void ContainsAll_ListFirst_CreatesTermsQuery()
        {
            var matchNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(r => ElasticMethods.ContainsAll(matchNames, r.Aliases));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termsCriteria = Assert.IsType<TermsCriteria>(criteria);
            Assert.Equal("aliases", termsCriteria.Field);
            Assert.Contains("Robbie", termsCriteria.Values);
            Assert.Contains("Kryten", termsCriteria.Values);
            Assert.Contains("IG-88", termsCriteria.Values);
            Assert.Contains("Marvin", termsCriteria.Values);
            Assert.Equal(TermsExecutionMode.and, termsCriteria.ExecutionMode);
        }

        [Fact]
        public void RegexElasticMethodCreatesRegexWhereCriteria()
        {
            var where = Robots.Where(r => ElasticMethods.Regexp(r.Name, "r.*bot"));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var regexpCriteria = Assert.IsType<RegexpCriteria>(criteria);
            Assert.Equal("regexp", regexpCriteria.Name);
            Assert.Equal("name", regexpCriteria.Field);
            Assert.Equal("r.*bot", regexpCriteria.Regexp);
        }

        [Fact]
        public void PrefixElasticMethodCreatesPrefixWhereCriteria()
        {
            var where = Robots.Where(r => ElasticMethods.Prefix(r.Name, "robot"));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var regexpCriteria = Assert.IsType<PrefixCriteria>(criteria);
            Assert.Equal("prefix", regexpCriteria.Name);
            Assert.Equal("name", regexpCriteria.Field);
            Assert.Equal("robot", regexpCriteria.Prefix);
        }

        [Fact]
        public void PrefixArrayElasticMethodCreatesPrefixWhereCriteria()
        {
            var where = Robots.Where(r => ElasticMethods.Prefix(r.Aliases, "robot"));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var regexpCriteria = Assert.IsType<PrefixCriteria>(criteria);
            Assert.Equal("prefix", regexpCriteria.Name);
            Assert.Equal("aliases", regexpCriteria.Field);
            Assert.Equal("robot", regexpCriteria.Prefix);
        }

        [Fact]
        public void ConvertIsOtherwiseIgnored()
        {
            var where = Robots.Where(r => ((double)r.Cost).Equals(1));
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var termCriteria = Assert.IsType<TermCriteria>(criteria);
            Assert.Equal("term", termCriteria.Name);
            Assert.Equal("cost", termCriteria.Field);
            Assert.Equal(1d, termCriteria.Value);
        }

        [Fact]
        public void ConstantTrueAtRootIsOptimizedOut()
        {
            var where = Robots.Where(r => true);
            var request = ElasticQueryTranslator.Translate(CouchMapping, where.Expression).SearchRequest;

            var existsCriteria = Assert.IsType<ExistsCriteria>(request.Query);
            Assert.Equal("doc.id", existsCriteria.Field);
        }

        [Fact]
        public void ConstantFalseIsTranslatedToConstantFalse()
        {
            var where = Robots.Where(r => false);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.NotNull(request.Query);
            Assert.Same(ConstantCriteria.False, request.Query);
        }

        [Fact]
        public void ConstantTrueWithinAndIsOptimizedOut()
        {
            var where = Robots.Where(r => true && r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.NotNull(request.Query);
            Assert.IsType<RangeCriteria>(request.Query);
        }

        [Fact]
        public void ConstantFalseWithinAndOptimizesOutAllOthers()
        {
            var where = Robots.Where(r => r.Cost > 1 && false && r.Zone == 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.Same(ConstantCriteria.False, request.Query);
        }

        [Fact]
        public void ConstantTrueWithinOrOptimizesOutBackToMapping()
        {
            var where = Robots.Where(r => r.Cost < 10 || true || r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(CouchMapping, where.Expression).SearchRequest;

            Assert.IsType<ExistsCriteria>(request.Query);
        }

        [Fact]
        public void ConstantFalseWithinOrIsOptimizedOut()
        {
            var where = Robots.Where(r => r.Cost < 10 || false || r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.NotNull(request.Query);
            var orCriteria = Assert.IsType<OrCriteria>(request.Query);
            Assert.Equal(2, orCriteria.Criteria.Count);
        }

        [Fact]
        public void EvaluatedConstantTrueWithinOrOptimizesOutBackToMapping()
        {
            Robot dead = null;
            var where = Robots.Where(r => r.Cost < 10 || dead == null || r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(CouchMapping, where.Expression).SearchRequest;

            Assert.IsType<ExistsCriteria>(request.Query);
        }

        [Fact]
        public void EvaluatedConstantFalseWithinOrOptimizesOutAllOthers()
        {
            Robot dead = null;
            var where = Robots.Where(r => r.Cost < 10 || dead != null || r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.NotNull(request.Query);
            var orCriteria = Assert.IsType<OrCriteria>(request.Query);
            Assert.Equal(2, orCriteria.Criteria.Count);
        }

        [Fact]
        public void ConstantLeftSideOfLessThanGeneratesRangeGreaterThan()
        {
            const decimal expectedConstant = 12.34m;
            var where = Robots.Where(e => expectedConstant < e.Cost);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void ConstantLeftSideOfLessThanOrEqualGeneratesRangeGreaterThanOrEqual()
        {
            const decimal expectedConstant = 12.34m;
            var where = Robots.Where(e => expectedConstant <= e.Cost);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void ConstantLeftSideOfGreaterThanGeneratesRangeLessThan()
        {
            const decimal expectedConstant = 12.34m;
            var where = Robots.Where(e => expectedConstant > e.Cost);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void ConstantLeftSideOfGreaterThanOrEqualGeneratesRangeLessThanOrEqual()
        {
            const decimal expectedConstant = 12.34m;
            var where = Robots.Where(e => expectedConstant >= e.Cost);
            var criteria = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Query;

            var rangeCriteria = Assert.IsType<RangeCriteria>(criteria);
            Assert.Equal("range", rangeCriteria.Name);
            Assert.Equal("cost", rangeCriteria.Field);
            Assert.Single(rangeCriteria.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeCriteria.Specifications.Count);
        }

        [Fact]
        public void ConstantTrueInsideAnOrInsideAnAndOptimizesOut()
        {
            var query = Robots.Where(r => r.Id == 11 && (true || r.EnergyUse > 2));

            var searchRequest = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<TermCriteria>(searchRequest.Query);
        }

        [Fact]
        public void ConstantFalseInsideAnAndInsideAnOrOptimizesOut()
        {
            var query = Robots.Where(r => r.Id == 11 || (false && r.EnergyUse > 2));

            var searchRequest = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<TermCriteria>(searchRequest.Query);
        }

        [Fact]
        public void QueryAndWhereGeneratesQueryAndFilterCriteria()
        {
            var query = Robots.Where(r => ElasticMethods.Regexp(r.Name, "r.*bot")).Where(r => r.Zone.HasValue);

            var searchRequest = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var andCriteria = Assert.IsType<AndCriteria>(searchRequest.Query);
            Assert.IsType<RegexpCriteria>(andCriteria.Criteria[0]);
            Assert.IsType<ExistsCriteria>(andCriteria.Criteria[1]);
        }

        [Fact]
        public void QueryGeneratesQueryCriteria()
        {
            var where = Robots.Where(r => r.Name == "IG-88" && r.Cost > 1);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            var boolCriteria = Assert.IsType<BoolCriteria>(request.Query);
            Assert.Single(boolCriteria.Must, f => f.Name == "term");
            Assert.Single(boolCriteria.Must, f => f.Name == "range");
            Assert.Equal(2, boolCriteria.Must.Count);
        }



        [Fact]
        public void QueryStringWithQueryCombinesToBoolQueryCriteria()
        {
            const string expectedQueryStringValue = "Data";
            var where = Robots.QueryString(expectedQueryStringValue).Where(q => q.Cost > 0);
            var request = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest;

            Assert.NotNull(request.Query);
            var boolCriteria = Assert.IsType<BoolCriteria>(request.Query);
            Assert.Single(boolCriteria.Must, a => a.Name == "query_string");
            Assert.Single(boolCriteria.Must, a => a.Name == "range");
            Assert.Equal(2, boolCriteria.Must.Count);
        }

        [Fact]
        public void AndOrQueryGeneratesBoolQueryWithAndArgs()
        {
            var query = Robots.Where(q => q.Cost > 0 && (q.EnergyUse > 0 || q.Started < DateTime.Now));

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<BoolCriteria>(request.Query);
        }

        [Fact]
        public void BooleanTrueGeneratesMatchAllQuery()
        {
            var query = Robots.Where(q => true);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<MatchAllCriteria>(request.Query);
        }

        [Fact]
        public void EvaluatedTrueGeneratesMatchAllQuery()
        {
            var query = Robots.Where(q => 1 == 1);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            Assert.IsType<MatchAllCriteria>(request.Query);
        }

        [Fact]
        public void BooleanFalseGeneratesNotMatchAllQuery()
        {
            var query = Robots.Where(q => false);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var notCriteria = Assert.IsType<NotCriteria>(request.Query);
            Assert.IsType<MatchAllCriteria>(notCriteria.Criteria);
        }

        [Fact]
        public void EvaluatedFalseGeneratesNotMatchAllQuery()
        {
            var query = Robots.Where(q => 1 > 1);

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var notCriteria = Assert.IsType<NotCriteria>(request.Query);
            Assert.IsType<MatchAllCriteria>(notCriteria.Criteria);
        }

        [Fact]
        public void BooleanConstantsGeneratesBoolMatchAllQueryAndNotMatchAllQuery()
        {
            var query = Robots.Where(r => (r.Id > 1 && true) || (r.Id < 1 && false));

            var request = ElasticQueryTranslator.Translate(Mapping, query.Expression).SearchRequest;

            var boolCriteria = Assert.IsType<BoolCriteria>(request.Query);
            Assert.Empty(boolCriteria.Must);
            Assert.Empty(boolCriteria.MustNot);
            Assert.Equal(2, boolCriteria.Should.Count);

            Assert.Single(boolCriteria.Should, c => c is BoolCriteria && ((BoolCriteria)c).Must.Any(s => s is MatchAllCriteria));
            Assert.Single(boolCriteria.Should, c => c is BoolCriteria && ((BoolCriteria)c).Must.Any(s => s is NotCriteria));
        }
    }
}