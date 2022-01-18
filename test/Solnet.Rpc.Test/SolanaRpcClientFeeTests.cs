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
    public class SolanaRpcClientFeeTest : SolanaRpcClientTestBase
    {


        [TestMethod]
        public void TestGetFeeRateGovernor()
        {
            var responseData = File.ReadAllText("Resources/Http/Fees/GetFeeRateGovernorResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Fees/GetFeeRateGovernorRequest.json");
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
            var responseData = File.ReadAllText("Resources/Http/Fees/GetFeesResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Fees/GetFeesRequest.json");
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
        public void TestGetFeesConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Fees/GetFeesResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Fees/GetFeesConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetFees(Commitment.Confirmed);

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
            var responseData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetFeeCalculatorForBlockhash("GJxqhuxcgfn5Tcj6y3f8X4FeCDd2RQ6SnEMo1AAxrPRZ");

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
            var responseData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Fees/GetFeeCalculatorForBlockhashConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetFeeCalculatorForBlockhash("GJxqhuxcgfn5Tcj6y3f8X4FeCDd2RQ6SnEMo1AAxrPRZ", Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(221UL, result.Result.Context.Slot);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);

            FinishTest(messageHandlerMock, TestnetUri);
        }

    }
}