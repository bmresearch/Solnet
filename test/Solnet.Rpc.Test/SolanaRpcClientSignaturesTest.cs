using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientSignaturesTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetSignaturesForAddress()
        {
            string responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result =
                sut.GetSignaturesForAddress("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b", 3);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(3, result.Result.Count);
            Assert.AreEqual(1616245823UL, result.Result[0].BlockTime);
            Assert.AreEqual(68710495UL, result.Result[0].Slot);
            Assert.AreEqual("5Jofwx5JcPT1dMsgo6DkyT6x61X5chS9K7hM7huGKAnUq8xxHwGKuDnnZmPGoapWVZcN4cPvQtGNCicnWZfPHowr",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddress2()
        {
            string responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2Request.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result =
                sut.GetConfirmedSignaturesForAddress2("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b", 3);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(3, result.Result.Count);
            Assert.AreEqual(1616245823UL, result.Result[0].BlockTime);
            Assert.AreEqual(68710495UL, result.Result[0].Slot);
            Assert.AreEqual("5Jofwx5JcPT1dMsgo6DkyT6x61X5chS9K7hM7huGKAnUq8xxHwGKuDnnZmPGoapWVZcN4cPvQtGNCicnWZfPHowr",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddress_InvalidCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<List<SignatureStatusInfo>> res =
                    sut.GetSignaturesForAddress("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b",
                        commitment: Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetSignaturesForAddress2_InvalidCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<List<SignatureStatusInfo>> res =
                    sut.GetConfirmedSignaturesForAddress2("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b",
                        commitment: Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetSignaturesForAddressUntil()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressUntilResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressUntilRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result = sut.GetSignaturesForAddress(
                "Vote111111111111111111111111111111111111111",
                1, until: "Vote111111111111111111111111111111111111111");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddress2Until()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressUntilResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2UntilRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result = sut.GetConfirmedSignaturesForAddress2(
                "Vote111111111111111111111111111111111111111",
                1, until: "Vote111111111111111111111111111111111111111");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddressBefore()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result = sut.GetSignaturesForAddress(
                "Vote111111111111111111111111111111111111111",
                1, "Vote111111111111111111111111111111111111111");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddress2Before()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2BeforeRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result = sut.GetConfirmedSignaturesForAddress2(
                "Vote111111111111111111111111111111111111111",
                1, "Vote111111111111111111111111111111111111111");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddressBeforeConfirmed()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result = sut.GetSignaturesForAddress(
                "Vote111111111111111111111111111111111111111",
                1, "Vote111111111111111111111111111111111111111", commitment: Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignaturesForAddress2BeforeConfirmed()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            string requestData =
                File.ReadAllText(
                    "Resources/Http/Signatures/GetConfirmedSignaturesForAddress2BeforeConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<List<SignatureStatusInfo>> result = sut.GetConfirmedSignaturesForAddress2(
                "Vote111111111111111111111111111111111111111",
                1, "Vote111111111111111111111111111111111111111", commitment: Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(null, result.Result[0].BlockTime);
            Assert.AreEqual(114UL, result.Result[0].Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv",
                result.Result[0].Signature);
            Assert.AreEqual(null, result.Result[0].Memo);
            Assert.AreEqual(null, result.Result[0].Error);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetSignatureStatuses()
        {
            string responseData = File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<List<SignatureStatusInfo>>> result = sut.GetSignatureStatuses(
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
            string responseData =
                File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesWithHistoryResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Signatures/GetSignatureStatusesWithHistoryRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<List<SignatureStatusInfo>>> result = sut.GetSignatureStatuses(
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
    }
}