// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Utility
{
    public class ArgumentTests
    {
        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void EnsureNotNullThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Argument.EnsureNotNull("a", null));
        }

        [Fact]
        public void EnsureNotNullDoesNotThrowsWhenValueIsNotNull()
        {
            Argument.EnsureNotNull("a", "b");
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void EnsureNotBlankThrowsArgumentExceptionWhenValueIsWhitespace()
        {
            Assert.Throws<ArgumentException>(() => Argument.EnsureNotBlank("a", "   "));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void EnsureNotBlankThrowsArgumentExceptionWhenValueIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() => Argument.EnsureNotBlank("a", string.Empty));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void EnsureNotBlankThrowsArgumentNullExceptionWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Argument.EnsureNotBlank("a", null));
        }

        [Fact]
        public void EnsureNotBlankDoesNotThrowsWhenValueIsNotBlank()
        {
            Argument.EnsureNotBlank("a", "b");
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void EnsureIsDefinedEnumThrowsArgumentOutOfRangeExceptionWhenValueIsNotDefinedEnum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Argument.EnsureIsDefinedEnum("a", ((TestEnum)9)));
        }

        [Fact]
        public void EnsureIsDefinedEnumDoesNotThrowWhenValueIsDefinedEnum()
        {
            Argument.EnsureIsDefinedEnum("a", TestEnum.Two);
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void EnsureNotEmptyThrowsArgumentExceptionWhenValuesIsNull()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Argument.EnsureNotEmpty("a", null));
        }

        enum TestEnum
        {
            Two = 2,
        }
    }
}