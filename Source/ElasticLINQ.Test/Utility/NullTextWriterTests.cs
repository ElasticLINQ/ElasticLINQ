// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System.Text;
using Xunit;

namespace ElasticLINQ.Test.Utility
{
    public class NullTextWriterTests
    {
        [Fact]
        public void WriteLineDoesNothing()
        {
            var writer = new NullTextWriter();
            Assert.DoesNotThrow(() => writer.WriteLine("Don't crash"));
        }

        [Fact]
        public void EncodingIsUTF8()
        {
            var writer = new NullTextWriter();
            Assert.Equal(Encoding.UTF8, writer.Encoding);
        }
    }
}