// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;

namespace ElasticLinq.Request.Filters
{
    internal class TermFilter : IFilter
    {
        private readonly string field;
        private readonly List<object> values;

        public TermFilter(string field, IEnumerable<object> values)
        {
            this.field = field;
            this.values = new List<object>(values);
        }

        public TermFilter(string field, object value)
            : this(field, new[] { value })
        {
        }

        public string Field
        {
            get { return field; }
        }

        public IReadOnlyList<Object> Values
        {
            get { return values.AsReadOnly(); }
        }

        public string Name
        {
            get { return Values.Count == 1 ? "term" : "terms"; }
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", Name, String.Join(",", values.ToArray()));
        }
    }
}
