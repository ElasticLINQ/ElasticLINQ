// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IQToolkit;
using IQToolkit.Data;
using IQToolkit.Data.Mapping;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = DbEntityProvider.From(@"c:\data\Northwind.mdf", "Test.NorthwindWithAttributes");
            //var provider = DbEntityProvider.From(@"c:\data\Northwind.accdb", "Test.NorthwindWithAttributes");
            //var provider = DbEntityProvider.From(@"c:\data\Northwind.mdb", "Test.NorthwindWithAttributes");
            //var provider = DbEntityProvider.From(@"c:\data\Northwind.sdf", "Test.NorthwindWithAttributes");            
            //var provider = DbEntityProvider.From("IQToolkit.Data.MySqlClient", "Northwind", "Test.MySqlNorthwind");
            //var provider = DbEntityProvider.From("IQToolkit.Data.SQLite", @"c:\data\Northwind.db3", "Test.NorthwindWithAttributes");

            //provider.Log = Console.Out;
            provider.Connection.Open();
            //provider.Cache = new QueryCache(5);

            try
            {
                var db = new Northwind(provider);

                //NorthwindTranslationTests.Run(db, true);
                NorthwindExecutionTests.Run(db);
                //NorthwindCUDTests.Run(db); 
                //MultiTableTests.Run(new MultiTableContext(provider.New(new AttributeMapping(typeof(MultiTableContext)))));
                //NorthwindPerfTests.Run(db, "TestStandardQuery");
            }
            finally
            {
                provider.Connection.Close();
            }
        }
    }
}
