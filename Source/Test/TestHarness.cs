// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Test
{
    using IQToolkit.Data;
    using IQToolkit.Data.Common;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class ExcludeProvider : Attribute
    {
        public string Provider { get; set; }

        public ExcludeProvider(string provider)
        {
            this.Provider = provider;
        }
    }

    public class TestHarness
    {
        protected class TestFailureException : Exception
        {
            internal TestFailureException(string message)
                : base(message)
            {
            }
        }

        private delegate void TestMethod();

        protected DbEntityProvider provider;
        XmlTextWriter baselineWriter;
        Dictionary<string, string> baselines;
        bool executeQueries;
        protected MethodInfo currentMethod;

        public static bool WriteOutput = true;

        protected TestHarness()
        {
        }

        protected void RunTest(DbEntityProvider provider, string baselineFile, bool executeQueries, string testName)
        {
            this.RunTests(provider, baselineFile, null, executeQueries,
                new MethodInfo[] { this.GetType().GetMethod(testName) }
                );
        }

        protected void RunTests(DbEntityProvider provider, string baselineFile, string newBaselineFile, bool executeQueries)
        {
            this.RunTests(provider, baselineFile, newBaselineFile, executeQueries, this.GetType().GetMethods().Where(m => m.Name.StartsWith("Test")).ToArray());
        }

        class TestFailure
        {
            internal string TestName;
            internal string Reason;
        }

        protected void RunTests(DbEntityProvider provider, string baselineFile, string newBaselineFile, bool executeQueries, MethodInfo[] tests)
        {
            this.provider = provider;
            this.executeQueries = executeQueries;

            ReadBaselines(baselineFile);

            if (!string.IsNullOrEmpty(newBaselineFile))
            {
                baselineWriter = new XmlTextWriter(newBaselineFile, Encoding.UTF8);
                baselineWriter.Formatting = Formatting.Indented;
                baselineWriter.Indentation = 2;
                baselineWriter.WriteStartDocument();
                baselineWriter.WriteStartElement("baselines");
            }

            int iTest = 0;
            int iPassed = 0;
            var failures = new List<TestFailure>();
            ConsoleColor originalColor = Console.ForegroundColor;
            if (WriteOutput)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Running tests: {0}", this.GetType().Name);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            try
            {
                foreach (MethodInfo method in tests.Where(m => m != null && TestIsEnabled(m)))
                {
                    iTest++;
                    currentMethod = method;
                    string testName = method.Name.Substring(4);
                    bool passed = false;
                    if (WriteOutput)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    SetupTest();
                    string reason = "";
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        TestMethod test = (TestMethod)Delegate.CreateDelegate(typeof(TestMethod), this, method);
                        test();
                        if (testName.EndsWith("Fails"))
                        {
                            passed = false;
                            reason = "Expected failure";
                        }
                        else
                        {
                            passed = true;
                            iPassed++;
                        }
                    }
                    catch (Exception tf)//(TestFailureException tf)
                    {
                        if (testName.EndsWith("Fails"))
                        {
                            passed = true;
                            iPassed++;
                        }
                        else if (tf.Message != null)
                        {
                            reason = tf.Message;
                        }
                    }
                    finally
                    {
                        TeardownTest();
                    }

                    if (!passed)
                    {
                        failures.Add(new TestFailure { TestName = method.Name, Reason = reason });
                    }

                    if (WriteOutput)
                    {
                        Console.ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.WriteLine("Test {0}: {1} - {2}", iTest, method.Name, passed ? "PASSED" : "FAILED");
                        if (!passed && !string.IsNullOrEmpty(reason))
                            Console.WriteLine("Reason: {0}", reason);
                    }
                }
            }
            finally
            {
                if (baselineWriter != null)
                {
                    baselineWriter.WriteEndElement();
                    baselineWriter.Close();
                }
            }

            timer.Stop();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("SUMMARY: {0}", this.GetType().Name);
            Console.WriteLine("Total tests run: {0}", iTest);
            Console.WriteLine("Total elapsed time: {0}", timer.Elapsed);

            Console.ForegroundColor = ConsoleColor.Green;
            if (iPassed == iTest)
            {
                Console.WriteLine("ALL tests passed!");
            }
            else
            {
                Console.WriteLine("Total tests passed: {0}", iPassed);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Total tests failed: {0}", iTest - iPassed);
                foreach (var failure in failures)
                {
                    Console.WriteLine("  {0}: {1}", failure.TestName, failure.Reason != null ? failure.Reason : string.Empty);
                }
            }
            Console.ForegroundColor = originalColor;
            Console.WriteLine();
        }

        private bool TestIsEnabled(MethodInfo test)
        {
            ExcludeProvider[] exclusions = (ExcludeProvider[])test.GetCustomAttributes(typeof(ExcludeProvider), true);
            foreach (var exclude in exclusions)
            {
                if (
                    // actual name of the provider type
                    string.Compare(this.provider.GetType().Name, exclude.Provider, true) == 0
                    // prefix of the provider type xxxQueryProvider
                    || string.Compare(this.provider.GetType().Name, exclude.Provider + "QueryProvider", true) == 0
                    // last name of the namespace
                    || string.Compare(this.provider.GetType().Namespace.Split(new[] { '.' }).Last(), exclude.Provider, true) == 0
                    )
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void SetupTest()
        {
        }

        protected virtual void TeardownTest()
        {
        }

        protected virtual void SetupSuite()
        {
        }

        protected virtual void TeardownSuite()
        {
        }

        private void WriteBaseline(string key, string text)
        {
            if (baselineWriter != null)
            {
                baselineWriter.WriteStartElement("baseline");
                baselineWriter.WriteAttributeString("key", key);
                baselineWriter.WriteWhitespace("\r\n");
                baselineWriter.WriteString(text);
                baselineWriter.WriteEndElement();
            }
        }

        private void ReadBaselines(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                XDocument doc = XDocument.Load(filename);
                this.baselines = doc.Root.Elements("baseline").ToDictionary(e => (string)e.Attribute("key"), e => e.Value);
            }
        }

        protected double RunTimedTest(int iterations, Action<int> action)
        {
            action(0); // throw out the first one  (makes sure code is loaded)

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            for (int i = 1; i <= iterations; i++)
            {
                action(i);
            }
            timer.Stop();
            return timer.Elapsed.TotalSeconds / iterations;
        }

        protected void TestQuery(IQueryable query)
        {
            TestQuery((EntityProvider)query.Provider, query.Expression, currentMethod.Name, false);
        }

        protected void TestQuery(IQueryable query, string baselineKey)
        {
            TestQuery((EntityProvider)query.Provider, query.Expression, baselineKey, false);
        }

        protected void TestQuery(Expression<Func<object>> query)
        {
            TestQuery(this.provider, query.Body, currentMethod.Name, false);
        }

        protected void TestQuery(Expression<Func<object>> query, string baselineKey)
        {
            TestQuery(this.provider, query.Body, baselineKey, false);
        }

        protected void TestQueryFails(IQueryable query)
        {
            TestQuery((EntityProvider)query.Provider, query.Expression, currentMethod.Name, true);
        }

        protected void TestQueryFails(Expression<Func<object>> query)
        {
            TestQuery(this.provider, query.Body, currentMethod.Name, true);
        }

        protected void TestQuery(EntityProvider pro, Expression query, string baselineKey, bool expectedToFail)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            try
            {
                if (query.NodeType == ExpressionType.Convert && query.Type == typeof(object))
                {
                    query = ((UnaryExpression)query).Operand; // remove box
                }

                if (pro.Log != null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    DbExpressionWriter.Write(pro.Log, pro.Language, query);
                    pro.Log.WriteLine();
                    pro.Log.WriteLine("==>");
                }

                string queryText = null;
                try
                {
                    queryText = pro.GetQueryText(query);
                    WriteBaseline(baselineKey, queryText);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("Query translation failed for {0}", baselineKey));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(query.ToString());
                    throw new TestFailureException(e.Message);
                }

                if (this.executeQueries)
                {
                    Exception caught = null;
                    try
                    {
                        object result = pro.Execute(query);
                        IEnumerable seq = result as IEnumerable;
                        if (seq != null)
                        {
                            // iterate results
                            foreach (var item in seq)
                            {
                            }
                        }
                        else
                        {
                            IDisposable disposable = result as IDisposable;
                            if (disposable != null)
                                disposable.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        caught = e;
                        if (!expectedToFail)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Query failed to execute:");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(queryText);
                            throw new TestFailureException(e.Message);
                        }
                    }
                    if (caught == null && expectedToFail)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Query succeeded when expected to fail");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(queryText);
                        throw new TestFailureException(null);
                    }
                }
                else if (pro.Log != null)
                {
                    var text = pro.GetQueryText(query);
                    pro.Log.WriteLine(text);
                    pro.Log.WriteLine();
                }

                string baseline = null;
                if (this.baselines != null && this.baselines.TryGetValue(baselineKey, out baseline))
                {
                    string trimAct = TrimExtraWhiteSpace(queryText).Trim();
                    string trimBase = TrimExtraWhiteSpace(baseline).Trim();
                    if (trimAct != trimBase)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Query translation does not match baseline:");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(queryText);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("---- current ----");
                        WriteDifferences(trimAct, trimBase);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("---- baseline ----");
                        WriteDifferences(trimBase, trimAct);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        throw new TestFailureException("Translation differed from baseline.");
                    }
                }

                if (baseline == null && this.baselines != null)
                {
                    throw new TestFailureException("No baseline");
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        private string TrimExtraWhiteSpace(string s)
        {
            StringBuilder sb = new StringBuilder();
            bool lastWasWhiteSpace = false;
            foreach (char c in s)
            {
                bool isWS = char.IsWhiteSpace(c);
                if (!isWS || !lastWasWhiteSpace)
                {
                    if (isWS)
                        sb.Append(' ');
                    else
                        sb.Append(c);
                    lastWasWhiteSpace = isWS;
                }
            }
            return sb.ToString();
        }

        private void WriteDifferences(string s1, string s2)
        {
            int start = 0;
            bool same = true;
            for (int i = 0, n = Math.Min(s1.Length, s2.Length); i < n; i++)
            {
                bool matches = s1[i] == s2[i];
                if (matches != same)
                {
                    if (i > start)
                    {
                        Console.ForegroundColor = same ? ConsoleColor.Gray : ConsoleColor.White;
                        Console.Write(s1.Substring(start, i - start));
                    }
                    start = i;
                    same = matches;
                }
            }
            if (start < s1.Length)
            {
                Console.ForegroundColor = same ? ConsoleColor.Gray : ConsoleColor.White;
                Console.Write(s1.Substring(start));
            }
            Console.WriteLine();
        }

        protected void Assert(bool truth, string message)
        {
            if (!truth)
            {
                throw new TestFailureException(message);
            }
        }

        protected void AssertValue(object expected, object actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new TestFailureException(string.Format("Assert failure - expected: {0} actual: {1}", expected, actual));
            }
        }

        protected void AssertValue(double expected, double actual, double epsilon)
        {
            if (!(actual >= expected - epsilon && actual <= expected + epsilon))
            {
                throw new TestFailureException(string.Format("Assert failure - expected: {0} +/- {1} actual: {1}", expected, epsilon, actual));
            }
        }

        protected void AssertNotValue(object notExpected, object actual)
        {
            if (object.Equals(notExpected, actual))
            {
                throw new TestFailureException(string.Format("Assert failure - value not expected: {0}", actual));
            }
        }

        protected void AssertTrue(bool value)
        {
            this.AssertValue(true, value);
        }

        protected void AssertFalse(bool value)
        {
            this.AssertValue(false, value);
        }

        protected bool ExecSilent(string commandText)
        {
            try
            {
                this.provider.ExecuteCommand(commandText);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}