// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Utility
{
    public class TypeHelperTests
    {
        private class DirectObjectSubclass : object { }
        private class IndirectObjectSubclass : DirectObjectSubclass { }
        private class SubclassOfList : List<Decimal> { }

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
        public void GetSequenceElementTypeReturnsOriginalTypeIfNoSequenceType()
        {
            var type = typeof(IndirectObjectSubclass);

            var elementType = TypeHelper.GetSequenceElementType(type);

            Assert.Equal(type, elementType);
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
        public void FindIEnumerableWithDictionaryTKeyTValueReturnsIEnumerableKeyValuePair()
        {
            var type = TypeHelper.FindIEnumerable(typeof(Dictionary<string, DateTime>));

            Assert.Equal(typeof(IEnumerable<KeyValuePair<string, DateTime>>), type);
        }

        [Fact]
        public void FindIEnumerableWithSubClassOfListTReturnsIEnumerableT()
        {
            var type = TypeHelper.FindIEnumerable(typeof(SubclassOfList));

            Assert.Equal(typeof(IEnumerable<Decimal>), type);
        }

        [Fact]
        public void FindIEnumerableWithDirectObjectSubclassReturnsNull()
        {
            var type = TypeHelper.FindIEnumerable(typeof(DirectObjectSubclass));

            Assert.Null(type);
        }

        [Fact]
        public void FindIEnumerableWithIndirectObjectSubclassReturnsNull()
        {
            var type = TypeHelper.FindIEnumerable(typeof(IndirectObjectSubclass));

            Assert.Null(type);
        }

        [Fact]
        public void FindIEnumerableWithObjectReturnsNull()
        {
            var type = TypeHelper.FindIEnumerable(typeof(object));

            Assert.Null(type);
        }

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