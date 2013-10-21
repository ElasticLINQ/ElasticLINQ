// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Filter that selects documents if they have any value
    /// in the specified field.
    /// </summary>
    internal class ExistsFilter : IFilter
    {
        private readonly string field;

        public ExistsFilter(string field)
        {
            Argument.EnsureNotBlank("field", field);

            this.field = field;
        }

        public string Field
        {
            get { return field; }
        }

        public string Name
        {
            get { return "exists"; }
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", Name, Field);
        }
    }
}