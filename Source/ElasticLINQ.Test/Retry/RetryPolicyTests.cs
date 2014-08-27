// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Retry;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ElasticLinq.Test.Retry
{
    public class RetryPolicyTests
    {
        public interface IFake
        {
            Task<int> DoSomething();
            bool IsRetryable(int result, Exception ex);
        }

        [Fact]
        public static async Task DoesNotRetry()
        {
            var fake = Substitute.For<IFake>();
            fake.DoSomething().ReturnsTask(0);
            fake.IsRetryable(1337, null).Returns(true);
            fake.IsRetryable(0, null).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 10, delay);

            var result = await retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable);

            Assert.Equal(0, result);
            fake.Received(1, x => x.DoSomething());
        }

        [Fact]
        public static async Task Retries_WhenShouldRetry()
        {
            var fake = Substitute.For<IFake>();
            fake.DoSomething().ReturnsTask(1337, 1337, 1337, 0);
            fake.IsRetryable(1337, null).Returns(true);
            fake.IsRetryable(0, null).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 10, delay);

            await retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable);

            fake.Received(4, x => x.DoSomething());
            delay.Received(1, x => x.Received(100));
            delay.Received(1, x => x.Received(200));
            delay.Received(1, x => x.Received(400));

            // Test logging for each retry
            AssertInfoLog(logger, 0, 100, 1);
            AssertInfoLog(logger, 1, 200, 2);
            AssertInfoLog(logger, 2, 400, 3);
        }

        [Fact]
        public static void Throws_WhenExceptionIsNotRetryable()
        {
            var fake = Substitute.For<IFake>();
            var ex = new Exception();
            fake.DoSomething().ThrowsTask(ex);
            fake.IsRetryable(Arg.Any<int>(), ex).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 10, delay);

            var actualEx = Record.Exception(() => retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable).GetAwaiter().GetResult());

            Assert.Equal(ex, actualEx);
        }

        [Fact]
        public static async Task MergesLogInfo()
        {
            var fake = Substitute.For<IFake>();
            fake.DoSomething().ReturnsTask(1337, 0);
            fake.IsRetryable(1337, null).Returns(true);
            fake.IsRetryable(0, null).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 10, delay);

            await retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable,
                (result, loggerInfo) =>
                {
                    loggerInfo["couchbaseDocumentKey"] = "mykey";
                    loggerInfo["result"] = result;
                });

            var fields = AssertInfoLog(logger, 0, 100, 1);
            Assert.Equal("mykey", fields["couchbaseDocumentKey"]);
            Assert.Equal(1337, fields["result"]);
        }

        [Fact]
        public static void Throws_IfRetriesAreExhausted()
        {
            var fake = Substitute.For<IFake>();
            fake.DoSomething().ReturnsTask(1337, 1337);
            fake.IsRetryable(1337, null).Returns(true);
            fake.IsRetryable(0, null).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 2, delay);

            var ex = Assert.Throws<RetryFailedException>(() => retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable).GetAwaiter().GetResult());
            Assert.Equal("The operation did not succeed after the maximum number of retries (2).", ex.Message);
        }

        private static Dictionary<string, object> AssertInfoLog(ILog logger, int callInstance, int operationRetryDelayMs, int operationAttempt)
        {
            var logCapture = logger.Captured(callInstance, x => x.Log(Arg.Any<TraceEventType>(), null, null, null));
            Assert.Equal(TraceEventType.Information, logCapture.Arg<TraceEventType>());
            var fields = logCapture.Arg<Dictionary<string, object>>();
            Assert.Equal("retry", fields["category"]);
            Assert.Equal(operationRetryDelayMs, fields["operationRetryDelayMS"]);
            Assert.Equal(operationAttempt, fields["operationAttempt"]);
            return fields;
        }
    }
}