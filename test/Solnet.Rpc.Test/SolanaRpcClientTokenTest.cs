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
    public class SolanaRpcClientTokenTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetTokenSupply()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenSupplyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenSupplyRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

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
        public void TestGetTokenSupplyProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenSupplyResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenSupplyProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenSupply("7ugkvt26sFjMdiFQFP5AQX8m8UkxWaW7rk2nBk4R6Gf2", Commitment.Processed);

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
        public void TestGetTokenAccountsByOwnerException()
        {
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                _ = sut.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetTokenAccountsByOwner()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByOwnerResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByOwnerRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

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
        public void TestGetTokenAccountsByOwnerConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByOwnerResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByOwnerConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenAccountsByOwner(
                "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",
                tokenMintPubKey: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", commitment: Commitment.Confirmed);

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
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByDelegateResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByDelegateRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

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
            Assert.AreEqual("1", result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.Amount);
            Assert.AreEqual(1, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.Decimals);
            Assert.AreEqual("0.1", result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.UiAmountString);
            Assert.AreEqual(0.1D, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.AmountDouble);
            Assert.AreEqual(1UL, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.AmountUlong);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTokenAccountsByDelegateProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByDelegateResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenAccountsByDelegateProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetTokenAccountsByDelegate(
                "4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T",
                tokenMintPubKey: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", commitment: Commitment.Processed);

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
            Assert.AreEqual("1", result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.Amount);
            Assert.AreEqual(1, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.Decimals);
            Assert.AreEqual("0.1", result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.UiAmountString);
            Assert.AreEqual(0.1D, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.AmountDouble);
            Assert.AreEqual(1UL, result.Result.Value[0].Account.Data.Parsed.Info.DelegatedAmount.AmountUlong);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTokenAccountsByDelegateBadParams()
        {
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var result = sut.GetTokenAccountsByDelegate("4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T");
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetTokenAccountBalance()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenAccountBalanceResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenAccountBalanceRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

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
        public void TestGetTokenAccountBalanceConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenAccountBalanceResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenAccountBalanceConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result =
                sut.GetTokenAccountBalance("7247amxcSBamBSKZJrqbj373CiJSa1v21cRav56C3WfZ", Commitment.Confirmed);

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
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenLargestAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenLargestAccountsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

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
        public void TestGetTokenLargestAccountsProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/Token/GetTokenLargestAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Token/GetTokenLargestAccountsProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object) { BaseAddress = TestnetUri, };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result =
                sut.GetTokenLargestAccounts("7ugkvt26sFjMdiFQFP5AQX8m8UkxWaW7rk2nBk4R6Gf2", Commitment.Processed);

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
    }
}