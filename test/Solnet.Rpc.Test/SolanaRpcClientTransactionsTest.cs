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
    public class SolanaRpcClientTransactionsTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetTransactionCount()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionCountResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/GetTransactionCountRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetTransactionCount();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(23632393337UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTransactionCountProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionCountResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Transaction/GetTransactionCountProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetTransactionCount(Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(23632393337UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSendTransaction()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/SendTransactionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/SendTransactionRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            string txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                            "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                            "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                            "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                            "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            RequestResult<string> result = sut.SendTransaction(txData);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual("gaSFQXFqbYQypZdMFZy4Fe7uB2VFDEo4sGDypyrVxFgzZqc5MqWnRWTT9hXamcrFRcsiiH15vWii5ACSsyNScbp",
                result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSendTransactionBytes()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/SendTransactionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/SendTransactionWithParamsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            string txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                            "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                            "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                            "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                            "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            byte[] bytes = Convert.FromBase64String(txData);

            RequestResult<string> result = sut.SendTransaction(bytes, true, Commitment.Confirmed);

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
            string responseData = File.ReadAllText("Resources/Http/Transaction/SimulateTransactionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/SimulateTransactionRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            string txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                            "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                            "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                            "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                            "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            RequestResult<ResponseValue<SimulationLogs>> result = sut.SimulateTransaction(txData);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result.Value);
            Assert.AreEqual(79206888UL, result.Result.Context.Slot);
            Assert.AreEqual(null, result.Result.Value.Error);
            Assert.AreEqual(5, result.Result.Value.Logs.Length);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSimulateTransactionExtraParams()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/SimulateTransactionResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Transaction/SimulateTransactionExtraParamsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            string txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                            "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                            "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                            "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                            "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            RequestResult<ResponseValue<SimulationLogs>> result = sut.SimulateTransaction(txData, true,
                Commitment.Confirmed, false, new List<string> {"6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z"});

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result.Value);
            Assert.AreEqual(1, result.Result.Value.Accounts.Length);
            Assert.AreEqual(79206888UL, result.Result.Context.Slot);
            Assert.AreEqual(null, result.Result.Value.Error);
            Assert.AreEqual(5, result.Result.Value.Logs.Length);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSimulateTransactionBytesExtraParams()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/SimulateTransactionResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Transaction/SimulateTransactionExtraParamsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            string txData = "ASIhFkj3HRTDLiPxrxudL7eXCQ3DKrBB6Go/pn0sHWYIYgIHWYu2jZjbDXQseCEu73Li53BP7AEt8lCwKz" +
                            "X5awcBAAIER2mrlyBLqD\u002Bwyu4X94aPHgdOUhWBoNidlDedqmW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhP" +
                            "LlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KY" +
                            "GeIhJMvTu9qCKNNRNmSFNMnUzw5\u002BFDszWV6YvuvspBr0qlIoAdeg67wICAgABDAIAAACAlpgAAAAAAAMBA" +
                            "BVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            byte[] bytes = Convert.FromBase64String(txData);

            RequestResult<ResponseValue<SimulationLogs>> result = sut.SimulateTransaction(bytes, true,
                Commitment.Confirmed, false, new List<string> {"6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z"});

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result.Value);
            Assert.AreEqual(1, result.Result.Value.Accounts.Length);
            Assert.AreEqual(79206888UL, result.Result.Context.Slot);
            Assert.AreEqual(null, result.Result.Value.Error);
            Assert.AreEqual(5, result.Result.Value.Logs.Length);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestSimulateTransactionIncompatibleParams()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<ResponseValue<SimulationLogs>> result =
                    sut.SimulateTransaction("", true, Commitment.Finalized, true);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestSimulateTransactionInsufficientLamports()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Transaction/SimulateTransactionInsufficientLamportsResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Transaction/SimulateTransactionInsufficientLamportsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            string txData = "ARymmnVB6PB0x//jV2vsTFFdeOkzD0FFoQq6P\u002BwzGKlMD\u002BXLb/hWnOebNaYlg/" +
                            "\u002Bj6jdm9Fe2Sba/ACnvcv9KIA4BAAIEUy4zulRg8z2yKITZaNwcnq6G6aH8D0ITae862qbJ" +
                            "\u002B3eE3M6r5DRwldquwlqOuXDDOWZagXmbHnAU3w5Dg44kogAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qBann0itTd6uxx69h" +
                            "ION5Js4E4drRP8CWwoLTdorAFUqAICAgABDAIAAACAlpgAAAAAAAMBABVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

            RequestResult<ResponseValue<SimulationLogs>> result = sut.SimulateTransaction(txData);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result.Value);
            Assert.AreEqual(79203980UL, result.Result.Context.Slot);
            Assert.AreEqual(3, result.Result.Value.Logs.Length);
            Assert.IsNotNull(result.Result.Value.Error);
            Assert.AreEqual(TransactionErrorType.InstructionError, result.Result.Value.Error.Type);
            Assert.IsNotNull(result.Result.Value.Error.InstructionError);
            Assert.AreEqual(InstructionErrorType.Custom, result.Result.Value.Error.InstructionError.Type);
            Assert.AreEqual(1u, result.Result.Value.Error.InstructionError.CustomError);
            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}