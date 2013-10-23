// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using System;
using System.Linq;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors
{
    public class ElasticQueryTranslationTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));
        private static readonly IElasticMapping mapping = new TrivialElasticMapping();
        private static readonly ElasticQueryProvider sharedProvider = new ElasticQueryProvider(connection, mapping);

        private static IQueryable<Employee> Employees
        {
            get { return new ElasticQuery<Employee>(sharedProvider); }
        }

        private class Employee
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime Hired { get; set; }
            public decimal HourlyWage { get; set; }
        }

        [Fact]
        public void TypeIsSetFromType()
        {
            var actual = mapping.GetTypeName(typeof(Employee));

            var query = Employees;
            var translation = ElasticQueryTranslator.Translate(mapping, query.Expression);

            Assert.Equal(actual, translation.SearchRequest.Type);
        }

        [Fact]
        public void SkipTranslatesToFrom()
        {
            const int actual = 325;

            var skipped = Employees.Skip(actual);
            var translation = ElasticQueryTranslator.Translate(mapping, skipped.Expression);

            Assert.Equal(actual, translation.SearchRequest.From);
        }

        [Fact]
        public void TakeTranslatesToSize()
        {
            const int actual = 73;

            var taken = Employees.Take(actual);
            var translation = ElasticQueryTranslator.Translate(mapping, taken.Expression);

            Assert.Equal(actual, translation.SearchRequest.Size);
        }

        [Fact]
        public void SelectAnonymousProjectionTranslatesToFields()
        {
            var selected = Employees.Select(e => new { e.Id, e.HourlyWage });
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var fields = translation.SearchRequest.Fields;
            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("hourlyWage", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectAnonymousProjectionWithSomeIdentifiersTranslatesToFields()
        {
            var selected = Employees.Select(e => new { First = e.Id, Second = e.Hired, e.HourlyWage });
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var fields = translation.SearchRequest.Fields;
            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("hired", fields);
            Assert.Contains("hourlyWage", fields);
            Assert.Equal(3, fields.Count);
        }

        [Fact]
        public void SelectTupleProjectionWithIdentifiersTranslatesToFields()
        {
            var selected = Employees.Select(e => Tuple.Create(e.Id, e.Name));
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var fields = translation.SearchRequest.Fields;
            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Contains("name", fields);
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void SelectSingleFieldTranslatesToField()
        {
            var selected = Employees.Select(e => e.Id);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var fields = translation.SearchRequest.Fields;
            Assert.NotNull(fields);
            Assert.Contains("id", fields);
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void OrderByTranslatesToSortAscending()
        {
            var selected = Employees.OrderBy(e => e.Id);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.Single(sortOptions, o => o.Ascending && o.Name == "id");
            Assert.Equal(1, sortOptions.Count);
        }

        [Fact]
        public void OrderByScoreFieldTranslatesToScoreSortAscending()
        {
            var selected = Employees.OrderBy(e => ElasticFields.Score);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.Single(sortOptions, o => o.Ascending && o.Name == "_score");
            Assert.Equal(1, sortOptions.Count);
        }

        [Fact]
        public void OrderByDescendingTranslatesToSortDescending()
        {
            var selected = Employees.OrderByDescending(e => e.Name);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.Single(sortOptions, o => !o.Ascending && o.Name == "name");
            Assert.Equal(1, sortOptions.Count);
        }

        [Fact]
        public void OrderByDescendingScoreFieldTranslatesToScoreSortDescending()
        {
            var selected = Employees.OrderByDescending(e => ElasticFields.Score);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.Single(sortOptions, o => !o.Ascending && o.Name == "_score");
            Assert.Equal(1, sortOptions.Count);
        }

        [Fact]
        public void OrderByScoreTranslatesToScoreSortAscending()
        {
            var selected = Employees.OrderByScore();
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.Single(sortOptions, o => o.Ascending && o.Name == "_score");
            Assert.Equal(1, sortOptions.Count);
        }

        [Fact]
        public void OrderByScoreDescendingTranslatesToScoreSortDescending()
        {
            var selected = Employees.OrderByScoreDescending();
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.Single(sortOptions, o => !o.Ascending && o.Name == "_score");
            Assert.Equal(1, sortOptions.Count);
        }

        [Fact]
        public void ThenByTranslatesToSecondSortAscending()
        {
            var selected = Employees.OrderBy(e => e.Id).ThenBy(e => e.Name);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.NotNull(sortOptions);
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(sortOptions[1].Ascending && sortOptions[1].Name == "name");
            Assert.Equal(2, sortOptions.Count);
        }

        [Fact]
        public void ThenByScoreFieldTranslatesToSecondScoreSortAscending()
        {
            var selected = Employees.OrderBy(e => e.Id).ThenBy(e => ElasticFields.Score);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.NotNull(sortOptions);
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(sortOptions[1].Ascending && sortOptions[1].Name == "_score");
            Assert.Equal(2, sortOptions.Count);
        }

        [Fact]
        public void ThenByDescendingTranslatesToSecondSortDescending()
        {
            var selected = Employees.OrderBy(e => e.Id).ThenByDescending(e => e.Name);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.NotNull(sortOptions);
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(!sortOptions[1].Ascending && sortOptions[1].Name == "name");
            Assert.Equal(2, sortOptions.Count);
        }

        [Fact]
        public void ThenByDescendingScoreFieldTranslatesToSecondScoreSortDescending()
        {
            var selected = Employees.OrderBy(e => e.Id).ThenByDescending(e => ElasticFields.Score);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.NotNull(sortOptions);
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(!sortOptions[1].Ascending && sortOptions[1].Name == "_score");
            Assert.Equal(2, sortOptions.Count);
        }

        [Fact]
        public void ThenByScoreTranslatesToSortAscending()
        {
            var selected = Employees.OrderBy(o => o.Id).ThenByScore();
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(sortOptions[1].Ascending && sortOptions[1].Name == "_score");
            Assert.Equal(2, sortOptions.Count);
        }

        [Fact]
        public void ThenByScoreDescendingTranslatesToSortDescending()
        {
            var selected = Employees.OrderBy(o => o.Id).ThenByScoreDescending();
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(!sortOptions[1].Ascending && sortOptions[1].Name == "_score");
            Assert.Equal(2, sortOptions.Count);
        }

        [Fact]
        public void OrderByWithThenByTwiceTranslatesToThreeSorts()
        {
            var selected = Employees.OrderBy(e => e.Id).ThenByScore().ThenBy(e => e.HourlyWage);
            var translation = ElasticQueryTranslator.Translate(mapping, selected.Expression);

            var sortOptions = translation.SearchRequest.SortOptions;
            Assert.NotNull(sortOptions);
            Assert.True(sortOptions[0].Ascending && sortOptions[0].Name == "id");
            Assert.True(sortOptions[1].Ascending && sortOptions[1].Name == "_score");
            Assert.True(sortOptions[2].Ascending && sortOptions[2].Name == "hourlyWage");
            Assert.Equal(3, sortOptions.Count);
        }
    }
}