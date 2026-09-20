using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcRateLimitingTests
    {
        [TestMethod]
        public async Task TestMaxSpeed_NoLimits()
        {
            // allow unlimited fires instantly
            var limit = RateLimiter.Create();
            Assert.IsTrue(limit.CanFire());
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
        }

        [TestMethod]
        public async Task TestMaxSpeed_WithinLimits()
        {
            // allow unlimited fires instantly
            var limit = RateLimiter.Create().AllowHits(100).PerSeconds(10);
            Assert.IsTrue(limit.CanFire());
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
        }

        [TestMethod]
        public async Task TestTwoHitsPerSecond()
        {
            // allow 2 hits per second
            var timeCheck = DateTime.UtcNow;
            var limit = RateLimiter.Create().AllowHits(2).PerSeconds(1);
            var twoSecondsLater = timeCheck.AddSeconds(2);
            Console.WriteLine(limit);
            Assert.IsTrue(limit.CanFire());
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);

            // observe why this may break the build
            var finalTimeCheck = DateTime.UtcNow;
            Console.WriteLine($" ExecTime diff {finalTimeCheck.Subtract(timeCheck).TotalMilliseconds}ms");
            Console.WriteLine($"TimeCheck diff {finalTimeCheck.Subtract(twoSecondsLater).TotalMilliseconds}ms");
            Assert.IsGreaterThan(value: finalTimeCheck.Subtract(timeCheck).TotalMilliseconds, lowerBound: 2000, message: $"ExecTime diff {finalTimeCheck.Subtract(timeCheck).TotalMilliseconds}ms");
        }

        [TestMethod]
        public async Task TestMaxSpeed_NoLimits_Async()
        {
            // allow unlimited fires instantly
            var limit = RateLimiter.Create();
            Assert.IsTrue(limit.CanFire());
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
        }

        [TestMethod]
        public async Task TestMaxSpeed_WithinLimits_Async()
        {
            // allow unlimited fires instantly
            var limit = RateLimiter.Create().AllowHits(100).PerSeconds(10);
            Assert.IsTrue(limit.CanFire());
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            await limit.WaitFireAsync(TestContext.CancellationToken);
        }

        [TestMethod]
        public async Task TestTwoHitsPerSecond_Async()
        {
            // allow 2 hits per second
            var timeCheck = DateTime.UtcNow;
            var limit = RateLimiter.Create().AllowHits(2).PerSeconds(1);
            var twoSecondsLater = timeCheck.AddSeconds(2);
            Console.WriteLine(limit);
            Assert.IsTrue(limit.CanFire());
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);
            await limit.WaitFireAsync(TestContext.CancellationToken);
            Console.WriteLine(limit);

            // observe why this may break the build
            var finalTimeCheck = DateTime.UtcNow;
            Console.WriteLine($" ExecTime diff {finalTimeCheck.Subtract(timeCheck).TotalMilliseconds}ms");
            Console.WriteLine($"TimeCheck diff {finalTimeCheck.Subtract(twoSecondsLater).TotalMilliseconds}ms");
            Assert.IsGreaterThan(value: finalTimeCheck.Subtract(timeCheck).TotalMilliseconds, lowerBound: 2000, message: $"ExecTime diff {finalTimeCheck.Subtract(timeCheck).TotalMilliseconds}ms");
        }

        public TestContext TestContext { get; set; }
    }

}
