// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Filters;
using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationWhereTests : ElasticQueryTranslationTestsBase
    {
        [Fact]
        public void StringEqualsOperatorGeneratesTermFilter()
        {
            const string expectedConstant = "Bob";
            var where = Employees.Where(e => e.Name == expectedConstant);
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
            const string expectedConstant = "Rob";
            var where = Employees.Where(e => expectedConstant == e.Name);
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
            const string expectedConstant = "Robert";
            var where = Employees.Where(e => String.Equals(e.Name, expectedConstant));
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
            const string expectedConstant = "Charles";
            var where = Employees.Where(e => String.Equals(expectedConstant, e.Name));
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
            var expectedNames = new[] { "Scott", "Brad", "Phil", "David" };
            var where = Employees.Where(e => expectedNames.Contains(e.Name));
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
            var where = Employees.Where(e => e.HourlyWage == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("hourlyWage", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DecimalStaticEqualsMethodGeneratesTermFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Employees.Where(e => decimal.Equals(e.HourlyWage, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("hourlyWage", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DecimalArrayContainsGeneratesTermsFilter()
        {
            var expectedHourlyWages = new[] { 1.1m, 3.2m, 4.9m, 7.6m, 8.9m, 12345.678m };
            var where = Employees.Where(e => expectedHourlyWages.Contains(e.HourlyWage));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("hourlyWage", termFilter.Field);
            Assert.Equal(expectedHourlyWages.Length, termFilter.Values.Count);
            foreach (var term in expectedHourlyWages)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void DecimalLessThanOperatorGeneratesLessThanRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Employees.Where(e => e.HourlyWage < expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("hourlyWage", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalLessThanOrEqualsOperatorGeneratesLessThanOrEqualRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Employees.Where(e => e.HourlyWage <= expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("hourlyWage", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOperatorGeneratesGreaterThanRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Employees.Where(e => e.HourlyWage > expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("hourlyWage", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOrEqualsOperatorGeneratesGreaterThanOrEqualRangeFilter()
        {
            const decimal expectedConstant = 710.956m;
            var where = Employees.Where(e => e.HourlyWage >= expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("hourlyWage", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedConstant));
            Assert.Equal(1, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanAndLessThanGeneratesGreaterThanOrLessThanRangeFilter()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Employees.Where(e => e.HourlyWage > expectedLower && e.HourlyWage < expectedUpper);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("hourlyWage", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThan && Equals(s.Value, expectedLower));
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThan && Equals(s.Value, expectedUpper));
            Assert.Equal(2, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DecimalGreaterThanOrEqualAndLessThanOrEqualGeneratesGreaterThanOrEqualLessThanOrEqualRangeFilter()
        {
            const decimal expectedLower = 710.956m;
            const decimal expectedUpper = 3428.9m;
            var where = Employees.Where(e => e.HourlyWage >= expectedLower && e.HourlyWage <= expectedUpper);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<RangeFilter>(filter);
            var rangeFilter = (RangeFilter)filter;
            Assert.Equal("range", rangeFilter.Name);
            Assert.Equal("hourlyWage", rangeFilter.Field);
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.GreaterThanOrEqual && Equals(s.Value, expectedLower));
            Assert.Single(rangeFilter.Specifications, s => s.Comparison == RangeComparison.LessThanOrEqual && Equals(s.Value, expectedUpper));
            Assert.Equal(2, rangeFilter.Specifications.Count);
        }

        [Fact]
        public void DoubleEqualsOperatorGeneratesTermFilter()
        {
            const double expectedConstant = 710.956;
            var where = Employees.Where(e => e.Efficiency == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("efficiency", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DoubleStaticEqualsMethodGeneratesTermFilter()
        {
            const double expectedConstant = 710.956;
            var where = Employees.Where(e => double.Equals(e.Efficiency, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("efficiency", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DoubleArrayContainsGeneratesTermsFilter()
        {
            var expectedEfficiency = new[] { 1.1, 3.2, 4.9, 7.6, 8.9, 12345.678 };
            var where = Employees.Where(e => expectedEfficiency.Contains(e.Efficiency));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("efficiency", termFilter.Field);
            Assert.Equal(expectedEfficiency.Length, termFilter.Values.Count);
            foreach (var term in expectedEfficiency)
                Assert.Contains(term, termFilter.Values);
        }

        [Fact]
        public void Int32EqualsOperatorGeneratesTermFilter()
        {
            const Int32 expectedConstant = 808;
            var where = Employees.Where(e => e.Id == expectedConstant);
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
            var where = Employees.Where(e => int.Equals(e.Id, expectedConstant));
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
            var expectedIds = new [] { 1, 3, 2, 9, 7, 6, 123, 45678 };
            var where = Employees.Where(e => expectedIds.Contains(e.Id));
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
            var where = Employees.Where(e => e.Hired == expectedConstant);
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("hired", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DateTimeStaticEqualsMethodGeneratesTermFilter()
        {
            var expectedConstant = DateTime.Now;
            var where = Employees.Where(e => DateTime.Equals(e.Hired, expectedConstant));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("term", termFilter.Name);
            Assert.Equal("hired", termFilter.Field);
            Assert.Equal(1, termFilter.Values.Count);
            Assert.Equal(expectedConstant, termFilter.Values[0]);
        }

        [Fact]
        public void DateTimeArrayContainsGeneratesTermsFilter()
        {
            var expectedIds = new[] { 1, 3, 2, 9, 7, 6, 123, 4568 }.Select(d => DateTime.Now.AddDays(d)).ToArray();
            var where = Employees.Where(e => expectedIds.Contains(e.Hired));
            var filter = ElasticQueryTranslator.Translate(Mapping, where.Expression).SearchRequest.Filter;

            Assert.IsType<TermFilter>(filter);
            var termFilter = (TermFilter)filter;
            Assert.Equal("terms", termFilter.Name);
            Assert.Equal("hired", termFilter.Field);
            Assert.Equal(expectedIds.Length, termFilter.Values.Count);
            foreach (var term in expectedIds)
                Assert.Contains(term, termFilter.Values);
        }
    }
}