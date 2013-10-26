// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that selects documents if they do not have a value
    /// in the specified field.
    /// </summary>
    internal class MissingCriteria : SingleFieldCriteria, INegatableCriteria
    {
        public MissingCriteria(string field)
            : base(field)
        {
        }

        public override string Name
        {
            get { return "missing"; }
        }

        public ICriteria Negate()
        {
            return new ExistsCriteria(Field);
        }
    }
}