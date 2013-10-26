// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Criteria;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Request.Criteria
{
    public class OrCriteriaTests
    {
        private readonly TermCriteria salutationMr = new TermCriteria("salutation", "Mr");
        private readonly TermCriteria salutationMrs = new TermCriteria("salutation", "Mrs");
        private readonly TermCriteria salutationMs = new TermCriteria("salutation", "Miss", "Ms");
        private readonly TermCriteria area408 = new TermCriteria("area", "408");

        [Fact]
        public void NamePropertyIsOr()
        {
            var criteria = new OrCriteria();

            Assert.Equal("or", criteria.Name);
        }

        [Fact]
        public void ConstructorSetsCriteria()
        {
            var criteria = new OrCriteria(salutationMr, area408);

            Assert.Contains(salutationMr, criteria.Criteria);
            Assert.Contains(area408, criteria.Criteria);
            Assert.Equal(2, criteria.Criteria.Count);
        }

        [Fact]
        public void CombineWithEmptyListReturnsEmptyOr()
        {
            var criteria = OrCriteria.Combine(new ICriteria[] { });

            Assert.IsType<OrCriteria>(criteria);
            Assert.Empty(((OrCriteria)criteria).Criteria);
        }

        [Fact]
        public void CombineWithAllTermFieldsSameCombinesIntoSingleTerm()
        {
            var criteria = OrCriteria.Combine(salutationMr, salutationMrs, salutationMs);

            Assert.IsType<TermCriteria>(criteria);
            var termCriteria = (TermCriteria)criteria;
            Assert.Equal(termCriteria.Field, salutationMr.Field);

            var allValues = salutationMr.Values.Concat(salutationMrs.Values).Concat(salutationMs.Values).Distinct().ToArray();
            foreach (var value in allValues)
                Assert.Contains(value, termCriteria.Values);

            Assert.Equal(allValues.Length, termCriteria.Values.Count);
        }

        [Fact]
        public void CombineWithDifferTermCriteriaFieldsDoesNotCombine()
        {
            var criteria = OrCriteria.Combine(salutationMr, salutationMrs, area408);

            Assert.IsType<OrCriteria>(criteria);
            var orCriteria = (OrCriteria)criteria;
            Assert.Contains(salutationMr, orCriteria.Criteria);
            Assert.Contains(salutationMrs, orCriteria.Criteria);
            Assert.Contains(area408, orCriteria.Criteria);
            Assert.Equal(3, orCriteria.Criteria.Count);
        }
    }
}