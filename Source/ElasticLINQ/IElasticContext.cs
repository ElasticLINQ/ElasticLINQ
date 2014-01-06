using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq
{
    public interface IElasticContext
    {
        ElasticQuery<T> Query<T>();
    }
}
