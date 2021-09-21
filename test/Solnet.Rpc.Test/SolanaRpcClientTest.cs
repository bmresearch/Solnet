using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestEmptyPayloadRequest()
        {
            var responseData = File.ReadAllText("Resources/Http/EmptyPayloadResponse.json");
            var requestData = File.ReadAllText("Resources/Http/EmptyPayloadRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetBalance("");

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
            var msg = "something bad happenned";
            var responseData = File.ReadAllText("Resources/Http/EmptyPayloadResponse.json");
            var requestData = File.ReadAllText("Resources/Http/EmptyPayloadRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            messageHandlerMock.Protected().Setup(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).Throws(new HttpRequestException(msg));

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient("https://non.existing.adddress.com", null, httpClient);

            var result = sut.GetBalance("");
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
            var msg = "not found bro";
            var responseData = File.ReadAllText("Resources/Http/EmptyPayloadResponse.json");
            var requestData = File.ReadAllText("Resources/Http/EmptyPayloadRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            messageHandlerMock.Protected().Setup(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).Throws(new HttpRequestException(msg, null, HttpStatusCode.NotFound));

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient("https://valid.server.but.invalid.endpoint.com", null, httpClient);

            var result = sut.GetBalance("");
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
            var responseData = File.ReadAllText("Resources/Http/GetBalanceResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetBalanceRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
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
            var responseData = File.ReadAllText("Resources/Http/GetBalanceResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetBalanceConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", Types.Commitment.Confirmed);

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
            var responseData = File.ReadAllText("Resources/Http/GetClusterNodesResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetClusterNodesRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetClusterNodes();

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
            var responseData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetEpochInfo();

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
            var responseData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Epoch/GetEpochInfoProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetEpochInfo(Types.Commitment.Processed);

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
            var responseData = File.ReadAllText("Resources/Http/Epoch/GetEpochScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Epoch/GetEpochScheduleRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetEpochSchedule();

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
            var responseData = File.ReadAllText("Resources/Http/GetGenesisHashResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetGenesisHashRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetGenesisHash();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("4uhcVJyU9pJkvQyS88uRDiswHXSCkY3zQawwpjk2NsNY", result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetIdentity()
        {
            var responseData = File.ReadAllText("Resources/Http/GetIdentityResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetIdentityRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetIdentity();
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("2r1F4iWqVcb8M1DbAjQuFpebkQHY9hcVU4WuW2DJBppN", result.Result.Identity);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetMaxRetransmitSlot()
        {
            var responseData = File.ReadAllText("Resources/Http/GetMaxRetransmitSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetMaxRetransmitSlotRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetMaxRetransmitSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetShredInsertSlot()
        {
            var responseData = File.ReadAllText("Resources/Http/GetMaxShredInsertSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetMaxShredInsertSlotRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetMaxShredInsertSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeadersEmpty()
        {
            var responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersEmptyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersEmptyRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSlotLeaders(0, 0);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeaders()
        {
            var responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeadersRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSlotLeaders(100, 10);

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
            var responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSlotLeader();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("ENvAW7JScgYq6o4zKZwewtkzzJgDzuJAFxYasvmEQdpS", result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotLeaderConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Slot/GetSlotLeaderConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSlotLeader(Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("ENvAW7JScgYq6o4zKZwewtkzzJgDzuJAFxYasvmEQdpS", result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlot()
        {
            var responseData = File.ReadAllText("Resources/Http/Slot/GetSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Slot/GetSlotRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSlotProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/Slot/GetSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Slot/GetSlotProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSlot(Types.Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1234UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetRecentPerformanceSamples()
        {
            var responseData = File.ReadAllText("Resources/Http/GetRecentPerformanceSamplesResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetRecentPerformanceSamplesRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetRecentPerformanceSamples(4);

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
            var responseData = File.ReadAllText("Resources/Http/GetSnapshotSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSnapshotSlotRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSnapshotSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(100UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSupply()
        {
            var responseData = File.ReadAllText("Resources/Http/GetSupplyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSupplyRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSupply();

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
            var responseData = File.ReadAllText("Resources/Http/GetSupplyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSupplyProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSupply(Commitment.Processed);

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
            var responseData = File.ReadAllText("Resources/Http/GetStakeActivationResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetStakeActivationRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetStakeActivation("CYRJWqiSjLitBAcRxPvWpgX3s5TvmN2SuRY3eEYypFvT");

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
            var responseData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetStakeActivation("CYRJWqiSjLitBAcRxPvWpgX3s5TvmN2SuRY3eEYypFvT", 4);

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
            var responseData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetStakeActivationWithEpochProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetStakeActivation("CYRJWqiSjLitBAcRxPvWpgX3s5TvmN2SuRY3eEYypFvT", 4, Commitment.Processed);

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
            var responseData = File.ReadAllText("Resources/Http/GetMinimumLedgerSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetMinimumLedgerSlotRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetMinimumLedgerSlot();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(78969229UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetVersion()
        {
            var responseData = File.ReadAllText("Resources/Http/GetVersionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetVersionRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetVersion();

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

            var responseData = File.ReadAllText("Resources/Http/Health/GetHealthHealthyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Health/GetHealthRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetHealth();

            Assert.AreEqual(requestData, sentMessage);
            Assert.AreEqual("ok", res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetHealth_UnhealthyResponse()
        {

            var responseData = File.ReadAllText("Resources/Http/Health/GetHealthUnhealthyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Health/GetHealthRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetHealth();

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

            var responseData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetMinimumBalanceForRentExemption(50);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(500UL, res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetMinimumBalanceForRentExemptionConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetMinimumBalanceForRateExemptionConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetMinimumBalanceForRentExemption(50, Types.Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(500UL, res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestRequestAirdrop()
        {
            var responseData = File.ReadAllText("Resources/Http/RequestAirdropResult.json");
            var requestData = File.ReadAllText("Resources/Http/RequestAirdropRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.RequestAirdrop("6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z", 100000000000);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual("2iyRQZmksTfkmyH9Fnho61x4Y7TeSN8g3GRZCHmQjzzFB1e3DwKEVrYfR7AnKjiE5LiDEfCowtzoE2Pau646g1Vf", res.Result);

            FinishTest(messageHandlerMock, TestnetUri);

        }

        /// <summary>
        /// Backstory - public RPC nodes no longer support GetProgramAccount requests for the Serum program-id
        /// These are dealt with a HTTP 410 "GONE" response.
        /// Make sure that the JsonRpc gives an practical response under these circumstances.
        /// </summary>
        [TestMethod]
        public void TestFailHttp410Gone()
        {
            var responseData = File.ReadAllText("Resources/Http/GetProgramAccountsResponse-Fail-410.json");
            var requestData = File.ReadAllText("Resources/Http/GetProgramAccountsRequest-Fail-410.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData, HttpStatusCode.Gone);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            List<MemCmp> filters = new()
            {
                new MemCmp { Offset = 45, Bytes = "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5" },
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetProgramAccounts("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin", // serum program-id
                dataSize:3228, memCmpList:filters ); 

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Gone, result.HttpStatusCode);
            Assert.AreEqual(responseData, result.RawRpcResponse);
            Assert.IsFalse(result.WasSuccessful);
            Assert.IsNull(result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        /// <summary>
        /// Backstory - when hopping from public RPC node to Serum RPC node there was a difference in their configuration
        /// Solnet was sending Content-Type header of `application/json; charset=utf-8` which is rejected by Serum RPC cluster.
        /// This was dealt with by a HTTP 415 response which was not accessible in the Solnet Result.
        /// </summary>
        [TestMethod]
        public void TestFailHttp415UnsupportedMediaType()
        {
            var responseData = "Supplied content type is not allowed. Content-Type: application/json is required";
            var requestData = File.ReadAllText("Resources/Http/GetBalanceRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData, HttpStatusCode.UnsupportedMediaType);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");

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