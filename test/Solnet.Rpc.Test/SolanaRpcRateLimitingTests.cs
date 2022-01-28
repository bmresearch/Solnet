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
        public void TestMaxSpeed_NoLimits()
        {
            // allow unlimited fires instantly
            var limit = RateLimiter.Create();
            Assert.IsTrue(limit.CanFire());
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
        }
        [TestMethod]
        public void TestMaxSpeed_WithinLimits()
        {
            // allow unlimited fires instantly
            var limit = RateLimiter.Create().AllowHits(100).PerSeconds(10);
            Assert.IsTrue(limit.CanFire());
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
        }

        [TestMethod]
        public void TestTwoHitsPerSecond()
        {
            // allow 2 hits per second
            var timeCheck = DateTime.UtcNow;
            var limit = RateLimiter.Create().AllowHits(2).PerSeconds(1);
            var twoSecondsLater = timeCheck.AddSeconds(2);
            Assert.IsTrue(limit.CanFire());
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();
            limit.Fire();

            // observe why this may break the build
            var finalTimeCheck= DateTime.UtcNow;
            Assert.IsTrue(finalTimeCheck >= twoSecondsLater, $"TimeCheck diff {twoSecondsLater.Subtract(finalTimeCheck).TotalMilliseconds}");
        }

    }

}
