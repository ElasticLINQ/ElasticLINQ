// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Filter that selects documents if they do not have a value
    /// in the specified field.
    /// </summary>
    internal class MissingFilter : SingleFieldFilter, INegatableFilter
    {
        public MissingFilter(string field)
            : base(field)
        {
        }

        public override string Name
        {
            get { return "missing"; }
        }

        public IFilter Negate()
        {
            return new ExistsFilter(Field);
        }
    }
}