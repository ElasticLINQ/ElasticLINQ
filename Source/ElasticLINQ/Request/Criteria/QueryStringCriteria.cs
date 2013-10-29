// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    public class QueryStringCriteria : ICriteria
    {
        private readonly string value;

        public QueryStringCriteria(string value)
        {
            Argument.EnsureNotBlank("value", value);
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public string Name
        {
            get { return "query_string"; }
        }
    }
}