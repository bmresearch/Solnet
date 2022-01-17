using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestEmptyPayloadRequest()
        {
            string responseData = File.ReadAllText("Resources/Http/EmptyPayloadResponse.json");
            string requestData = File.ReadAllText("Resources/Http/EmptyPayloadRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ResponseValue<ulong>> result = sut.GetBalance("");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Result);
            Assert.IsTrue(result.WasHttpRequestSuccessful);
            Assert.IsFalse(result.WasRequestSuccessfullyHandled);
            Assert.IsFalse(result.WasSuccessful);
            Assert.AreEqual("Invalid param: WrongSize", result.Reason);
            Assert.AreEqual(-32602, result.ServerErrorCode);
            Assert.IsNotNull(result.RawRpcRequest);
            Assert.IsNotNull(result.RawRpcResponse);
            Assert.AreEqual(requestData, result.RawRpcRequest);
            Assert.AreEqual(responseData, result.RawRpcResponse);
        }

        [TestMethod]
        public void TestBadAddressExceptionRequest()
        {
            string msg = "something bad happenned";
            string responseData = File.ReadAllText("Resources/Http/EmptyPayloadResponse.json");
            string requestData = File.ReadAllText("Resources/Http/EmptyPayloadRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            messageHandlerMock.Protected().Setup(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).Throws(new HttpRequestException(msg));

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient("https://non.existing.adddress.com", null, httpClient);

            RequestResult<ResponseValue<ulong>> result = sut.GetBalance("");
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
            Assert.AreEqual(msg, result.Reason);
            Assert.IsFalse(result.WasHttpRequestSuccessful);
            Assert.IsFalse(result.WasRequestSuccessfullyHandled);
            Assert.IsNotNull(result.RawRpcRequest);
            Assert.IsNull(result.RawRpcResponse);
            Assert.AreEqual(requestData, result.RawRpcRequest);
        }

        [TestMethod]
        public void TestBadAddress2ExceptionRequest()
        {
            string msg = "not found bro";
            string responseData = File.ReadAllText("Resources/Http/EmptyPayloadResponse.json");
            string requestData = File.ReadAllText("Resources/Http/EmptyPayloadRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            messageHandlerMock.Protected().Setup(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).Throws(new HttpRequestException(msg, null, HttpStatusCode.NotFound));

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut =
                new SolanaRpcClient("https://valid.server.but.invalid.endpoint.com", null, httpClient);

            RequestResult<ResponseValue<ulong>> result = sut.GetBalance("");
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
            Assert.AreEqual(msg, result.Reason);
            Assert.IsFalse(result.WasHttpRequestSuccessful);
            Assert.IsFalse(result.WasRequestSuccessfullyHandled);
            Assert.IsNotNull(result.RawRpcRequest);
            Assert.IsNull(result.RawRpcResponse);
            Assert.AreEqual(requestData, result.RawRpcRequest);
        }

        [TestMethod]
        public void TestGetBalance()
        {
            string responseData = File.ReadAllText("Resources/Http/GetBalanceResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetBalanceRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<ulong>> result = sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79274779UL, result.Result.Context.Slot);
            Assert.AreEqual(168855000000UL, result.Result.Value);
            Assert.IsNotNull(result.RawRpcRequest);
            Assert.IsNotNull(result.RawRpcResponse);
            Assert.AreEqual(requestData, result.RawRpcRequest);
            Assert.AreEqual(responseData, result.RawRpcResponse);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBalanceConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/GetBalanceResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetBalanceConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<ulong>> result =
                sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79274779UL, result.Result.Context.Slot);
            Assert.AreEqual(168855000000UL, result.Result.Value);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetClusterNodes()
        {
            string responseData = File.ReadAllText("Resources/Http/GetClusterNodesResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetClusterNodesRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<ClusterNode>> result = sut.GetClusterNodes();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(5, result.Result.Count);
            Assert.AreEqual(3533521759UL, result.Result[0].FeatureSet);
            Assert.AreEqual("216.24.140.155:8001", result.Result[0].Gossip);
            Assert.AreEqual("5D1fNXzvv5NjV1ysLjirC4WY92RNsVH18vjmcszZd8on", result.Result[0].PublicKey);
            Assert.AreEqual("216.24.140.155:8899", result.Result[0].Rpc);
            Assert.AreEqual(18122UL, result.Result[0].ShredVersion);
            Assert.AreEqual("216.24.140.155:8004", result.Result[0].Tpu);
            Assert.AreEqual("1.7.0", result.Result[0].Version);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetEpochInfo()
        {
            string responseData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<EpochInfo> result = sut.GetEpochInfo();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(166598UL, result.Result.AbsoluteSlot);
            Assert.AreEqual(166500UL, result.Result.BlockHeight);
            Assert.AreEqual(27UL, result.Result.Epoch);
            Assert.AreEqual(2790UL, result.Result.SlotIndex);
            Assert.AreEqual(8192UL, result.Result.SlotsInEpoch);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetEpochInfoProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<EpochInfo> result = sut.GetEpochInfo(Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(166598UL, result.Result.AbsoluteSlot);
            Assert.AreEqual(166500UL, result.Result.BlockHeight);
            Assert.AreEqual(27UL, result.Result.Epoch);
            Assert.AreEqual(2790UL, result.Result.SlotIndex);
            Assert.AreEqual(8192UL, result.Result.SlotsInEpoch);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetEpochSchedule()
        {
            string responseData = File.ReadAllText("Resources/Http/Epoch/GetEpochScheduleResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Epoch/GetEpochScheduleRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<EpochScheduleInfo> result = sut.GetEpochSchedule();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(8UL, result.Result.FirstNormalEpoch);
            Assert.AreEqual(8160UL, result.Result.FirstNormalSlot);
            Assert.AreEqual(8192UL, result.Result.LeaderScheduleSlotOffset);
            Assert.AreEqual(8192UL, result.Result.SlotsPerEpoch);
            Assert.AreEqual(true, result.Result.Warmup);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetGenesisHash()
        {
            string responseData = File.ReadAllText("Resources/Http/GetGenesisHashResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetGenesisHashRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<string> result = sut.GetGenesisHash();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("4uhcVJyU9pJkvQyS88uRDiswHXSCkY3zQawwpjk2NsNY", result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetIdentity()
        {
            string responseData = File.ReadAllText("Resources/Http/GetIdentityResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetIdentityRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<NodeIdentity> result = sut.GetIdentity();
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("2r1F4iWqVcb8M1DbAjQuFpebkQHY9hcVU4WuW2DJBppN", result.Result.Identity);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetMaxRetransmitSlot()
        {
            string responseData = File.ReadAllText("Resources/Http/GetMaxRetransmitSlotResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetMaxRetransmitSlotRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetMaxRetransmitSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetShredInsertSlot()
        {
            string responseData = File.ReadAllText("Resources/Http/GetMaxShredInsertSlotResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetMaxShredInsertSlotRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetMaxShredInsertSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeadersEmpty()
        {
            string responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersEmptyResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersEmptyRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<string>> result = sut.GetSlotLeaders(0, 0);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeaders()
        {
            string responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<string>> result = sut.GetSlotLeaders(100, 10);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(10, result.Result.Count);
            Assert.AreEqual("ChorusmmK7i1AxXeiTtQgQZhQNiXYU84ULeaYF1EH15n", result.Result[0]);
            Assert.AreEqual("Awes4Tr6TX8JDzEhCZY2QVNimT6iD1zWHzf1vNyGvpLM", result.Result[4]);
            Assert.AreEqual("DWvDTSh3qfn88UoQTEKRV2JnLt5jtJAVoiCo3ivtMwXP", result.Result[8]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeader()
        {
            string responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<string> result = sut.GetSlotLeader();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("ENvAW7JScgYq6o4zKZwewtkzzJgDzuJAFxYasvmEQdpS", result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeaderConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<string> result = sut.GetSlotLeader(Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("ENvAW7JScgYq6o4zKZwewtkzzJgDzuJAFxYasvmEQdpS", result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlot()
        {
            string responseData = File.ReadAllText("Resources/Http/Slot/GetSlotResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Slot/GetSlotRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/Slot/GetSlotResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Slot/GetSlotProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetSlot(Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetRecentPerformanceSamples()
        {
            string responseData = File.ReadAllText("Resources/Http/GetRecentPerformanceSamplesResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetRecentPerformanceSamplesRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<PerformanceSample>> result = sut.GetRecentPerformanceSamples(4);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(4, result.Result.Count);
            Assert.AreEqual(126UL, result.Result[0].NumSlots);
            Assert.AreEqual(348125UL, result.Result[0].Slot);
            Assert.AreEqual(126UL, result.Result[0].NumTransactions);
            Assert.AreEqual(60, result.Result[0].SamplePeriodSecs);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSnapshotSlot()
        {
            string responseData = File.ReadAllText("Resources/Http/GetSnapshotSlotResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetSnapshotSlotRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetSnapshotSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(100UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSupply()
        {
            string responseData = File.ReadAllText("Resources/Http/GetSupplyResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetSupplyRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<Supply>> result = sut.GetSupply();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79266564UL, result.Result.Context.Slot);
            Assert.AreEqual(1359481823340465122UL, result.Result.Value.Circulating);
            Assert.AreEqual(122260000000UL, result.Result.Value.NonCirculating);
            Assert.AreEqual(16, result.Result.Value.NonCirculatingAccounts.Count);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSupplyProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/GetSupplyResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetSupplyProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<Supply>> result = sut.GetSupply(Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79266564UL, result.Result.Context.Slot);
            Assert.AreEqual(1359481823340465122UL, result.Result.Value.Circulating);
            Assert.AreEqual(122260000000UL, result.Result.Value.NonCirculating);
            Assert.AreEqual(16, result.Result.Value.NonCirculatingAccounts.Count);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetStakeActivation()
        {
            string responseData = File.ReadAllText("Resources/Http/GetStakeActivationResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetStakeActivationRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<StakeActivationInfo> result =
                sut.GetStakeActivation("CYRJWqiSjLitBAcRxPvWpgX3s5TvmN2SuRY3eEYypFvT");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(197717120UL, result.Result.Active);
            Assert.AreEqual(0UL, result.Result.Inactive);
            Assert.AreEqual("active", result.Result.State);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetStakeActivationWithEpoch()
        {
            string responseData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<StakeActivationInfo> result =
                sut.GetStakeActivation("CYRJWqiSjLitBAcRxPvWpgX3s5TvmN2SuRY3eEYypFvT", 4);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(124429280UL, result.Result.Active);
            Assert.AreEqual(73287840UL, result.Result.Inactive);
            Assert.AreEqual("activating", result.Result.State);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetStakeActivationWithEpochProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<StakeActivationInfo> result =
                sut.GetStakeActivation("CYRJWqiSjLitBAcRxPvWpgX3s5TvmN2SuRY3eEYypFvT", 4, Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(124429280UL, result.Result.Active);
            Assert.AreEqual(73287840UL, result.Result.Inactive);
            Assert.AreEqual("activating", result.Result.State);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetMinimumLedgerSlot()
        {
            string responseData = File.ReadAllText("Resources/Http/GetMinimumLedgerSlotResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetMinimumLedgerSlotRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetMinimumLedgerSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(78969229UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetVersion()
        {
            string responseData = File.ReadAllText("Resources/Http/GetVersionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetVersionRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<NodeVersion> result = sut.GetVersion();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1082270801UL, result.Result.FeatureSet);
            Assert.AreEqual("1.6.11", result.Result.SolanaCore);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetHealth_HealthyResponse()
        {
            string responseData = File.ReadAllText("Resources/Http/Health/GetHealthHealthyResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Health/GetHealthRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<string> res = sut.GetHealth();

            Assert.AreEqual(requestData, sentMessage);
            Assert.AreEqual("ok", res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetHealth_UnhealthyResponse()
        {
            string responseData = File.ReadAllText("Resources/Http/Health/GetHealthUnhealthyResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Health/GetHealthRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<string> res = sut.GetHealth();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNull(res.Result);
            Assert.IsTrue(res.WasHttpRequestSuccessful);
            Assert.IsFalse(res.WasRequestSuccessfullyHandled);
            Assert.AreEqual(-32005, res.ServerErrorCode);
            Assert.AreEqual("Node is behind by 42 slots", res.Reason);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetMinimumBalanceForRentExemption()
        {
            string responseData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ulong> res = sut.GetMinimumBalanceForRentExemption(50);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(500UL, res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetMinimumBalanceForRentExemptionConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ulong> res = sut.GetMinimumBalanceForRentExemption(50, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(500UL, res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestRequestAirdrop()
        {
            string responseData = File.ReadAllText("Resources/Http/RequestAirdropResult.json");
            string requestData = File.ReadAllText("Resources/Http/RequestAirdropRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<string> res =
                sut.RequestAirdrop("6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z", 100000000000);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual("2iyRQZmksTfkmyH9Fnho61x4Y7TeSN8g3GRZCHmQjzzFB1e3DwKEVrYfR7AnKjiE5LiDEfCowtzoE2Pau646g1Vf",
                res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        /// <summary>
        ///     Backstory - public RPC nodes no longer support GetProgramAccount requests for the Serum program-id
        ///     These are dealt with a HTTP 410 "GONE" response.
        ///     Make sure that the JsonRpc gives an practical response under these circumstances.
        /// </summary>
        [TestMethod]
        public void TestFailHttp410Gone()
        {
            string responseData = File.ReadAllText("Resources/Http/GetProgramAccountsResponse-Fail-410.json");
            string requestData = File.ReadAllText("Resources/Http/GetProgramAccountsRequest-Fail-410.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData, HttpStatusCode.Gone);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            List<MemCmp> filters = new()
            {
                new MemCmp {Offset = 45, Bytes = "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5"}
            };

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<AccountKeyPair>> result = sut.GetProgramAccounts(
                "9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin", // serum program-id
                dataSize: 3228, memCmpList: filters);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Gone, result.HttpStatusCode);
            Assert.AreEqual(responseData, result.RawRpcResponse);
            Assert.IsFalse(result.WasSuccessful);
            Assert.IsNull(result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        /// <summary>
        ///     Backstory - when hopping from public RPC node to Serum RPC node there was a difference in their configuration
        ///     Solnet was sending Content-Type header of `application/json; charset=utf-8` which is rejected by Serum RPC cluster.
        ///     This was dealt with by a HTTP 415 response which was not accessible in the Solnet Result.
        /// </summary>
        [TestMethod]
        public void TestFailHttp415UnsupportedMediaType()
        {
            string responseData = "Supplied content type is not allowed. Content-Type: application/json is required";
            string requestData = File.ReadAllText("Resources/Http/GetBalanceRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData, HttpStatusCode.UnsupportedMediaType);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<ulong>> result = sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, result.HttpStatusCode);
            Assert.AreEqual(responseData, result.RawRpcResponse);
            Assert.IsFalse(result.WasSuccessful);
            Assert.IsNull(result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}