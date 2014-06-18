using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticLinq.Test.Logging
{
    public class LogExtensionTests
    {
        [Fact]
        public void DebugRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Debug(new Exception("DebugLog"), new Dictionary<string, object> { { "DebugInfo", new Object() } }, "DebugMessage", 1, 2, 3);

            Assert.Equal(1, spy.Messages.Count);
            Assert.Contains("VERBOSE", spy.Messages[0]);
            Assert.Contains("DebugLog", spy.Messages[0]);
            Assert.Contains("DebugInfo", spy.Messages[0]);
            Assert.Contains("DebugMessage", spy.Messages[0]);
        }

        [Fact]
        public void ErrorRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Error(new Exception("ErrorLog"), new Dictionary<string, object> { { "ErrorInfo", new Object() } }, "ErrorMessage", 1, 2, 3);

            Assert.Equal(1, spy.Messages.Count);
            Assert.Contains("ERROR", spy.Messages[0]);
            Assert.Contains("ErrorLog", spy.Messages[0]);
            Assert.Contains("ErrorInfo", spy.Messages[0]);
            Assert.Contains("ErrorMessage", spy.Messages[0]);
        }

        [Fact]
        public void FatalRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Fatal(new Exception("FatalLog"), new Dictionary<string, object> { { "FatalInfo", new Object() } }, "FatalMessage", 1, 2, 3);

            Assert.Equal(1, spy.Messages.Count);
            Assert.Contains("CRITICAL", spy.Messages[0]);
            Assert.Contains("FatalLog", spy.Messages[0]);
            Assert.Contains("FatalInfo", spy.Messages[0]);
            Assert.Contains("FatalMessage", spy.Messages[0]);
        }

        [Fact]
        public void InfoRecordsToLog()
        {
            var spy = new SpyLog();

            spy.Info(new Exception("InfoLog"), new Dictionary<string, object> { { "InfoInfo", new Object() } }, "InfoMessage", 1, 2, 3);

            Assert.Equal(1, spy.Messages.Count);
            Assert.Contains("INFO", spy.Messages[0]);
            Assert.Contains("InfoLog", spy.Messages[0]);
            Assert.Contains("InfoInfo", spy.Messages[0]);
            Assert.Contains("InfoMessage", spy.Messages[0]);
        }
    }
}