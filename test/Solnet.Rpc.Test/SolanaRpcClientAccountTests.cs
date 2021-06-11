using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    public partial class SolanaRpcClientTest
    {
        [TestMethod]
        public void TestGetAccountInfoDefault()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetAccountInfo("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79200467UL, result.Result.Context.Slot);
            Assert.AreEqual("", result.Result.Value.Data[0]);
            Assert.AreEqual("base64", result.Result.Value.Data[1]);
            Assert.AreEqual(false, result.Result.Value.Executable);
            Assert.AreEqual(5478840UL, result.Result.Value.Lamports);
            Assert.AreEqual("11111111111111111111111111111111", result.Result.Value.Owner);
            Assert.AreEqual(195UL, result.Result.Value.RentEpoch);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetAccountInfoConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoConfirmedRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetAccountInfo("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", Types.Commitment.Confirmed);
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79200467UL, result.Result.Context.Slot);
            Assert.AreEqual("", result.Result.Value.Data[0]);
            Assert.AreEqual("base64", result.Result.Value.Data[1]);
            Assert.AreEqual(false, result.Result.Value.Executable);
            Assert.AreEqual(5478840UL, result.Result.Value.Lamports);
            Assert.AreEqual("11111111111111111111111111111111", result.Result.Value.Owner);
            Assert.AreEqual(195UL, result.Result.Value.RentEpoch);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetProgramAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetProgramAccounts("GrAkKfEpTKQuVHG2Y97Y2FF4i7y7Q5AHLK94JBy7Y5yv");
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(2, result.Result.Count);
            Assert.AreEqual("FzNKvS4SCHDoNbnnfhmGSLVRCLNBUuGecxdvobSGmWMh", result.Result[0].PublicKey);

            Assert.AreEqual("NhOiFR2mEcZJFj1ciaG2IrWOf2poe4LNGYC5gvdULBYyFH1Kq4cdNyYf+7u2r6NaWXHwnqiXnCzkFhIDU"
                + "jSbNN2i/bmtSgasAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADkpoamWb2mUaHqREQNm8VPcqSWUGCgPjWK"
                + "jh0raCI+OEo8UAXpyc1w/8KV64XXwhGP70z6aN3K1vnzjpYXQqr3vvsgJ4UD4OatRY1IsR9NYTReSKpRIhPpTupzQ9W"
                + "zTpfWSTLZP2xvdcWyo8spQGJ2uGX0jH9h4ZxJ+orI/IsnqxyAHH+MXZuMBl28YfgFJRh8PZHPKbmFvVPDFs3xgBVWzz"
                + "QuNTAlY5aWAEN5CRqkYmOXDcge++gRlEry6ItrMEA0VZV0zsOFk2oDiT9W7slB3JefUOpWS4DMPJW6N0zRUDTtXaGmW"
                + "rqt6W4vEGC0DnBI++A2ZkHoMmJ+qeCKBVkNJgAAADc4o2AAAAAA/w==", result.Result[0].Account.Data[0]);
            Assert.AreEqual("base64", result.Result[0].Account.Data[1]);
            Assert.AreEqual(false, result.Result[0].Account.Executable);
            Assert.AreEqual(3486960UL, result.Result[0].Account.Lamports);
            Assert.AreEqual("GrAkKfEpTKQuVHG2Y97Y2FF4i7y7Q5AHLK94JBy7Y5yv", result.Result[0].Account.Owner);
            Assert.AreEqual(188UL, result.Result[0].Account.RentEpoch);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetMultipleAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetMultipleAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetMultipleAccountsRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetMultipleAccounts(new List<string> { "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5" });
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(2, result.Result.Value.Count);

            Assert.AreEqual("base64", result.Result.Value[0].Data[1]);
            Assert.AreEqual("", result.Result.Value[0].Data[0]);
            Assert.AreEqual(false, result.Result.Value[0].Executable);
            Assert.AreEqual(503668985208UL, result.Result.Value[0].Lamports);
            Assert.AreEqual("11111111111111111111111111111111", result.Result.Value[0].Owner);
            Assert.AreEqual(197UL, result.Result.Value[0].RentEpoch);

            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}