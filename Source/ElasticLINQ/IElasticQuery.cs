// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq
{
    public interface IElasticQuery<T> : IOrderedQueryable<T>
    {
        string ToElasticSearchQuery();
    }
}
