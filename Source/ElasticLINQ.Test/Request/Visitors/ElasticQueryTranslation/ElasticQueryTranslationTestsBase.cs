// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using ElasticLinq.Connection;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationTestsBase
    {
        protected static readonly IElasticConnection Connection = new HttpElasticConnection(new Uri("http://localhost"));
        protected static readonly IElasticMapping Mapping = new TrivialElasticMapping();
        protected static readonly ILog Log = NullLog.Instance;
        protected static readonly IRetryPolicy RetryPolicy = NullRetryPolicy.Instance;
        protected static readonly ElasticQueryProvider SharedProvider = new ElasticQueryProvider(Connection, Mapping, Log, RetryPolicy, "prefix");

        protected static IQueryable<Robot> Robots
        {
            get { return new ElasticQuery<Robot>(SharedProvider); }
        }

        protected class Robot
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }
            public DateTime Started { get; set; }
            public decimal Cost { get; set; }
            public double EnergyUse { get; set; }
            public int? Zone { get; set; }
            public List<string> Aliases { get; set; }
            public RobotStats Stats { get; set; }
        }

        protected class RobotStats
        {
            public RobotLimbs Limbs { get; set; }
            public RobotPricing Pricing { get; set; }
        }

        public class RobotLimbs
        {
            public Int32 HandCount { get; set; }
        }

        public class RobotPricing
        {
            public decimal InvoicePrice { get; set; }
        }
    }
}