using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Solnet.Rpc.Types;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientTest
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
                .Callback<HttpRequestMessage, CancellationToken>((httpRequest, ct) => sentPayloadCapture(httpRequest.Content.ReadAsStringAsync(ct).Result))
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            
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
        public void TestGetAccountInfo()
        {
            var responseData = File.ReadAllText("Resources/Http/GetAccountInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetAccountInfoRequest.json");
            
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            
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
        public void TestGetBlockCommitment()
        {
            var responseData = File.ReadAllText("Resources/Http/GetBlockCommitmentResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetBlockCommitmentRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);
            
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetBlockCommitment(78561320);
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(null, result.Result.Commitment);
            Assert.AreEqual(78380558524696194UL, result.Result.TotalStake);
            
            FinishTest(messageHandlerMock, TestnetUri);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetBalance("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79274779UL, result.Result.Context.Slot);
            Assert.AreEqual(168855000000UL, result.Result.Value);
            
            FinishTest(messageHandlerMock, TestnetUri);
        }
        
        [TestMethod]
        public void TestGetBlockTime()
        {
            var responseData = File.ReadAllText("Resources/Http/GetBlockTimeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetBlockTimeRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);
            
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetBlockTime(78561320);
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1621971949UL, result.Result);
            
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetClusterNodes();
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(5, result.Result.Length);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetGenesisHash();
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("4uhcVJyU9pJkvQyS88uRDiswHXSCkY3zQawwpjk2NsNY", result.Result);
            
            FinishTest(messageHandlerMock, TestnetUri);
        }
        
        [TestMethod]
        public void TestGetRecentBlockHash()
        {
            var responseData = File.ReadAllText("Resources/Http/GetRecentBlockhashResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetRecentBlockhashRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);
            
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetRecentBlockHash();
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79206433UL, result.Result.Context.Slot);
            Assert.AreEqual("FJGZeJiYkwCZCnFbGujHxfVGnFgrgZiomczHr247Tn2p", result.Result.Value.Blockhash);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);
            
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetTokenAccountsByOwner(
                "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 
                tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79200468UL, result.Result.Context.Slot);
            Assert.AreEqual(7, result.Result.Value.Length);
            
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetTokenAccountsByDelegate(
                "4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T", 
                tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1114UL, result.Result.Context.Slot);
            Assert.AreEqual(1, result.Result.Value.Length);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", result.Result.Value[0].Account.Owner);
            Assert.AreEqual(false, result.Result.Value[0].Account.Executable);
            Assert.AreEqual(4UL, result.Result.Value[0].Account.RentEpoch);
            Assert.AreEqual(1726080UL, result.Result.Value[0].Account.Lamports);
            Assert.AreEqual("4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T", result.Result.Value[0].Account.Data.Parsed.Info.Delegate);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetTokenLargestAccounts("7ugkvt26sFjMdiFQFP5AQX8m8UkxWaW7rk2nBk4R6Gf2");
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79207653UL, result.Result.Context.Slot);
            Assert.AreEqual(1, result.Result.Value.Length);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var result = sut.GetMinimumLedgerSlot();
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(78969229UL, result.Result);
            
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
            var txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                         "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                         "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                         "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                         "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";
            
            var result = sut.SendTransaction(txData);
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("gaSFQXFqbYQypZdMFZy4Fe7uB2VFDEo4sGDypyrVxFgzZqc5MqWnRWTT9hXamcrFRcsiiH15vWii5ACSsyNScbp", result.Result);
            
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
 
            var sut = new SolanaRpcClient(TestnetUrl, httpClient);
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
            Assert.AreEqual(2, result.Result.Value.Error.InstructionError.Length);
            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}