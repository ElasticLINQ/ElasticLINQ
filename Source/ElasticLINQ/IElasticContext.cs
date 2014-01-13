using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq
{
    public interface IElasticContext
    {
        IElasticQuery<T> Query<T>();
    }
}
