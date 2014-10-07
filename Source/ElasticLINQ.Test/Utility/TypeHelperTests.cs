// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
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
        
        [ExcludeFromCodeCoverage]
        private class ClassWithMemberInfo
        {
            public DirectObjectSubclass AField = null;
            public IndirectObjectSubclass AProperty { get; set; }
        }

        [ExcludeFromCodeCoverage]
        private class ClassWithMethodInfo : ClassWithMemberInfo
        {
            public int SomeMember() {  return 0; }
        }

        [Fact]
        public static void GetMemberInfoReturnsMemberInfoForMember()
        {
            var memberInfo = TypeHelper.GetMemberInfo((ClassWithMemberInfo c) => c.AField);

            Assert.Equal(typeof(ClassWithMemberInfo).GetField("AField"), memberInfo);
        }

        [Fact]
        public static void GetMemberThrowsNotSupportedExceptionForNonMemberAccessExpressions()
        {
            Expression<Func<ClassWithMethodInfo,int>> invalid = c => c.SomeMember();

            Assert.Throws<NotSupportedException>(() => TypeHelper.GetMemberInfo(invalid));
        }

        [Fact]
        public void GetReturnTypeReturnsFieldTypeForField()
        {
            var memberInfo = TypeHelper.GetMemberInfo((ClassWithMemberInfo c) => c.AField);

            var returnType = TypeHelper.GetReturnType(memberInfo);

            Assert.Equal(typeof(DirectObjectSubclass), returnType);
        }

        [Fact]
        public void GetReturnTypeReturnsPropertyTypeForProperty()
        {
            var memberInfo = TypeHelper.GetMemberInfo((ClassWithMemberInfo c) => c.AProperty);

            var returnType = TypeHelper.GetReturnType(memberInfo);

            Assert.Equal(typeof(IndirectObjectSubclass), returnType);
        }

        [Fact]
        public void GetReturnTypeThrowsNotSupportedExceptionForOtherTypes()
        {
            var methodInfo = typeof (ClassWithMethodInfo).GetMethod("SomeMember");

            var exception = Assert.Throws<NotSupportedException>(() => TypeHelper.GetReturnType(methodInfo));
            Assert.Contains("SomeMember", exception.Message);
            Assert.Contains("ClassWithMethodInfo", exception.Message);
        }

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
        public void IsGenericOfReturnsFalseIfTypeIsNull()
        {
            var isNullable = typeof(int).IsGenericOf(null);

            Assert.False(isNullable);
        }

        [Fact]
        public void IsGenericOfReturnsFalseIfTypeIsNotGeneric()
        {
            var isNullable = typeof(int).IsGenericOf(typeof(Nullable<>));

            Assert.False(isNullable);
        }

        [Fact]
        public void IsGenericOfReturnsTrueIfTypeIsGenericOf()
        {
            var isNullable = typeof(int?).IsGenericOf(typeof(Nullable<>));

            Assert.True(isNullable);
        }

        [Fact]
        public void IsAssignableFromIsTrueForValidAssignment()
        {
            var actual = TypeHelper.IsAssignableFrom(typeof(int?), typeof(int));

            Assert.True(actual);
        }

        [Fact]
        public void IsAssignableFromIsFalseTrueForInvalidAssignment()
        {
            var actual = TypeHelper.IsAssignableFrom(typeof(int), typeof(int?));

            Assert.False(actual);
        }
    }
}