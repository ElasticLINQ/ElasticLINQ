// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;

namespace ElasticLinq.Request.Filters
{
    /// <summary>
    /// Filter that inverts the logic of it's child filter.
    /// </summary>
    internal class NotFilter : IFilter
    {
        private readonly IFilter childFilter;

        public static IFilter Create(IFilter childFilter)
        {
            Argument.EnsureNotNull("childFilter", childFilter);

            // Not inside a not cancels out
            if (childFilter is NotFilter)
                return ((NotFilter) childFilter).ChildFilter;

            return new NotFilter(childFilter);
        }

        private NotFilter(IFilter childFilter)
        {
            this.childFilter = childFilter;
        }

        public string Name
        {
            get { return "not"; }
        }

        public IFilter ChildFilter
        {
            get { return childFilter; }
        }
    }
}