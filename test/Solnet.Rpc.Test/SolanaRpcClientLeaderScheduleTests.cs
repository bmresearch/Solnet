using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientLeaderScheduleTest : SolanaRpcClientTestBase
    {

        [TestMethod]
        public void TestGetLeaderSchedule_SlotArgsRequest()
        {

            var responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleSlotArgsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetLeaderSchedule(79700000);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.WasSuccessful);

            Assert.AreEqual(2, res.Result.Count);
            Assert.IsTrue(res.Result.ContainsKey("4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"));

            Assert.AreEqual(7, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"].Count);
            Assert.AreEqual(0UL, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"][0]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetLeaderSchedule_IdentityArgsRequest()
        {

            var responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleIdentityArgsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetLeaderSchedule(identity: "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.WasSuccessful);

            Assert.AreEqual(2, res.Result.Count);
            Assert.IsTrue(res.Result.ContainsKey("4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"));

            Assert.AreEqual(7, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"].Count);
            Assert.AreEqual(0UL, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"][0]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetLeaderSchedule_SlotIdentityArgsRequest()
        {

            var responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleSlotIdentityArgsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetLeaderSchedule(79700000, "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.WasSuccessful);

            Assert.AreEqual(2, res.Result.Count);
            Assert.IsTrue(res.Result.ContainsKey("4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"));

            Assert.AreEqual(7, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"].Count);
            Assert.AreEqual(0UL, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"][0]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetLeaderSchedule_NoArgsRequest()
        {

            var responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleNoArgsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetLeaderSchedule();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.WasSuccessful);

            Assert.AreEqual(2, res.Result.Count);
            Assert.IsTrue(res.Result.ContainsKey("4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"));

            Assert.AreEqual(7, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"].Count);
            Assert.AreEqual(0UL, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"][0]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetLeaderSchedule_CommitmentFinalizedRequest()
        {

            var responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleNoArgsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetLeaderSchedule(commitment: Types.Commitment.Finalized);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.WasSuccessful);

            Assert.AreEqual(2, res.Result.Count);
            Assert.IsTrue(res.Result.ContainsKey("4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"));

            Assert.AreEqual(7, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"].Count);
            Assert.AreEqual(0UL, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"][0]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetLeaderSchedule_CommitmentProcessedRequest()
        {

            var responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetLeaderSchedule(commitment: Types.Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.WasSuccessful);

            Assert.AreEqual(2, res.Result.Count);
            Assert.IsTrue(res.Result.ContainsKey("4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"));

            Assert.AreEqual(7, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"].Count);
            Assert.AreEqual(0UL, res.Result["4Qkev8aNZcqFNSRhQzwyLMFSsi94jHqE8WNVTJzTP99F"][0]);

            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}