// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;
using System.Threading;
using ElasticLinq.Logging;
using ElasticLinq.Retry;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TraceEventType = ElasticLinq.Logging.TraceEventType;

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
        public static async void Throws_WhenExceptionIsNotRetryable()
        {
            var fake = Substitute.For<IFake>();
            var ex = new Exception();
            fake.DoSomething().ThrowsTask(ex);
            fake.IsRetryable(Arg.Any<int>(), ex).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 10, delay);

            var actualEx = await Record.ExceptionAsync(() => retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable));

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
        public static async void NullDelayDoesNotDelay()
        {
            var delay = new NullDelay();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            await delay.For((int)TimeSpan.FromSeconds(10).TotalMilliseconds, CancellationToken.None);
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }

        [Fact]
        public static async void DelayDoesDelay()
        {
            const int timingFudge = 10; // Task.Delay sometimes returns a milli too soon...
            var delayTime = TimeSpan.FromSeconds(2);
            var delay = new Delay();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            await delay.For((int)delayTime.TotalMilliseconds, CancellationToken.None);
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds + timingFudge >= delayTime.TotalMilliseconds, string.Format("Requested {0}ms delay but only took {1}ms", delayTime.TotalMilliseconds, stopwatch.ElapsedMilliseconds));
        }

        [Fact]
        public static void DelayCanBeCancelled()
        {
            var cts = new CancellationTokenSource();

            var task = new Delay().For(2000, cts.Token);
            cts.Cancel();
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public static async void Throws_IfRetriesAreExhausted()
        {
            var fake = Substitute.For<IFake>();
            fake.DoSomething().ReturnsTask(1337, 1337);
            fake.IsRetryable(1337, null).Returns(true);
            fake.IsRetryable(0, null).Returns(false);
            var logger = Substitute.For<ILog>();
            var delay = Substitute.For<Delay>();
            var retryHandler = new RetryPolicy(logger, 100, 2, delay);

            var ex = await Assert.ThrowsAsync<RetryFailedException>(() => retryHandler.ExecuteAsync(fake.DoSomething, fake.IsRetryable));
            Assert.Equal("The operation did not succeed after the maximum number of retries (2).", ex.Message);
        }

        static Dictionary<string, object> AssertInfoLog(ILog logger, int callInstance, int operationRetryDelayMs, int operationAttempt)
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