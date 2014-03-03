// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Test
{
    public class TestableElasticContext : IElasticContext
    {
        private readonly Dictionary<Type, object> data = new Dictionary<Type, object>();

        public TestableElasticContext(IElasticMapping mapping = null,
                                      ILog log = null,
                                      int maxAttempts = 1,
                                      TimeSpan timeout = default(TimeSpan))
        {
            Connection = new ElasticConnection(new Uri("http://localhost/"), timeout: timeout);
            Mapping = mapping ?? new TrivialElasticMapping();
            Provider = new TestableElasticQueryProvider(this);
            Requests = new List<string>();
            Log = log ?? NullLog.Instance;
            RetryPolicy = new RetryPolicy(Log, 0, maxAttempts, NullDelay.Instance);
        }

        public ElasticConnection Connection { get; private set; }

        public ILog Log { get; private set; }

        public IElasticMapping Mapping { get; private set; }

        public TestableElasticQueryProvider Provider { get; private set; }

        public List<string> Requests { get; private set; }

        public IRetryPolicy RetryPolicy { get; private set; }

        public IEnumerable<T> Data<T>()
        {
            object result;
            if (!data.TryGetValue(typeof(T), out result))
                result = Enumerable.Empty<T>();

            return (IEnumerable<T>)result;
        }

        public void SetData<T>(IEnumerable<T> values)
        {
            data[typeof(T)] = values;
        }

        public void SetData<T>(params T[] values)
        {
            SetData((IEnumerable<T>)values);
        }

        public IQueryable<T> Query<T>()
        {
            return new TestableElasticQuery<T>(this);
        }
    }
}
