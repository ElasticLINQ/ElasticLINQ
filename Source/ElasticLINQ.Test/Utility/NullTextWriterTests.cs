// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq.Utility;
using Xunit;

namespace ElasticLINQ.Test.Utility
{
    public class NullTextWriterTests
    {
        [Fact]
        public void WriteLineDoesNothing()
        {
            new NullTextWriter().WriteLine("Don't crash");
        }
    }
}