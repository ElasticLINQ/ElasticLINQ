// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Newtonsoft.Json.Bson;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationWhereTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void ObjectEqualsMethodFromConstantGeneratesTermFilter()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void ObjectEqualsMethodFromMemberGeneratesTermFilter()
        {
            const string expectedConstant = "Robbie";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void ObjectStaticEqualsMethodGeneratesTermFilter()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => Object.Equals(e.Name, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringEqualsOperatorGeneratesTermFilter()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => e.Name == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringEqualsMethodFromMemberGeneratesTermFilter()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => e.Name.Equals(expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringEqualsMethodFromConstantGeneratesTermFilter()
        {
            const string expectedConstant = "Marvin";
            var where = Robots.Where(e => expectedConstant.Equals(e.Name));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringEqualsOperatorReversedArgumentsGeneratesTermFilter()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => expectedConstant == e.Name);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringStaticEqualsMethodGeneratesTermFilter()
        {
            const string expectedConstant = "Kryten";
            var where = Robots.Where(e => String.Equals(e.Name, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringStaticEqualsMethodReversedArgumentsGeneratesTermFilter()
        {
            const string expectedConstant = "IG-88";
            var where = Robots.Where(e => String.Equals(expectedConstant, e.Name));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void StringArrayContainsGeneratesTermsFilter()
        {
            var expectedNames = new[] { "Robbie", "Kryten", "IG-88", "Marvin" };
            var where = Robots.Where(e => expectedNames.Contains(e.Name));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("name", termFilter.Field);
            Assert.Equal(expectedNames.Length, termFilter.Values.Count);
            foreach (var term in expectedNames)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void DecimalEqualsOperatorGeneratesTermFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("cost", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DecimalStaticEqualsMethodGeneratesTermFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => decimal.Equals(e.Cost, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("cost", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DecimalArrayContainsGeneratesTermsFilter()
        {
            var expectedHourlyWages = new[] { 1.1m, 3.2m, 4.9m, 7.6m, 8.9m, 12345.678m };
            var where = Robots.Where(e => expectedHourlyWages.Contains(e.Cost));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("cost", termFilter.Field);
            Assert.Equal(expectedHourlyWages.Length, termFilter.Values.Count);
            foreach (var term in expectedHourlyWages)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void DecimalLessThanOperatorGeneratesLessThanRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost < expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("cost", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalLessThanOrEqualsOperatorGeneratesLessThanOrEqualRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost <= expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("cost", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOperatorGeneratesGreaterThanRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost > expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("cost", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOrEqualsOperatorGeneratesGreaterThanOrEqualRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Robots.Where(e => e.Cost >= expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("cost", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanAndLessThanGeneratesGreaterThanOrLessThanRangeFilter()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Robots.Where(e => e.Cost > expectedLower && e.Cost < expectedUpper);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("cost", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedLower));
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedUpper));
            Assert.Equal(2, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOrEqualAndLessThanOrEqualGeneratesGreaterThanOrEqualLessThanOrEqualRangeFilter()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Robots.Where(e => e.Cost >= expectedLower && e.Cost <= expectedUpper);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("cost", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedLower));
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedUpper));
            Assert.Equal(2, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DoubleEqualsOperatorGeneratesTermFilter()
        {
            const double expectedConstant = 710.956;
            var where = Robots.Where(e => e.EnergyUse == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("energyUse", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DoubleStaticEqualsMethodGeneratesTermFilter()
        {
            const double expectedConstant = 710.956;
            var where = Robots.Where(e => double.Equals(e.EnergyUse, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("energyUse", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DoubleArrayContainsGeneratesTermsFilter()
        {
            var expectedValues = new[] { 1.1, 3.2, 4.9, 7.6, 8.9, 12345.678 };
            var where = Robots.Where(e => expectedValues.Contains(e.EnergyUse));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("energyUse", termFilter.Field);
            Assert.Equal(expectedValues.Length, termFilter.Values.Count);
            foreach (var term in expectedValues)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void Int32EqualsOperatorGeneratesTermFilter()
        {
            const Int32 expectedConstant = 808;
            var where = Robots.Where(e => e.Id == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("id", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void Int32StaticEqualsMethodGeneratesTermFilter()
        {
            const Int32 expectedConstant = 98004;
            var where = Robots.Where(e => int.Equals(e.Id, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("id", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void Int32ArrayContainsGeneratesTermsFilter()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 45678 };
            var where = Robots.Where(e => expectedIds.Contains(e.Id));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("id", termFilter.Field);
            Assert.Equal(expectedIds.Length, termFilter.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void DateTimeEqualsOperatorGeneratesTermFilter()
        {
            var expectedConstant = DateTime.Now;
            var where = Robots.Where(e => e.Started == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("started", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DateTimeStaticEqualsMethodGeneratesTermFilter()
        {
            var expectedConstant = DateTime.Now;
            var where = Robots.Where(e => DateTime.Equals(e.Started, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("started", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DateTimeArrayContainsGeneratesTermsFilter()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 4568 }.Select(d => DateTime.Now.AddDays(d)).ToArray();
            var where = Robots.Where(e => expectedIds.Contains(e.Started));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("started", termFilter.Field);
            Assert.Equal(expectedIds.Length, termFilter.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void StringEqualsNullGeneratesMissingFilter()
        {
            var where = Robots.Where(e => e.Name == null);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingFilter>(filter);
            var missingFilter = (MissingFilter)filter;
            Assert.Equal("missing", missingFilter.Name);
            Assert.Equal("name", missingFilter.Field);
        }

        [Fact]
        public void StringStaticEqualsNullMethodGeneratesMissingFilter()
        {
            var where = Robots.Where(e => String.Equals(e.Name, null));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingFilter>(filter);
            var missingFilter = (MissingFilter)filter;
            Assert.Equal("missing", missingFilter.Name);
            Assert.Equal("name", missingFilter.Field);
        }

        [Fact]
        public void StringNotEqualsNullGeneratesExistsFilter()
        {
            var where = Robots.Where(e => e.Name != null);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsFilter>(filter);
            var existsFilter = (ExistsFilter)filter;
            Assert.Equal("exists", existsFilter.Name);
            Assert.Equal("name", existsFilter.Field);
        }

        [Fact]
        public void StringStaticNotEqualsNullMethodGeneratesMissingFilter()
        {
            var where = Robots.Where(e => !String.Equals(e.Name, null));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsFilter>(filter);
            var existsFilter = (ExistsFilter)filter;
            Assert.Equal("exists", existsFilter.Name);
            Assert.Equal("name", existsFilter.Field);
        }

        [Fact]
        public void NullableIntEqualsNullGeneratesMissingFilter()
        {
            var where = Robots.Where(e => e.Zone == null);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingFilter>(filter);
            var missingFilter = (MissingFilter)filter;
            Assert.Equal("missing", missingFilter.Name);
            Assert.Equal("zone", missingFilter.Field);
        }

        [Fact]
        public void NullableIntStaticEqualsNullMethodGeneratesMissingFilter()
        {
            var where = Robots.Where(e => Nullable<int>.Equals(e.Zone, null));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingFilter>(filter);
            var missingFilter = (MissingFilter)filter;
            Assert.Equal("missing", missingFilter.Name);
            Assert.Equal("zone", missingFilter.Field);
        }

        [Fact]
        public void NullableIntNotEqualsNullGeneratesExistsFilter()
        {
            var where = Robots.Where(e => e.Zone != null);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsFilter>(filter);
            var existsFilter = (ExistsFilter)filter;
            Assert.Equal("exists", existsFilter.Name);
            Assert.Equal("zone", existsFilter.Field);
        }

        [Fact]
        public void NullableIntStaticNotEqualsNullMethodGeneratesExistsFilter()
        {
            var where = Robots.Where(e => !Nullable<int>.Equals(e.Zone, null));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsFilter>(filter);
            var existsFilter = (ExistsFilter)filter;
            Assert.Equal("exists", existsFilter.Name);
            Assert.Equal("zone", existsFilter.Field);
        }

        [Fact]
        public void NullableIntNotValuePropertyGeneratesExistsFilter()
        {
            var where = Robots.Where(e => e.Zone.HasValue);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<ExistsFilter>(filter);
            var existsFilter = (ExistsFilter)filter;
            Assert.Equal("exists", existsFilter.Name);
            Assert.Equal("zone", existsFilter.Field);
        }

        [Fact]
        public void NullableIntNotHasValuePropertyGeneratesMissingFilter()
        {
            var where = Robots.Where(e => !e.Zone.HasValue);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<MissingFilter>(filter);
            var missingFilter = (MissingFilter)filter;
            Assert.Equal("missing", missingFilter.Name);
            Assert.Equal("zone", missingFilter.Field);
        }
    }
}