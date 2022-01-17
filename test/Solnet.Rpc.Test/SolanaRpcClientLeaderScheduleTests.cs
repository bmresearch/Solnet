using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Types;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientLeaderScheduleTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetLeaderSchedule_SlotArgsRequest()
        {
            string responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleSlotArgsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<Dictionary<string, List<ulong>>> res = sut.GetLeaderSchedule(79700000);

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
            string responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleIdentityArgsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<Dictionary<string, List<ulong>>> res =
                sut.GetLeaderSchedule(identity: "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

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
            string responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleSlotIdentityArgsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<Dictionary<string, List<ulong>>> res =
                sut.GetLeaderSchedule(79700000, "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

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
            string responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            string requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleNoArgsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<Dictionary<string, List<ulong>>> res = sut.GetLeaderSchedule();

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
            string responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            string requestData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleNoArgsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<Dictionary<string, List<ulong>>>
                res = sut.GetLeaderSchedule(commitment: Commitment.Finalized);

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
            string responseData = File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/LeaderSchedule/GetLeaderScheduleProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<Dictionary<string, List<ulong>>>
                res = sut.GetLeaderSchedule(commitment: Commitment.Processed);

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