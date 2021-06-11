using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Solnet.Rpc.Models;
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
    public partial class SolanaRpcClientTest
    {
        private const string TestnetUrl = "https://testnet.solana.com";
        private static readonly Uri TestnetUri = new Uri(TestnetUrl);

        /// <summary>
        /// Setup the test with the request and response data.
        /// </summary>
        /// <param name="sentPayloadCapture">Capture the sent content.</param>
        /// <param name="responseContent">The response content.</param>
        private Mock<HttpMessageHandler> SetupTest(Action<string> sentPayloadCapture, string responseContent)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        message => message.Method == HttpMethod.Post &&
                                   message.RequestUri == TestnetUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((httpRequest, ct) =>
                    sentPayloadCapture(httpRequest.Content.ReadAsStringAsync(ct).Result))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();
            return messageHandlerMock;
        }

        /// <summary>
        /// Finish the test by asserting the http request went as expected.
        /// </summary>
        /// <param name="expectedUri">The request uri.</param>
        private void FinishTest(Mock<HttpMessageHandler> mockHandler, Uri expectedUri)
        {
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

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
            var responseData = File.ReadAllText("Resources/Http/GetEpochInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetEpochInfoRequest.json");
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
        public void TestGetEpochSchedule()
        {
            var responseData = File.ReadAllText("Resources/Http/GetEpochScheduleResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetEpochScheduleRequest.json");
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
        public void TestGetFeeRateGovernor()
        {
            var responseData = File.ReadAllText("Resources/Http/GetFeeRateGovernorResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetFeeRateGovernorRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetFeeRateGovernor();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(54UL, result.Result.Context.Slot);
            Assert.AreEqual(50, result.Result.Value.FeeRateGovernor.BurnPercent);
            Assert.AreEqual(100000UL, result.Result.Value.FeeRateGovernor.MaxLamportsPerSignature);
            Assert.AreEqual(5000UL, result.Result.Value.FeeRateGovernor.MinLamportsPerSignature);
            Assert.AreEqual(10000UL, result.Result.Value.FeeRateGovernor.TargetLamportsPerSignature);
            Assert.AreEqual(20000UL, result.Result.Value.FeeRateGovernor.TargetSignaturesPerSlot);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetFees()
        {
            var responseData = File.ReadAllText("Resources/Http/GetFeesResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetFeesRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetFees();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1UL, result.Result.Context.Slot);
            Assert.AreEqual("CSymwgTNX1j3E4qhKfJAUE41nBWEwXufoYryPbkde5RR", result.Result.Value.Blockhash);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);
            Assert.AreEqual(297UL, result.Result.Value.LastValidSlot);
            Assert.AreEqual(296UL, result.Result.Value.LastValidBlockHeight);

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
        public void TestGetInflationGovernor()
        {
            var responseData = File.ReadAllText("Resources/Http/GetInflationGovernorResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetInflationGovernorRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetInflationGovernor();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual((decimal)0.05, result.Result.Foundation);
            Assert.AreEqual(7, result.Result.FoundationTerm);
            Assert.AreEqual((decimal)0.15, result.Result.Initial);
            Assert.AreEqual((decimal)0.15, result.Result.Taper);
            Assert.AreEqual((decimal)0.015, result.Result.Terminal);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetInflationRate()
        {
            var responseData = File.ReadAllText("Resources/Http/GetInflationRateResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetInflationRateRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetInflationRate();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(100, result.Result.Epoch);
            Assert.AreEqual((decimal)0.149, result.Result.Total);
            Assert.AreEqual((decimal)0.148, result.Result.Validator);
            Assert.AreEqual((decimal)0.001, result.Result.Foundation);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetInflationReward()
        {
            var responseData = File.ReadAllText("Resources/Http/GetInflationRewardResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetInflationRewardRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetInflationReward(
                new List<string>
                {
                    "6dmNQ5jwLeLk5REvio1JcMshcbvkYMwy26sJ8pbkvStu",
                    "BGsqMegLpV6n6Ve146sSX2dTjUMj3M92HnU8BbNRMhF2"
                }, 2);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(2, result.Result.Count);
            Assert.AreEqual(2500UL, result.Result[0].Amount);
            Assert.AreEqual(224UL, result.Result[0].EffectiveSlot);
            Assert.AreEqual(2UL, result.Result[0].Epoch);
            Assert.AreEqual(499999442500UL, result.Result[0].PostBalance);
            Assert.AreEqual(null, result.Result[1]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetInflationRewardNoEpoch()
        {
            var responseData = File.ReadAllText("Resources/Http/GetInflationRewardNoEpochResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetInflationRewardNoEpochRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetInflationReward(
                new List<string>
                {
                    "25xzEf8cqLLEm2wyZTEBtCDchsUFm3SVESjs6eEFHJWe",
                    "GPQdoUUDQXM1gWgRVwBbYmDqAgxoZN3bhVeKr1P8jd4c"
                });

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(2, result.Result.Count);
            Assert.AreEqual(1758149777313UL, result.Result[0].Amount);
            Assert.AreEqual(81216004UL, result.Result[0].EffectiveSlot);
            Assert.AreEqual(187UL, result.Result[0].Epoch);
            Assert.AreEqual(1759149777313UL, result.Result[0].PostBalance);
            Assert.AreEqual(null, result.Result[1]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetLargestAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/GetLargestAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetLargestAccountsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetLargestAccounts("circulating");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(20, result.Result.Value.Count);
            Assert.AreEqual("6caH6ayzofHnP8kcPQTEBrDPG4A2qDo1STE5xTMJ52k8", result.Result.Value[0].Address);
            Assert.AreEqual(20161157050000000UL, result.Result.Value[0].Lamports);
            Assert.AreEqual("gWgqQ4udVxE3uNxRHEwvftTHwpEmPHAd8JR9UzaHbR2", result.Result.Value[19].Address);
            Assert.AreEqual(2499999990454560UL, result.Result.Value[19].Lamports);

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
            var responseData = File.ReadAllText("Resources/Http/GetSlotLeadersEmptyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSlotLeadersEmptyRequest.json");
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
            var responseData = File.ReadAllText("Resources/Http/GetSlotLeadersResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSlotLeadersRequest.json");
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
            var responseData = File.ReadAllText("Resources/Http/GetSlotLeaderResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSlotLeaderRequest.json");
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
        public void TestGetSlot()
        {
            var responseData = File.ReadAllText("Resources/Http/GetSlotResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetSlotRequest.json");
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
        public void TestGetSignaturesForAddress()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSignaturesForAddress("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b", limit: 3);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(3, result.Result.Count);
            Assert.AreEqual(1616245823UL, result.Result[0].BlockTime);
            Assert.AreEqual(68710495UL, result.Result[0].Slot);
            Assert.AreEqual("5Jofwx5JcPT1dMsgo6DkyT6x61X5chS9K7hM7huGKAnUq8xxHwGKuDnnZmPGoapWVZcN4cPvQtGNCicnWZfPHowr", result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddressUntil()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressUntilResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressUntilRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSignaturesForAddress(
                "Vote111111111111111111111111111111111111111",
                1, until: "Vote111111111111111111111111111111111111111");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv", result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddressBefore()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSignaturesForAddress(
                "Vote111111111111111111111111111111111111111",
                1, before: "Vote111111111111111111111111111111111111111");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv", result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignatureStatuses()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSignatureStatuses(
                new List<string>
                {
                    "5VERv8NMvzbJMEkV8xnrLkEaWRtSz9CosKDYjCJjBRnbJLgp8uirBgmQpjKhoR4tjF3ZpRzrFmBV6UjKdiSZkQUW",
                    "5j7s6NiJS3JAkvgkoc18WVAsiSaci2pxB2A6ueCJP4tprA2TFg9wSyTLeYouxPBJEMzJinENTkpA52YStRW5Dia7"
                });

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(82UL, result.Result.Context.Slot);
            Assert.AreEqual(2, result.Result.Value.Count);
            Assert.AreEqual(null, result.Result.Value[1]);
            Assert.AreEqual(72UL, result.Result.Value[0].Slot);
            Assert.AreEqual(10UL, result.Result.Value[0].Confirmations);
            Assert.AreEqual("confirmed", result.Result.Value[0].ConfirmationStatus);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignatureStatusesWithHistory()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesWithHistoryResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesWithHistoryRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetSignatureStatuses(
                new List<string>
                {
                    "5VERv8NMvzbJMEkV8xnrLkEaWRtSz9CosKDYjCJjBRnbJLgp8uirBgmQpjKhoR4tjF3ZpRzrFmBV6UjKdiSZkQUW",
                    "5j7s6NiJS3JAkvgkoc18WVAsiSaci2pxB2A6ueCJP4tprA2TFg9wSyTLeYouxPBJEMzJinENTkpA52YStRW5Dia7"
                }, true);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(82UL, result.Result.Context.Slot);
            Assert.AreEqual(2, result.Result.Value.Count);
            Assert.AreEqual(null, result.Result.Value[1]);
            Assert.AreEqual(48UL, result.Result.Value[0].Slot);
            Assert.AreEqual(null, result.Result.Value[0].Confirmations);
            Assert.AreEqual("finalized", result.Result.Value[0].ConfirmationStatus);

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
        public void TestGetTokenSupply()
        {
            var responseData = File.ReadAllText("Resources/Http/GetTokenSupplyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetTokenSupplyRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenSupply("7ugkvt26sFjMdiFQFP5AQX8m8UkxWaW7rk2nBk4R6Gf2");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79266576UL, result.Result.Context.Slot);
            Assert.AreEqual("1000", result.Result.Value.Amount);
            Assert.AreEqual(2, result.Result.Value.Decimals);
            Assert.AreEqual("10", result.Result.Value.UiAmountString);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TestGetTokenAccountsByOwnerException()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            _ = sut.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");
        }

        [TestMethod]
        public void TestGetTokenAccountsByOwner()
        {
            var responseData = File.ReadAllText("Resources/Http/GetTokenAccountsByOwnerResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetTokenAccountsByOwnerRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenAccountsByOwner(
                "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",
                tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79200468UL, result.Result.Context.Slot);
            Assert.AreEqual(7, result.Result.Value.Count);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTokenAccountsByDelegate()
        {
            var responseData = File.ReadAllText("Resources/Http/GetTokenAccountsByDelegateResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetTokenAccountsByDelegateRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenAccountsByDelegate(
                "4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T",
                tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1114UL, result.Result.Context.Slot);
            Assert.AreEqual(1, result.Result.Value.Count);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", result.Result.Value[0].Account.Owner);
            Assert.AreEqual(false, result.Result.Value[0].Account.Executable);
            Assert.AreEqual(4UL, result.Result.Value[0].Account.RentEpoch);
            Assert.AreEqual(1726080UL, result.Result.Value[0].Account.Lamports);
            Assert.AreEqual("4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T",
                result.Result.Value[0].Account.Data.Parsed.Info.Delegate);
            Assert.AreEqual(1UL, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTokenAccountBalance()
        {
            var responseData = File.ReadAllText("Resources/Http/GetTokenAccountBalanceResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetTokenAccountBalanceRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenAccountBalance("7247amxcSBamBSKZJrqbj373CiJSa1v21cRav56C3WfZ");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79207643UL, result.Result.Context.Slot);
            Assert.AreEqual("1000", result.Result.Value.Amount);
            Assert.AreEqual(2, result.Result.Value.Decimals);
            Assert.AreEqual("10", result.Result.Value.UiAmountString);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTokenLargestAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/GetTokenLargestAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetTokenLargestAccountsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenLargestAccounts("7ugkvt26sFjMdiFQFP5AQX8m8UkxWaW7rk2nBk4R6Gf2");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79207653UL, result.Result.Context.Slot);
            Assert.AreEqual(1, result.Result.Value.Count);
            Assert.AreEqual("7247amxcSBamBSKZJrqbj373CiJSa1v21cRav56C3WfZ", result.Result.Value[0].Address);
            Assert.AreEqual("1000", result.Result.Value[0].Amount);
            Assert.AreEqual(2, result.Result.Value[0].Decimals);
            Assert.AreEqual("10", result.Result.Value[0].UiAmountString);

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
        public void TestGetVoteAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/GetVoteAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetVoteAccountsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetVoteAccounts();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Current.Length);
            Assert.AreEqual(1, result.Result.Delinquent.Length);

            Assert.AreEqual(81274518UL, result.Result.Current[0].RootSlot);
            Assert.AreEqual("3ZT31jkAGhUaw8jsy4bTknwBMP8i4Eueh52By4zXcsVw", result.Result.Current[0].VotePublicKey);
            Assert.AreEqual("B97CCUW3AEZFGy6uUg6zUdnNYvnVq5VG8PUtb2HayTDD", result.Result.Current[0].NodePublicKey);
            Assert.AreEqual(42UL, result.Result.Current[0].ActivatedStake);
            Assert.AreEqual(0, result.Result.Current[0].Commission);
            Assert.AreEqual(147UL, result.Result.Current[0].LastVote);
            Assert.AreEqual(true, result.Result.Current[0].EpochVoteAccount);
            Assert.AreEqual(2, result.Result.Current[0].EpochCredits.Length);

            Assert.AreEqual(1234UL, result.Result.Delinquent[0].RootSlot);
            Assert.AreEqual("CmgCk4aMS7KW1SHX3s9K5tBJ6Yng2LBaC8MFov4wx9sm", result.Result.Delinquent[0].VotePublicKey);
            Assert.AreEqual("6ZPxeQaDo4bkZLRsdNrCzchNQr5LN9QMc9sipXv9Kw8f", result.Result.Delinquent[0].NodePublicKey);
            Assert.AreEqual(0UL, result.Result.Delinquent[0].ActivatedStake);
            Assert.AreEqual(false, result.Result.Delinquent[0].EpochVoteAccount);
            Assert.AreEqual(127UL, result.Result.Delinquent[0].Commission);
            Assert.AreEqual(0UL, result.Result.Delinquent[0].LastVote);
            Assert.AreEqual(0, result.Result.Delinquent[0].EpochCredits.Length);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTransactionCount()
        {
            var responseData = File.ReadAllText("Resources/Http/GetTransactionCountResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetTransactionCountRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTransactionCount();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(23632393337UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSendTransaction()
        {
            var responseData = File.ReadAllText("Resources/Http/SendTransactionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/SendTransactionRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                         "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                         "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                         "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                         "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            var result = sut.SendTransaction(txData);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("gaSFQXFqbYQypZdMFZy4Fe7uB2VFDEo4sGDypyrVxFgzZqc5MqWnRWTT9hXamcrFRcsiiH15vWii5ACSsyNScbp",
                result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSimulateTransaction()
        {
            var responseData = File.ReadAllText("Resources/Http/SimulateTransactionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/SimulateTransactionRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                         "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                         "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                         "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                         "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            var result = sut.SimulateTransaction(txData);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result.Value);
            Assert.AreEqual(79206888UL, result.Result.Context.Slot);
            Assert.AreEqual(null, result.Result.Value.Error);
            Assert.AreEqual(5, result.Result.Value.Logs.Length);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSimulateTransactionInsufficientLamports()
        {
            var responseData = File.ReadAllText("Resources/Http/SimulateTransactionInsufficientLamportsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/SimulateTransactionInsufficientLamportsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var txData = "ARymmnVB6PB0x//jV2vsTFFdeOkzD0FFoQq6P\u002BwzGKlMD\u002BXLb/hWnOebNaYlg/" +
                         "\u002Bj6jdm9Fe2Sba/ACnvcv9KIA4BAAIEUy4zulRg8z2yKITZaNwcnq6G6aH8D0ITae862qbJ" +
                         "\u002B3eE3M6r5DRwldquwlqOuXDDOWZagXmbHnAU3w5Dg44kogAAAAAAAAAAAAAAAAAAAAAAAA" +
                         "AAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qBann0itTd6uxx69h" +
                         "ION5Js4E4drRP8CWwoLTdorAFUqAICAgABDAIAAACAlpgAAAAAAAMBABVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            var result = sut.SimulateTransaction(txData);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result.Value);
            Assert.AreEqual(79203980UL, result.Result.Context.Slot);
            Assert.AreEqual(3, result.Result.Value.Logs.Length);
            Assert.IsNotNull(result.Result.Value.Error);
            Assert.AreEqual(TransactionErrorType.InstructionError, result.Result.Value.Error.Type);
            Assert.IsNotNull(result.Result.Value.Error.InstructionError);
            Assert.AreEqual(InstructionErrorType.Custom, result.Result.Value.Error.InstructionError.Type);
            Assert.AreEqual(1, result.Result.Value.Error.InstructionError.CustomError);
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
    }
}