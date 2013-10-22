// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;
using System;

namespace ElasticLinq.Request.Filters
{
    internal abstract class SingleFieldFilter : IFilter
    {
        private readonly string field;

        protected SingleFieldFilter(string field)
        {
            Argument.EnsureNotBlank("field", field);

            this.field = field;
        }

        public string Field
        {
            get { return field; }
        }

        public abstract string Name
        {
            get;
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", Name, Field);
        }
    }
}