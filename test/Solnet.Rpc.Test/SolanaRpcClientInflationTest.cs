using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientInflationTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetInflationGovernor()
        {
            string responseData = File.ReadAllText("Resources/Http/Inflation/GetInflationGovernorResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Inflation/GetInflationGovernorRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<InflationGovernor> result = sut.GetInflationGovernor();

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
        public void TestGetInflationGovernorConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Inflation/GetInflationGovernorResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Inflation/GetInflationGovernorConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<InflationGovernor> result = sut.GetInflationGovernor(Commitment.Confirmed);

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
            string responseData = File.ReadAllText("Resources/Http/Inflation/GetInflationRateResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Inflation/GetInflationRateRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<InflationRate> result = sut.GetInflationRate();

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
            string responseData = File.ReadAllText("Resources/Http/Inflation/GetInflationRewardResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Inflation/GetInflationRewardRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<InflationReward>> result = sut.GetInflationReward(
                new List<string>
                {
                    "6dmNQ5jwLeLk5REvio1JcMshcbvkYMwy26sJ8pbkvStu", "BGsqMegLpV6n6Ve146sSX2dTjUMj3M92HnU8BbNRMhF2"
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
        public void TestGetInflationRewardProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/Inflation/GetInflationRewardResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Inflation/GetInflationRewardProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<InflationReward>> result = sut.GetInflationReward(
                new List<string>
                {
                    "6dmNQ5jwLeLk5REvio1JcMshcbvkYMwy26sJ8pbkvStu", "BGsqMegLpV6n6Ve146sSX2dTjUMj3M92HnU8BbNRMhF2"
                }, 2, Commitment.Processed);

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
            string responseData = File.ReadAllText("Resources/Http/Inflation/GetInflationRewardNoEpochResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Inflation/GetInflationRewardNoEpochRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<InflationReward>> result = sut.GetInflationReward(
                new List<string>
                {
                    "25xzEf8cqLLEm2wyZTEBtCDchsUFm3SVESjs6eEFHJWe", "GPQdoUUDQXM1gWgRVwBbYmDqAgxoZN3bhVeKr1P8jd4c"
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
    }
}