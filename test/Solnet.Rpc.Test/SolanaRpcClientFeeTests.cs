using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System.IO;
using System.Net.Http;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientFeeTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetFeeRateGovernor()
        {
            string responseData = File.ReadAllText("Resources/Http/Fees/GetFeeRateGovernorResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Fees/GetFeeRateGovernorRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<FeeRateGovernorInfo>> result = sut.GetFeeRateGovernor();

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
            string responseData = File.ReadAllText("Resources/Http/Fees/GetFeesResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Fees/GetFeesRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<FeesInfo>> result = sut.GetFees();

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
        public void TestGetFeesConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Fees/GetFeesResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Fees/GetFeesConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<FeesInfo>> result = sut.GetFees(Commitment.Confirmed);

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
        public void TestGetFeeCalculatorForBlockhash()
        {
            string responseData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<FeeCalculatorInfo>> result =
                sut.GetFeeCalculatorForBlockhash("GJxqhuxcgfn5Tcj6y3f8X4FeCDd2RQ6SnEMo1AAxrPRZ");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(221UL, result.Result.Context.Slot);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetFeeCalculatorForBlockhashConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<FeeCalculatorInfo>> result =
                sut.GetFeeCalculatorForBlockhash("GJxqhuxcgfn5Tcj6y3f8X4FeCDd2RQ6SnEMo1AAxrPRZ", Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(221UL, result.Result.Context.Slot);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);

            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}