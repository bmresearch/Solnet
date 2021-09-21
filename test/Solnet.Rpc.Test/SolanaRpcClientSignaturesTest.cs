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
    public class SolanaRpcClientSignaturesTest : SolanaRpcClientTestBase
    {

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
        public void TestGetSignaturesForAddress2()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2Request.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetConfirmedSignaturesForAddress2("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b", limit: 3);

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
        public void TestGetSignaturesForAddress_InvalidCommitment()
        {

            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetSignaturesForAddress("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b", commitment: Types.Commitment.Processed);
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

            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetConfirmedSignaturesForAddress2("4Rf9mGD7FeYknun5JczX5nGLTfQuS1GRjNVfkEMKE92b", commitment: Types.Commitment.Processed);
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
        public void TestGetSignaturesForAddress2Until()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressUntilResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2UntilRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetConfirmedSignaturesForAddress2(
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
        public void TestGetSignaturesForAddress2Before()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2BeforeRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetConfirmedSignaturesForAddress2(
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
        public void TestGetSignaturesForAddressBeforeConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeConfirmedRequest.json");
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
                1, before: "Vote111111111111111111111111111111111111111", commitment: Types.Commitment.Confirmed);

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
        public void TestGetSignaturesForAddress2BeforeConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Signatures/GetSignaturesForAddressBeforeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Signatures/GetConfirmedSignaturesForAddress2BeforeConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetConfirmedSignaturesForAddress2(
                "Vote111111111111111111111111111111111111111",
                1, before: "Vote111111111111111111111111111111111111111", commitment: Types.Commitment.Confirmed);

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
    }
}