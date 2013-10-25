// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq;
using ElasticLinq.Mapping;
using System;
using System.Linq;

namespace ElasticLINQ.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationTestsBase
    {
        protected static readonly ElasticConnection Connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));
        protected static readonly IElasticMapping Mapping = new TrivialElasticMapping();
        protected static readonly ElasticQueryProvider SharedProvider = new ElasticQueryProvider(Connection, Mapping);

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
        }
    }
}