// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticLINQ.Test.Utility
{
    public class TypeHelperTests
    {
        [Fact]
        public void GetSequenceElementTypeIdentifiesListElementType()
        {
            var type = typeof(List<String>);

            var elementType = TypeHelper.GetSequenceElementType(type);

            Assert.Equal(typeof(string), elementType);
        }

        [Fact]
        public void GetSequenceElementTypeIdentifiesArrayElementType()
        {
            var type = typeof(decimal[]);

            var elementType = TypeHelper.GetSequenceElementType(type);

            Assert.Equal(typeof(decimal), elementType);
        }

        [Fact]
        public void FindIEnumerableWithNullReturnsNull()
        {
            var type = TypeHelper.FindIEnumerable(null);

            Assert.Null(type);
        }

        [Fact]
        public void FindIEnumerableWithArrayTReturnsIEnumerableT()
        {
            var type = TypeHelper.FindIEnumerable(typeof(int[]));

            Assert.Equal(typeof(IEnumerable<int>), type);
        }

        [Fact]
        public void FindIEnumerableWithListTReturnsIEnumerableT()
        {
            var type = TypeHelper.FindIEnumerable(typeof(List<DateTime>));

            Assert.Equal(typeof(IEnumerable<DateTime>), type);
        }

        [Fact]
        public void FindIEnumerableWithSubClassOfListTReturnsIEnumerableT()
        {
            var type = TypeHelper.FindIEnumerable(typeof(SubclassOfList));

            Assert.Equal(typeof(IEnumerable<Decimal>), type);
        }

        private class SubclassOfList : List<Decimal> { }

        [Fact]
        public void IsNullableTypeIdentifiesNullableValueTypeAsNullableT()
        {
            var isNullable = TypeHelper.IsNullableType(typeof(int?));

            Assert.True(isNullable);
        }

        [Fact]
        public void IsNullableTypeIdentifiesValueTypeAsNotNullableT()
        {
            var isNullable = TypeHelper.IsNullableType(typeof(int));

            Assert.False(isNullable);
        }

        [Fact]
        public void IsNullableTypeIdentifiesReferenceTypeAsNotNullableT()
        {
            var isNullable = TypeHelper.IsNullableType(typeof(string));

            Assert.False(isNullable);
        }
    }
}