// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using ElasticLinq.Logging;
using Xunit;

namespace ElasticLinq.Test.Logging
{
    public static class LogExtensionTests
    {
        [Fact]
        public static void DebugRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Debug(new Exception("DebugLog"), new Dictionary<string, object> { { "DebugInfo", new object() } }, "DebugMessage", 1, 2, 3);

            var entry = Assert.Single(spy.Entries);
            Assert.Equal(TraceEventType.Verbose, entry.Type);
            Assert.Contains("DebugLog", entry.Exception.Message);
            Assert.Contains("DebugInfo", entry.AdditionalInfo.Keys);
            Assert.Equal("DebugMessage", entry.Message);
        }

        [Fact]
        public static void ErrorRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Error(new Exception("ErrorLog"), new Dictionary<string, object> { { "ErrorInfo", new object() } }, "ErrorMessage", 1, 2, 3);

            var entry = Assert.Single(spy.Entries);
            Assert.Equal(TraceEventType.Error, entry.Type);
            Assert.Contains("ErrorLog", entry.Exception.Message);
            Assert.Contains("ErrorInfo", entry.AdditionalInfo.Keys);
            Assert.Equal("ErrorMessage", entry.Message);
        }

        [Fact]
        public static void FatalRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Fatal(new Exception("FatalLog"), new Dictionary<string, object> { { "FatalInfo", new object() } }, "FatalMessage", 1, 2, 3);

            var entry = Assert.Single(spy.Entries);
            Assert.Equal(TraceEventType.Critical, entry.Type);
            Assert.Contains("FatalLog", entry.Exception.Message);
            Assert.Contains("FatalInfo", entry.AdditionalInfo.Keys);
            Assert.Equal("FatalMessage", entry.Message);
        }

        [Fact]
        public static void InfoRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Info(new Exception("InfoLog"), new Dictionary<string, object> { { "InfoInfo", new object() } }, "InfoMessage", 1, 2, 3);

            var entry = Assert.Single(spy.Entries);
            Assert.Equal(TraceEventType.Information, entry.Type);
            Assert.Contains("InfoLog", entry.Exception.Message);
            Assert.Contains("InfoInfo", entry.AdditionalInfo.Keys);
            Assert.Equal("InfoMessage", entry.Message);
        }
    }
}