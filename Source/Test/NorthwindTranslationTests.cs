// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Test
{
    using IQToolkit.Data;
    using IQToolkit.Data.Mapping;

    public class NorthwindTranslationTests : NorthwindTestHarness
    {
        public static void Run(Northwind db, bool executeQueries)
        {
            string prefix = GetBaselinePrefix(db);
            string baselineFile = prefix + ".base";
            string newBase = prefix + ".new";

            new NorthwindTranslationTests().RunTests(db, @"..\..\" + baselineFile, newBase, executeQueries);
        }

        public static void Run(Northwind db, bool executeQueries, string testName)
        {
            string prefix = GetBaselinePrefix(db);
            string baselineFile = prefix + ".base";
            new NorthwindTranslationTests().RunTest(db, @"..\..\" + baselineFile, executeQueries, testName);
        }

        protected static string GetBaselinePrefix(Northwind db)
        {
            return "NorthwindTranslation_" + db.Provider.GetType().Name;
        }

        public void TestWhere()
        {
            TestQuery(db.Customers.Where(c => c.City == "London"));
        }

        public void TestWhereTrue()
        {
            TestQuery(db.Customers.Where(c => true));
        }

        public void TestWhereFalse()
        {
            TestQuery(db.Customers.Where(c => false));
        }

        public void TestCompareEntityEqual()
        {
            Customer alfki = new Customer { CustomerID = "ALFKI" };
            TestQuery(
                db.Customers.Where(c => c == alfki)
                );
        }

        public void TestCompareEntityNotEqual()
        {
            Customer alfki = new Customer { CustomerID = "ALFKI" };
            TestQuery(
                db.Customers.Where(c => c != alfki)
                );
        }

        public void TestCompareConstructedEqual()
        {
            TestQuery(
                db.Customers.Where(c => new { x = c.City } == new { x = "London" })
                );
        }

        public void TestCompareConstructedMultiValueEqual()
        {
            TestQuery(
                db.Customers.Where(c => new { x = c.City, y = c.Country } == new { x = "London", y = "UK" })
                );
        }

        public void TestCompareConstructedMultiValueNotEqual()
        {
            TestQuery(
                db.Customers.Where(c => new { x = c.City, y = c.Country } != new { x = "London", y = "UK" })
                );
        }

        public void TestCompareConstructed()
        {
            TestQuery(
                db.Customers.Where(c => new { x = c.City } == new { x = "London" })
                );
        }

        public void TestSelectScalar()
        {
            TestQuery(db.Customers.Select(c => c.City));
        }

        public void TestSelectAnonymousOne()
        {
            TestQuery(db.Customers.Select(c => new { c.City }));
        }

        public void TestSelectAnonymousTwo()
        {
            TestQuery(db.Customers.Select(c => new { c.City, c.Phone }));
        }

        public void TestSelectAnonymousThree()
        {
            TestQuery(db.Customers.Select(c => new { c.City, c.Phone, c.Country }));
        }

        public void TestSelectCustomerTable()
        {
            TestQuery(db.Customers);
        }

        public void TestSelectCustomerIdentity()
        {
            TestQuery(db.Customers.Select(c => c));
        }

        public void TestSelectAnonymousWithObject()
        {
            TestQuery(db.Customers.Select(c => new { c.City, c }));
        }

        public void TestSelectAnonymousNested()
        {
            TestQuery(db.Customers.Select(c => new { c.City, Country = new { c.Country } }));
        }

        public void TestSelectAnonymousEmpty()
        {
            TestQuery(db.Customers.Select(c => new { }));
        }

        public void TestSelectAnonymousLiteral()
        {
            TestQuery(db.Customers.Select(c => new { X = 10 }));
        }

        public void TestSelectConstantInt()
        {
            TestQuery(db.Customers.Select(c => 0));
        }

        public void TestSelectConstantNullString()
        {
            TestQuery(db.Customers.Select(c => (string)null));
        }

        public void TestSelectLocal()
        {
            int x = 10;
            TestQuery(db.Customers.Select(c => x));
        }

        public void TestSelectNestedCollection()
        {
            TestQuery(
                from c in db.Customers
                where c.City == "London"
                select db.Orders.Where(o => o.CustomerID == c.CustomerID && o.OrderDate.Year == 1997).Select(o => o.OrderID)
                );
        }

        public void TestSelectNestedCollectionInAnonymousType()
        {
            TestQuery(
                from c in db.Customers
                where c.CustomerID == "ALFKI"
                select new { Foos = db.Orders.Where(o => o.CustomerID == c.CustomerID && o.OrderDate.Year == 1997).Select(o => o.OrderID) }
                );
        }

        public void TestJoinCustomerOrders()
        {
            TestQuery(
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID
                select new { c.ContactName, o.OrderID }
                );
        }

        public void TestJoinMultiKey()
        {
            TestQuery(
                from c in db.Customers
                join o in db.Orders on new { a = c.CustomerID, b = c.CustomerID } equals new { a = o.CustomerID, b = o.CustomerID }
                select new { c, o }
                );
        }

        public void TestJoinIntoCustomersOrders()
        {
            TestQuery(
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID into ords
                select new { cust = c, ords = ords.ToList() }
                );
        }

        public void TestJoinIntoCustomersOrdersCount()
        {
            TestQuery(
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID into ords
                select new { cust = c, ords = ords.Count() }
                );
        }

        public void TestJoinIntoDefaultIfEmpty()
        {
            TestQuery(
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID into ords
                from o in ords.DefaultIfEmpty()
                select new { c, o }
                );
        }

        public void TestSelectManyCustomerOrders()
        {
            TestQuery(
                from c in db.Customers
                from o in db.Orders
                where c.CustomerID == o.CustomerID
                select new { c.ContactName, o.OrderID }
                );
        }

        public void TestMultipleJoinsWithJoinConditionsInWhere()
        {
            // this should reduce to inner joins
            TestQuery(
                from c in db.Customers
                from o in db.Orders
                from d in db.OrderDetails
                where o.CustomerID == c.CustomerID && o.OrderID == d.OrderID
                where c.CustomerID == "ALFKI"
                select d.ProductID
                );
        }

        public void TestMultipleJoinsWithMissingJoinCondition()
        {
            // this should force a naked cross join
            TestQuery(
                from c in db.Customers
                from o in db.Orders
                from d in db.OrderDetails
                where o.CustomerID == c.CustomerID /*&& o.OrderID == d.OrderID*/
                where c.CustomerID == "ALFKI"
                select d.ProductID
                );
        }

        public void TestOrderBy()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.CustomerID)
                );
        }

        public void TestOrderBySelect()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.CustomerID).Select(c => c.ContactName)
                );
        }

        public void TestOrderByOrderBy()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.CustomerID).OrderBy(c => c.Country).Select(c => c.City)
                );
        }

        public void TestOrderByThenBy()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.CustomerID).ThenBy(c => c.Country).Select(c => c.City)
                );
        }

        public void TestOrderByDescending()
        {
            TestQuery(
                db.Customers.OrderByDescending(c => c.CustomerID).Select(c => c.City)
                );
        }

        public void TestOrderByDescendingThenBy()
        {
            TestQuery(
                db.Customers.OrderByDescending(c => c.CustomerID).ThenBy(c => c.Country).Select(c => c.City)
                );
        }

        public void TestOrderByDescendingThenByDescending()
        {
            TestQuery(
                db.Customers.OrderByDescending(c => c.CustomerID).ThenByDescending(c => c.Country).Select(c => c.City)
                );
        }

        public void TestOrderByJoin()
        {
            TestQuery(
                from c in db.Customers.OrderBy(c => c.CustomerID)
                join o in db.Orders.OrderBy(o => o.OrderID) on c.CustomerID equals o.CustomerID
                select new { c.CustomerID, o.OrderID }
                );
        }

        public void TestOrderBySelectMany()
        {
            TestQuery(
                from c in db.Customers.OrderBy(c => c.CustomerID)
                from o in db.Orders.OrderBy(o => o.OrderID)
                where c.CustomerID == o.CustomerID
                select new { c.ContactName, o.OrderID }
                );
        }

        public void TestGroupBy()
        {
            TestQuery(
                db.Customers.GroupBy(c => c.City)
                );
        }

        public void TestGroupBySelectMany()
        {
            TestQuery(
                db.Customers.GroupBy(c => c.City).SelectMany(g => g)
                );
        }

        public void TestGroupBySum()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID).Select(g => g.Sum(o => o.OrderID))
                );
        }

        public void TestGroupByCount()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID).Select(g => g.Count())
                );
        }

        public void TestGroupByLongCount()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID).Select(g => g.LongCount())
                );
        }

        public void TestGroupBySumMinMaxAvg()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID).Select(g =>
                    new
                    {
                        Sum = g.Sum(o => o.OrderID),
                        Min = g.Min(o => o.OrderID),
                        Max = g.Max(o => o.OrderID),
                        Avg = g.Average(o => o.OrderID)
                    })
                );
        }

        public void TestGroupByWithResultSelector()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID, (k, g) =>
                    new
                    {
                        Sum = g.Sum(o => o.OrderID),
                        Min = g.Min(o => o.OrderID),
                        Max = g.Max(o => o.OrderID),
                        Avg = g.Average(o => o.OrderID)
                    })
                );
        }

        public void TestGroupByWithElementSelectorSum()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID, o => o.OrderID).Select(g => g.Sum())
                );
        }

        public void TestGroupByWithElementSelector()
        {
            // note: groups are retrieved through a separately execute subquery per row
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID, o => o.OrderID)
                );
        }

        public void TestGroupByWithElementSelectorSumMax()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID, o => o.OrderID).Select(g => new { Sum = g.Sum(), Max = g.Max() })
                );
        }

        public void TestGroupByWithAnonymousElement()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID, o => new { o.OrderID }).Select(g => g.Sum(x => x.OrderID))
                );
        }

        public void TestGroupByWithTwoPartKey()
        {
            TestQuery(
                db.Orders.GroupBy(o => new { o.CustomerID, o.OrderDate }).Select(g => g.Sum(o => o.OrderID))
                );
        }

        public void TestOrderByGroupBy()
        {
            // note: order-by is lost when group-by is applied (the sequence of groups is not ordered)
            TestQuery(
                db.Orders.OrderBy(o => o.OrderID).GroupBy(o => o.CustomerID).Select(g => g.Sum(o => o.OrderID))
                );
        }

        public void TestOrderByGroupBySelectMany()
        {
            // note: order-by is preserved within grouped sub-collections
            TestQuery(
                db.Orders.OrderBy(o => o.OrderID).GroupBy(o => o.CustomerID).SelectMany(g => g)
                );
        }

        public void TestSumWithNoArg()
        {
            TestQuery(
                () => db.Orders.Select(o => o.OrderID).Sum()
                );
        }

        public void TestSumWithArg()
        {
            TestQuery(
                () => db.Orders.Sum(o => o.OrderID)
                );
        }

        public void TestCountWithNoPredicate()
        {
            TestQuery(
                () => db.Orders.Count()
                );
        }

        public void TestCountWithPredicate()
        {
            TestQuery(
                () => db.Orders.Count(o => o.CustomerID == "ALFKI")
                );
        }

        public void TestDistinct()
        {
            TestQuery(
                db.Customers.Distinct()
                );
        }

        public void TestDistinctScalar()
        {
            TestQuery(
                db.Customers.Select(c => c.City).Distinct()
                );
        }

        public void TestOrderByDistinct()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.CustomerID).Select(c => c.City).Distinct()
                );
        }

        public void TestDistinctOrderBy()
        {
            TestQuery(
                db.Customers.Select(c => c.City).Distinct().OrderBy(c => c)
                );
        }

        public void TestDistinctGroupBy()
        {
            TestQuery(
                db.Orders.Distinct().GroupBy(o => o.CustomerID)
                );
        }

        public void TestGroupByDistinct()
        {
            TestQuery(
                db.Orders.GroupBy(o => o.CustomerID).Distinct()
                );

        }

        public void TestDistinctCount()
        {
            TestQuery(
                () => db.Customers.Distinct().Count()
                );
        }

        public void TestSelectDistinctCount()
        {
            // cannot do: SELECT COUNT(DISTINCT some-colum) FROM some-table
            // because COUNT(DISTINCT some-column) does not count nulls
            TestQuery(
                () => db.Customers.Select(c => c.City).Distinct().Count()
                );
        }

        public void TestSelectSelectDistinctCount()
        {
            TestQuery(
                () => db.Customers.Select(c => c.City).Select(c => c).Distinct().Count()
                );
        }

        public void TestDistinctCountPredicate()
        {
            TestQuery(
                () => db.Customers.Distinct().Count(c => c.CustomerID == "ALFKI")
                );
        }

        public void TestDistinctSumWithArg()
        {
            TestQuery(
                () => db.Orders.Distinct().Sum(o => o.OrderID)
                );
        }

        public void TestSelectDistinctSum()
        {
            TestQuery(
                () => db.Orders.Select(o => o.OrderID).Distinct().Sum()
                );
        }

        public void TestTake()
        {
            TestQuery(
                db.Orders.Take(5)
                );
        }

        public void TestTakeDistinct()
        {
            // distinct must be forced to apply after top has been computed
            TestQuery(
                db.Orders.Take(5).Distinct()
                );
        }

        public void TestDistinctTake()
        {
            // top must be forced to apply after distinct has been computed
            TestQuery(
                db.Orders.Distinct().Take(5)
                );
        }

        public void TestDistinctTakeCount()
        {
            TestQuery(
                () => db.Orders.Distinct().Take(5).Count()
                );
        }

        public void TestTakeDistinctCount()
        {
            TestQuery(
                () => db.Orders.Take(5).Distinct().Count()
                );
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestSkip()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Skip(5)
                );
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestTakeSkip()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Take(10).Skip(5)
                );
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestDistinctSkip()
        {
            TestQuery(
                db.Customers.Distinct().OrderBy(c => c.ContactName).Skip(5)
                );
        }

        public void TestSkipTake()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Skip(5).Take(10)
                );
        }

        public void TestDistinctSkipTake()
        {
            TestQuery(
                db.Customers.Distinct().OrderBy(c => c.ContactName).Skip(5).Take(10)
                );
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestSkipDistinct()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Skip(5).Distinct()
                );
        }

        //[ExcludeProvider("Access")]
        public void TestSkipTakeDistinct()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Skip(5).Take(10).Distinct()
                );
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestTakeSkipDistinct()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Take(10).Skip(5).Distinct()
                );
        }

        public void TestFirst()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).First()
                );
        }

        public void TestFirstPredicate()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).First(c => c.City == "London")
                );
        }

        public void TestWhereFirst()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").First()
                );
        }

        public void TestFirstOrDefault()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).FirstOrDefault()
                );
        }

        public void TestFirstOrDefaultPredicate()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).FirstOrDefault(c => c.City == "London")
                );
        }

        public void TestWhereFirstOrDefault()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").FirstOrDefault()
                );
        }

        public void TestReverse()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Reverse()
                );
        }

        public void TestReverseReverse()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Reverse().Reverse()
                );
        }

        public void TestReverseWhereReverse()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Reverse().Where(c => c.City == "London").Reverse()
                );
        }

        public void TestReverseTakeReverse()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Reverse().Take(5).Reverse()
                );
        }

        public void TestReverseWhereTakeReverse()
        {
            TestQuery(
                db.Customers.OrderBy(c => c.ContactName).Reverse().Where(c => c.City == "London").Take(5).Reverse()
                );
        }

        public void TestLast()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).Last()
                );
        }

        public void TestLastPredicate()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).Last(c => c.City == "London")
                );
        }

        public void TestWhereLast()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").Last()
                );
        }

        public void TestLastOrDefault()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).LastOrDefault()
                );
        }

        public void TestLastOrDefaultPredicate()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).LastOrDefault(c => c.City == "London")
                );
        }

        public void TestWhereLastOrDefault()
        {
            TestQuery(
                () => db.Customers.OrderBy(c => c.ContactName).Where(c => c.City == "London").LastOrDefault()
                );
        }

        public void TestSingle()
        {
            TestQueryFails(
                () => db.Customers.Single()
                );
        }

        public void TestSinglePredicate()
        {
            TestQuery(
                () => db.Customers.Single(c => c.CustomerID == "ALFKI")
                );
        }

        public void TestWhereSingle()
        {
            TestQuery(
                () => db.Customers.Where(c => c.CustomerID == "ALFKI").Single()
                );
        }

        public void TestSingleOrDefault()
        {
            TestQueryFails(
                () => db.Customers.SingleOrDefault()
                );
        }

        public void TestSingleOrDefaultPredicate()
        {
            TestQuery(
                () => db.Customers.SingleOrDefault(c => c.CustomerID == "ALFKI")
                );
        }

        public void TestWhereSingleOrDefault()
        {
            TestQuery(
                () => db.Customers.Where(c => c.CustomerID == "ALFKI").SingleOrDefault()
                );
        }

        public void TestAnyWithSubquery()
        {
            TestQuery(
                db.Customers.Where(c => db.Orders.Where(o => o.CustomerID == c.CustomerID).Any(o => o.OrderDate.Year == 1997))
                );
        }

        public void TestAnyWithSubqueryNoPredicate()
        {
            TestQuery(
                db.Customers.Where(c => db.Orders.Where(o => o.CustomerID == c.CustomerID).Any())
                );
        }

        public void TestAnyWithLocalCollection()
        {
            string[] ids = new[] { "ABCDE", "ALFKI" };
            TestQuery(
                db.Customers.Where(c => ids.Any(id => c.CustomerID == id))
                );
        }

        public void TestAnyTopLevel()
        {
            TestQuery(
                () => db.Customers.Any()
                );
        }

        public void TestAllWithSubquery()
        {
            TestQuery(
                db.Customers.Where(c => db.Orders.Where(o => o.CustomerID == c.CustomerID).All(o => o.OrderDate.Year == 1997))
                );
        }

        public void TestAllWithLocalCollection()
        {
            string[] patterns = new[] { "a", "e" };

            TestQuery(
                db.Customers.Where(c => patterns.All(p => c.ContactName.Contains(p)))
                );
        }

        public void TestAllTopLevel()
        {
            TestQuery(
                () => db.Customers.All(c => c.ContactName.StartsWith("a"))
                );
        }

        public void TestContainsWithSubquery()
        {
            TestQuery(
                db.Customers.Where(c => db.Orders.Select(o => o.CustomerID).Contains(c.CustomerID))
                );
        }

        public void TestContainsWithLocalCollection()
        {
            string[] ids = new[] { "ABCDE", "ALFKI" };
            TestQuery(
                db.Customers.Where(c => ids.Contains(c.CustomerID))
                );
        }

        public void TestContainsTopLevel()
        {
            TestQuery(
                () => db.Customers.Select(c => c.CustomerID).Contains("ALFKI")
                );
        }



        public void TestCoalesce()
        {
            TestQuery(db.Customers.Where(c => (c.City ?? "Seattle") == "Seattle"));
        }

        public void TestCoalesce2()
        {
            TestQuery(db.Customers.Where(c => (c.City ?? c.Country ?? "Seattle") == "Seattle"));
        }


        // framework function tests

        public void TestStringLength()
        {
            TestQuery(db.Customers.Where(c => c.City.Length == 7));
        }

        public void TestStringStartsWithLiteral()
        {
            TestQuery(db.Customers.Where(c => c.ContactName.StartsWith("M")));
        }

        public void TestStringStartsWithColumn()
        {
            TestQuery(db.Customers.Where(c => c.ContactName.StartsWith(c.ContactName)));
        }

        public void TestStringEndsWithLiteral()
        {
            TestQuery(db.Customers.Where(c => c.ContactName.EndsWith("s")));
        }

        public void TestStringEndsWithColumn()
        {
            TestQuery(db.Customers.Where(c => c.ContactName.EndsWith(c.ContactName)));
        }

        public void TestStringContainsLiteral()
        {
            TestQuery(db.Customers.Where(c => c.ContactName.Contains("and")));
        }

        public void TestStringContainsColumn()
        {
            TestQuery(db.Customers.Where(c => c.ContactName.Contains(c.ContactName)));
        }

        public void TestStringConcatImplicit2Args()
        {
            TestQuery(db.Customers.Where(c => c.ContactName + "X" == "X"));
        }

        public void TestStringConcatExplicit2Args()
        {
            TestQuery(db.Customers.Where(c => string.Concat(c.ContactName, "X") == "X"));
        }

        public void TestStringConcatExplicit3Args()
        {
            TestQuery(db.Customers.Where(c => string.Concat(c.ContactName, "X", c.Country) == "X"));
        }

        public void TestStringConcatExplicitNArgs()
        {
            TestQuery(db.Customers.Where(c => string.Concat(new string[] { c.ContactName, "X", c.Country }) == "X"));
        }

        public void TestStringIsNullOrEmpty()
        {
            TestQuery(db.Customers.Where(c => string.IsNullOrEmpty(c.City)));
        }

        public void TestStringToUpper()
        {
            TestQuery(db.Customers.Where(c => c.City.ToUpper() == "SEATTLE"));
        }

        public void TestStringToLower()
        {
            TestQuery(db.Customers.Where(c => c.City.ToLower() == "seattle"));
        }

        public void TestStringSubstring()
        {
            TestQuery(db.Customers.Where(c => c.City.Substring(0, 4) == "Seat"));
        }

        public void TestStringSubstringNoLength()
        {
            TestQuery(db.Customers.Where(c => c.City.Substring(4) == "tle"));
        }

        [ExcludeProvider("SQLite")]
        public void TestStringIndexOf()
        {
            TestQuery(db.Customers.Where(c => c.City.IndexOf("tt") == 4));
        }

        [ExcludeProvider("SQLite")]
        public void TestStringIndexOfChar()
        {
            TestQuery(db.Customers.Where(c => c.City.IndexOf('t') == 4));
        }

        public void TestStringTrim()
        {
            TestQuery(db.Customers.Where(c => c.City.Trim() == "Seattle"));
        }

        public void TestStringToString()
        {
            // just to prove this is a no op
            TestQuery(db.Customers.Where(c => c.City.ToString() == "Seattle"));
        }

        [ExcludeProvider("Access")]
        public void TestStringReplace()
        {
            TestQuery(db.Customers.Where(c => c.City.Replace("ea", "ae") == "Saettle"));
        }

        [ExcludeProvider("Access")]
        public void TestStringReplaceChars()
        {
            TestQuery(db.Customers.Where(c => c.City.Replace("e", "y") == "Syattly"));
        }

        [ExcludeProvider("Access")]
        public void TestStringRemove()
        {
            TestQuery(db.Customers.Where(c => c.City.Remove(1, 2) == "Sttle"));
        }

        [ExcludeProvider("Access")]
        public void TestStringRemoveNoCount()
        {
            TestQuery(db.Customers.Where(c => c.City.Remove(4) == "Seat"));
        }

        public void TestDateTimeConstructYMD()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate == new DateTime(o.OrderDate.Year, 1, 1)));
        }

        public void TestDateTimeConstructYMDHMS()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate == new DateTime(o.OrderDate.Year, 1, 1, 10, 25, 55)));
        }

        public void TestDateTimeDay()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Day == 5));
        }

        public void TestDateTimeMonth()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Month == 12));
        }

        public void TestDateTimeYear()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Year == 1997));
        }

        public void TestDateTimeHour()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Hour == 6));
        }

        public void TestDateTimeMinute()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Minute == 32));
        }

        public void TestDateTimeSecond()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Second == 47));
        }

        [ExcludeProvider("Access")]
        public void TestDateTimeMillisecond()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.Millisecond == 200));
        }

        [ExcludeProvider("Access")]
        public void TestDateTimeDayOfYear()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.DayOfYear == 360));
        }

        public void TestDateTimeDayOfWeek()
        {
            TestQuery(db.Orders.Where(o => o.OrderDate.DayOfWeek == DayOfWeek.Friday));
        }

        public void TestMathAbs()
        {
            TestQuery(db.Orders.Where(o => Math.Abs(o.OrderID) == 10));
        }

        public void TestMathAtan()
        {
            TestQuery(db.Orders.Where(o => Math.Atan(o.OrderID) == 0));
        }

        public void TestMathCos()
        {
            TestQuery(db.Orders.Where(o => Math.Cos(o.OrderID) == 0));
        }

        public void TestMathSin()
        {
            TestQuery(db.Orders.Where(o => Math.Sin(o.OrderID) == 0));
        }

        public void TestMathTan()
        {
            TestQuery(db.Orders.Where(o => Math.Tan(o.OrderID) == 0));
        }

        public void TestMathExp()
        {
            TestQuery(db.Orders.Where(o => Math.Exp(o.OrderID < 1000 ? 1 : 2) == 0));
        }

        public void TestMathLog()
        {
            TestQuery(db.Orders.Where(o => Math.Log(o.OrderID) == 0));
        }

        public void TestMathSqrt()
        {
            TestQuery(db.Orders.Where(o => Math.Sqrt(o.OrderID) == 0));
        }

        public void TestMathPow()
        {
            TestQuery(db.Orders.Where(o => Math.Pow(o.OrderID < 1000 ? 1 : 2, 3) == 0));
        }

        public void TestMathRoundDefault()
        {
            TestQuery(db.Orders.Where(o => Math.Round((decimal)o.OrderID) == 0));
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestMathAcos()
        {
            TestQuery(db.Orders.Where(o => Math.Acos(1.0/o.OrderID) == 0));
        }

        [ExcludeProvider("Access")]
        [ExcludeProvider("SqlCe")]
        public void TestMathAsin()
        {
            TestQuery(db.Orders.Where(o => Math.Asin(1.0/o.OrderID) == 0));
        }

        [ExcludeProvider("Access")]
        public void TestMathAtan2()
        {
            TestQuery(db.Orders.Where(o => Math.Atan2(1.0/o.OrderID, 3) == 0));
        }

        [ExcludeProvider("SQLite")]
        [ExcludeProvider("Access")]
        public void TestMathLog10()
        {
            TestQuery(db.Orders.Where(o => Math.Log10(o.OrderID) == 0));
        }

        [ExcludeProvider("SQLite")]
        [ExcludeProvider("Access")]
        public void TestMathCeiling()
        {
            TestQuery(db.Orders.Where(o => Math.Ceiling((double)o.OrderID) == 0));
        }

        [ExcludeProvider("Access")]
        public void TestMathRoundToPlace()
        {
            TestQuery(db.Orders.Where(o => Math.Round((decimal)o.OrderID, 2) == 0));
        }

        [ExcludeProvider("SQLite")]
        [ExcludeProvider("Access")]
        public void TestMathFloor()
        {
            TestQuery(db.Orders.Where(o => Math.Floor((double)o.OrderID) == 0));
        }

        [ExcludeProvider("SQLite")]
        public void TestMathTruncate()
        {
            TestQuery(db.Orders.Where(o => Math.Truncate((double)o.OrderID) == 0));
        }

        public void TestStringCompareToLT()
        {
            TestQuery(db.Customers.Where(c => c.City.CompareTo("Seattle") < 0));
        }

        public void TestStringCompareToLE()
        {
            TestQuery(db.Customers.Where(c => c.City.CompareTo("Seattle") <= 0));
        }

        public void TestStringCompareToGT()
        {
            TestQuery(db.Customers.Where(c => c.City.CompareTo("Seattle") > 0));
        }

        public void TestStringCompareToGE()
        {
            TestQuery(db.Customers.Where(c => c.City.CompareTo("Seattle") >= 0));
        }

        public void TestStringCompareToEQ()
        {
            TestQuery(db.Customers.Where(c => c.City.CompareTo("Seattle") == 0));
        }

        public void TestStringCompareToNE()
        {
            TestQuery(db.Customers.Where(c => c.City.CompareTo("Seattle") != 0));
        }

        public void TestStringCompareLT()
        {
            TestQuery(db.Customers.Where(c => string.Compare(c.City, "Seattle") < 0));
        }

        public void TestStringCompareLE()
        {
            TestQuery(db.Customers.Where(c => string.Compare(c.City, "Seattle") <= 0));
        }

        public void TestStringCompareGT()
        {
            TestQuery(db.Customers.Where(c => string.Compare(c.City, "Seattle") > 0));
        }

        public void TestStringCompareGE()
        {
            TestQuery(db.Customers.Where(c => string.Compare(c.City, "Seattle") >= 0));
        }

        public void TestStringCompareEQ()
        {
            TestQuery(db.Customers.Where(c => string.Compare(c.City, "Seattle") == 0));
        }

        public void TestStringCompareNE()
        {
            TestQuery(db.Customers.Where(c => string.Compare(c.City, "Seattle") != 0));
        }

        public void TestIntCompareTo()
        {
            // prove that x.CompareTo(y) works for types other than string
            TestQuery(db.Orders.Where(o => o.OrderID.CompareTo(1000) == 0));
        }

        public void TestDecimalCompare()
        {
            // prove that type.Compare(x,y) works with decimal
            TestQuery(db.Orders.Where(o => decimal.Compare((decimal)o.OrderID, 0.0m) == 0));
        }

        public void TestDecimalAdd()
        {
            TestQuery(db.Orders.Where(o => decimal.Add(o.OrderID, 0.0m) == 0.0m));
        }

        public void TestDecimalSubtract()
        {
            TestQuery(db.Orders.Where(o => decimal.Subtract(o.OrderID, 0.0m) == 0.0m));
        }

        public void TestDecimalMultiply()
        {
            TestQuery(db.Orders.Where(o => decimal.Multiply(o.OrderID, 1.0m) == 1.0m));
        }

        public void TestDecimalDivide()
        {
            TestQuery(db.Orders.Where(o => decimal.Divide(o.OrderID, 1.0m) == 1.0m));
        }

        [ExcludeProvider("SqlCe")]
        public void TestDecimalRemainder()
        {
            TestQuery(db.Orders.Where(o => decimal.Remainder(o.OrderID, 1.0m) == 0.0m));
        }

        public void TestDecimalNegate()
        {
            TestQuery(db.Orders.Where(o => decimal.Negate(o.OrderID) == 1.0m));
        }

        public void TestDecimalRoundDefault()
        {
            TestQuery(db.Orders.Where(o => decimal.Round(o.OrderID) == 0m));
        }

        [ExcludeProvider("Access")]
        public void TestDecimalRoundPlaces()
        {
            TestQuery(db.Orders.Where(o => decimal.Round(o.OrderID, 2) == 0.00m));
        }

        [ExcludeProvider("SQLite")]
        public void TestDecimalTruncate()
        {
            TestQuery(db.Orders.Where(o => decimal.Truncate(o.OrderID) == 0m));
        }

        [ExcludeProvider("SQLite")]
        [ExcludeProvider("Access")]
        public void TestDecimalCeiling()
        {
            TestQuery(db.Orders.Where(o => decimal.Ceiling(o.OrderID) == 0.0m));
        }

        [ExcludeProvider("SQLite")]
        [ExcludeProvider("Access")]
        public void TestDecimalFloor()
        {
            TestQuery(db.Orders.Where(o => decimal.Floor(o.OrderID) == 0.0m));
        }

        public void TestDecimalLT()
        {
            // prove that decimals are treated normally with respect to normal comparison operators
            TestQuery(db.Orders.Where(o => ((decimal)o.OrderID) < 0.0m));
        }

        public void TestIntLessThan()
        {
            TestQuery(db.Orders.Where(o => o.OrderID < 0));
        }

        public void TestIntLessThanOrEqual()
        {
            TestQuery(db.Orders.Where(o => o.OrderID <= 0));
        }

        public void TestIntGreaterThan()
        {
            TestQuery(db.Orders.Where(o => o.OrderID > 0));
        }

        public void TestIntGreaterThanOrEqual()
        {
            TestQuery(db.Orders.Where(o => o.OrderID >= 0));
        }

        public void TestIntEqual()
        {
            TestQuery(db.Orders.Where(o => o.OrderID == 0));
        }

        public void TestIntNotEqual()
        {
            TestQuery(db.Orders.Where(o => o.OrderID != 0));
        }

        public void TestIntAdd()
        {
            TestQuery(db.Orders.Where(o => o.OrderID + 0 == 0));
        }

        public void TestIntSubtract()
        {
            TestQuery(db.Orders.Where(o => o.OrderID - 0 == 0));
        }

        public void TestIntMultiply()
        {
            TestQuery(db.Orders.Where(o => o.OrderID * 1 == 1));
        }

        public void TestIntDivide()
        {
            TestQuery(db.Orders.Where(o => o.OrderID / 1 == 1));
        }

        public void TestIntModulo()
        {
            TestQuery(db.Orders.Where(o => o.OrderID % 1 == 0));
        }

        public void TestIntLeftShift()
        {
            TestQuery(db.Orders.Where(o => o.OrderID << 1 == 0));
        }

        public void TestIntRightShift()
        {
            TestQuery(db.Orders.Where(o => o.OrderID >> 1 == 0));
        }

        public void TestIntBitwiseAnd()
        {
            TestQuery(db.Orders.Where(o => (o.OrderID & 1) == 0));
        }

        public void TestIntBitwiseOr()
        {
            TestQuery(db.Orders.Where(o => (o.OrderID | 1) == 1));
        }

        public void TestIntBitwiseExclusiveOr()
        {
            TestQuery(db.Orders.Where(o => (o.OrderID ^ 1) == 1));
        }

        public void TestIntBitwiseNot()
        {
            TestQuery(db.Orders.Where(o => ~o.OrderID == 0));
        }

        public void TestIntNegate()
        {
            TestQuery(db.Orders.Where(o => -o.OrderID == -1));
        }

        public void TestAnd()
        {
            TestQuery(db.Orders.Where(o => o.OrderID > 0 && o.OrderID < 2000));
        }

        public void TestOr()
        {
            TestQuery(db.Orders.Where(o => o.OrderID < 5 || o.OrderID > 10));
        }

        public void TestNot()
        {
            TestQuery(db.Orders.Where(o => !(o.OrderID == 0)));
        }

        public void TestEqualNull()
        {
            TestQuery(db.Customers.Where(c => c.City == null));
        }

        public void TestEqualNullReverse()
        {
            TestQuery(db.Customers.Where(c => null == c.City));
        }

        public void TestConditional()
        {
            TestQuery(db.Orders.Where(o => (o.CustomerID == "ALFKI" ? 1000 : 0) == 1000));
        }

        public void TestConditional2()
        {
            TestQuery(db.Orders.Where(o => (o.CustomerID == "ALFKI" ? 1000 : o.CustomerID == "ABCDE" ? 2000 : 0) == 1000));
        }

        public void TestConditionalTestIsValue()
        {
            TestQuery(db.Orders.Where(o => (((bool)(object)o.OrderID) ? 100 : 200) == 100));
        }

        public void TestConditionalResultsArePredicates()
        {
            TestQuery(db.Orders.Where(o => (o.CustomerID == "ALFKI" ? o.OrderID < 10 : o.OrderID > 10)));
        }

        public void TestSelectManyJoined()
        {
            TestQuery(
                from c in db.Customers
                from o in db.Orders.Where(o => o.CustomerID == c.CustomerID)
                select new { c.ContactName, o.OrderDate }
                );
        }

        public void TestSelectManyJoinedDefaultIfEmpty()
        {
            TestQuery(
                from c in db.Customers
                from o in db.Orders.Where(o => o.CustomerID == c.CustomerID).DefaultIfEmpty()
                select new { c.ContactName, o.OrderDate }
                );
        }

        public void TestSelectWhereAssociation()
        {
            TestQuery(
                from o in db.Orders
                where o.Customer.City == "Seattle"
                select o
                );
        }

        public void TestSelectWhereAssociations()
        {
            TestQuery(
                from o in db.Orders
                where o.Customer.City == "Seattle" && o.Customer.Phone != "555 555 5555"
                select o
                );
        }

        public void TestSelectWhereAssociationTwice()
        {
            TestQuery(
                from o in db.Orders
                where o.Customer.City == "Seattle" && o.Customer.Phone != "555 555 5555"
                select o
                );
        }

        public void TestSelectAssociation()
        {
            TestQuery(
                from o in db.Orders
                select o.Customer
                );
        }

        public void TestSelectAssociations()
        {
            TestQuery(
                from o in db.Orders
                select new { A = o.Customer, B = o.Customer }
                );
        }

        public void TestSelectAssociationsWhereAssociations()
        {
            TestQuery(
                from o in db.Orders
                where o.Customer.City == "Seattle"
                where o.Customer.Phone != "555 555 5555"
                select new { A = o.Customer, B = o.Customer }
                );
        }

        public void TestCustomersIncludeOrders()
        {
            var policy = new EntityPolicy();
            policy.IncludeWith<Customer>(c => c.Orders);
            Northwind nw = new Northwind(this.provider.New(policy));

            TestQuery(
                nw.Customers
                );
        }

        public void TestCustomersIncludeOrdersDeferred()
        {
            var policy = new EntityPolicy();
            policy.IncludeWith<Customer>(c => c.Orders, true);
            Northwind nw = new Northwind(this.provider.New(policy));

            TestQuery(
                nw.Customers
                );
        }

        public void TestCustomersIncludeOrdersViaConstructorOnly()
        {
            var mapping = new AttributeMapping(typeof(NorthwindX));
            var policy = new EntityPolicy();
            policy.IncludeWith<CustomerX>(c => c.Orders);
            NorthwindX nw = new NorthwindX(this.provider.New(policy).New(mapping));

            TestQuery(
                nw.Customers
                );
        }

        public void TestCustomersWhereIncludeOrders()
        {
            var policy = new EntityPolicy();
            policy.IncludeWith<Customer>(c => c.Orders);
            Northwind nw = new Northwind(this.provider.New(policy));

            TestQuery(
                from c in nw.Customers
                where c.City == "London"
                select c
                );
        }

        public void TestCustomersIncludeOrdersAndDetails()
        {
            var policy = new EntityPolicy();
            policy.IncludeWith<Customer>(c => c.Orders);
            policy.IncludeWith<Order>(o => o.Details);
            Northwind nw = new Northwind(this.provider.New(policy));

            TestQuery(
                nw.Customers
                );
        }

        public void TestCustomersWhereIncludeOrdersAndDetails()
        {
            var policy = new EntityPolicy();
            policy.IncludeWith<Customer>(c => c.Orders);
            policy.IncludeWith<Order>(o => o.Details);
            Northwind nw = new Northwind(this.provider.New(policy));

            TestQuery(
                from c in nw.Customers
                where c.City == "London"
                select c
                );
        }

        public void TestInterfaceElementTypeAsGenericConstraint()
        {
            TestQuery(
                GetById(db.Products, 5)
                );
        }

        private static IQueryable<T> GetById<T>(IQueryable<T> query, int id) where T : IEntity
        {
            return query.Where(x => x.ID == id);
        }

        public void TestXmlMappingSelectCustomers()
        {
            var nw = new Northwind(this.provider.New(XmlMapping.FromXml(File.ReadAllText(@"Northwind.xml"))));

            TestQuery(
                from c in db.Customers
                where c.City == "London"
                select c.ContactName
                );
        }

        public void TestSingletonAssociationWithMemberAccess()
        {
            TestQuery(
                from o in db.Orders
                where o.Customer.City == "Seattle"
                where o.Customer.Phone != "555 555 5555"
                select new { A = o.Customer, B = o.Customer.City }
                );
        }

        public void TestCompareDateTimesWithDifferentNullability()
        {
            TestQuery(
                from o in db.Orders
                where o.OrderDate < DateTime.Today && ((DateTime?)o.OrderDate) < DateTime.Today
                select o
                );
        }

        public void TestContainsWithEmptyLocalList()
        {
            var ids = new string[0];
            TestQuery(
                from c in db.Customers
                where ids.Contains(c.CustomerID)
                select c
                );
        }

        public void TestContainsWithSubQuery()
        {
            var custsInLondon = db.Customers.Where(c => c.City == "London").Select(c => c.CustomerID);

            TestQuery(
                from c in db.Customers
                where custsInLondon.Contains(c.CustomerID)
                select c
                );
        }

        public void TestCombineQueriesDeepNesting()
        {
            var custs = db.Customers.Where(c => c.ContactName.StartsWith("xxx"));
            var ords = db.Orders.Where(o => custs.Any(c => c.CustomerID == o.CustomerID));
            TestQuery(
                db.OrderDetails.Where(d => ords.Any(o => o.OrderID == d.OrderID))
                );
        }

        public void TestLetWithSubquery()
        {
            TestQuery(
                from customer in db.Customers
                let orders =
                    from order in db.Orders
                    where order.CustomerID == customer.CustomerID
                    select order
                select new
                {
                    Customer = customer,
                    OrdersCount = orders.Count(),
                }
                );
        }
    }
}
