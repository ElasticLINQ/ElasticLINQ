// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class MappingHelpersTests
    {
        [Fact]
        public void ToCamelCaseWithAllCapsLowersFirstCapitalOnly()
        {
            var actual = "ALLCAPS".ToCamelCase();

            Assert.Equal("aLLCAPS", actual);
        }

        [Fact]
        public void ToCamelCaseWithAllLowerCaseReturnsAllLowerCase()
        {
            var actual = "lowercase".ToCamelCase();

            Assert.Equal("lowercase", actual);
        }

        [Fact]
        public void ToCamelCaseWithMixedCaseOnlyChangesFirstLetter()
        {
            var actual = "MixedCaseExample".ToCamelCase();

            Assert.Equal("mixedCaseExample", actual);
        }

        [Fact]
        public void ToCamelCaseWithCamelCaseReturnsCamelCase()
        {
            var actual = "alreadyCamelCase".ToCamelCase();

            Assert.Equal("alreadyCamelCase", actual);
        }

        [Fact]
        public void ToCamelCaseWithSingleCharStringReturnsSingleCharLowered()
        {
            var actual = "S".ToCamelCase();

            Assert.Equal("s", actual);
        }

        [Fact]
        public void ToCamelCaseWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToCamelCase();

            Assert.Equal("", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ToCamelCaseWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToCamelCase());
        }

        [Fact]
        public void ToPluralWithStringNotEndingInSAddsS()
        {
            var actual = "test".ToPlural();

            Assert.Equal("tests", actual);
        }

        [Fact]
        public void ToPluralWithStringEndingInSDoesNotAddS()
        {
            var actual = "tests".ToPlural();

            Assert.Equal("tests", actual);
        }

        [Fact]
        public void ToPluralWithEmptyStringReturnsEmptyString()
        {
            var actual = "".ToPlural();

            Assert.Equal("", actual);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ToPluralWithNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).ToPlural());
        }
    }
}