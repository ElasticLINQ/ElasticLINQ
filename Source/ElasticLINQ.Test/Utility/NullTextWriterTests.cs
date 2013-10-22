// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using Xunit;

namespace ElasticLINQ.Test.Utility
{
    public class NullTextWriterTests
    {
        [Fact]
        public void WriteLineDoesNothing()
        {
            Assert.DoesNotThrow(() => new NullTextWriter().WriteLine("Don't crash"));
        }
    }
}