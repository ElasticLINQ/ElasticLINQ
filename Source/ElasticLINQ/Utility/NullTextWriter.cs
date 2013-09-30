// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.IO;
using System.Text;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// TextWriter that doesn't actually write to avoid null handling.
    /// </summary>
    internal class NullTextWriter : TextWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
