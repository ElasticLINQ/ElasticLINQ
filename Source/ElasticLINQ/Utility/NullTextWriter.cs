// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

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
