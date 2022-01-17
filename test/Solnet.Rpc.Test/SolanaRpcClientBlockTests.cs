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
    public class SolanaRpcClientBlockTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetBlock()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<BlockInfo> res = sut.GetBlock(79662905);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);

            Assert.AreEqual(66130135, res.Result.BlockHeight);
            Assert.AreEqual(1622632900, res.Result.BlockTime);
            Assert.AreEqual(79662904UL, res.Result.ParentSlot);
            Assert.AreEqual("5wLhsKAH9SCPbRZc4qWf3GBiod9CD8sCEZfMiU25qW8", res.Result.Blockhash);
            Assert.AreEqual("CjJ97j84mUq3o67CEqzEkTifXpHLBCD8GvmfBYLz4Zdg", res.Result.PreviousBlockhash);

            Assert.AreEqual(1, res.Result.Rewards.Length);
            RewardInfo rewards = res.Result.Rewards[0];

            Assert.AreEqual(1785000, rewards.Lamports);
            Assert.AreEqual(365762267923UL, rewards.PostBalance);
            Assert.AreEqual("9zkU8suQBdhZVax2DSGNAnyEhEzfEELvA25CJhy5uwnW", rewards.Pubkey);
            Assert.AreEqual(RewardType.Fee, rewards.RewardType);

            TransactionMetaInfo first = res.Result.Transactions[0];

            Assert.IsNotNull(first.Meta.Error);
            Assert.AreEqual(TransactionErrorType.InstructionError, first.Meta.Error.Type);
            Assert.IsNotNull(first.Meta.Error.InstructionError);
            Assert.AreEqual(InstructionErrorType.Custom, first.Meta.Error.InstructionError.Type);
            Assert.AreEqual(0u, first.Meta.Error.InstructionError.CustomError);

            Assert.AreEqual(5000UL, first.Meta.Fee);
            Assert.AreEqual(0, first.Meta.InnerInstructions.Length);
            Assert.AreEqual(2, first.Meta.LogMessages.Length);
            Assert.AreEqual(5, first.Meta.PostBalances.Length);
            Assert.AreEqual(35132731759UL, first.Meta.PostBalances[0]);
            Assert.AreEqual(5, first.Meta.PreBalances.Length);
            Assert.AreEqual(35132736759UL, first.Meta.PreBalances[0]);
            Assert.AreEqual(0, first.Meta.PostTokenBalances.Length);
            Assert.AreEqual(0, first.Meta.PreTokenBalances.Length);

            Assert.AreEqual(1, first.Transaction.Signatures.Length);
            Assert.AreEqual("2Hh35eZPP1wZLYQ1HHv8PqGoRo73XirJeKFpBVc19msi6qeJHk3yUKqS1viRtqkdb545CerTWeywPFXxjKEhDWTK",
                first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("DjuMPGThkGdyk2vDvDDYjTFSyxzTumdapnDNbvVZbYQE", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual("2ZjTR1vUs2pHXyTLxtFDhN2tsm2HbaH36cAxzJcwaXf8y5jdTESsGNBLFaxGuWENxLa2ZL3cX9foNJcWbRq",
                first.Transaction.Message.Instructions[0].Data);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("D8qh6AeX4KaTe6ZBpsZDdntTQUyPy7x6Xjp7NnEigCWH", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlock()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlockRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<BlockInfo> res = sut.GetConfirmedBlock(79662905);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);

            Assert.AreEqual(66130135, res.Result.BlockHeight);
            Assert.AreEqual(1622632900, res.Result.BlockTime);
            Assert.AreEqual(79662904UL, res.Result.ParentSlot);
            Assert.AreEqual("5wLhsKAH9SCPbRZc4qWf3GBiod9CD8sCEZfMiU25qW8", res.Result.Blockhash);
            Assert.AreEqual("CjJ97j84mUq3o67CEqzEkTifXpHLBCD8GvmfBYLz4Zdg", res.Result.PreviousBlockhash);

            Assert.AreEqual(1, res.Result.Rewards.Length);
            RewardInfo rewards = res.Result.Rewards[0];

            Assert.AreEqual(1785000, rewards.Lamports);
            Assert.AreEqual(365762267923UL, rewards.PostBalance);
            Assert.AreEqual("9zkU8suQBdhZVax2DSGNAnyEhEzfEELvA25CJhy5uwnW", rewards.Pubkey);
            Assert.AreEqual(RewardType.Fee, rewards.RewardType);

            TransactionMetaInfo first = res.Result.Transactions[0];

            Assert.IsNotNull(first.Meta.Error);
            Assert.AreEqual(TransactionErrorType.InstructionError, first.Meta.Error.Type);
            Assert.IsNotNull(first.Meta.Error.InstructionError);
            Assert.AreEqual(InstructionErrorType.Custom, first.Meta.Error.InstructionError.Type);
            Assert.AreEqual(0u, first.Meta.Error.InstructionError.CustomError);

            Assert.AreEqual(5000UL, first.Meta.Fee);
            Assert.AreEqual(0, first.Meta.InnerInstructions.Length);
            Assert.AreEqual(2, first.Meta.LogMessages.Length);
            Assert.AreEqual(5, first.Meta.PostBalances.Length);
            Assert.AreEqual(35132731759UL, first.Meta.PostBalances[0]);
            Assert.AreEqual(5, first.Meta.PreBalances.Length);
            Assert.AreEqual(35132736759UL, first.Meta.PreBalances[0]);
            Assert.AreEqual(0, first.Meta.PostTokenBalances.Length);
            Assert.AreEqual(0, first.Meta.PreTokenBalances.Length);

            Assert.AreEqual(1, first.Transaction.Signatures.Length);
            Assert.AreEqual("2Hh35eZPP1wZLYQ1HHv8PqGoRo73XirJeKFpBVc19msi6qeJHk3yUKqS1viRtqkdb545CerTWeywPFXxjKEhDWTK",
                first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("DjuMPGThkGdyk2vDvDDYjTFSyxzTumdapnDNbvVZbYQE", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual("2ZjTR1vUs2pHXyTLxtFDhN2tsm2HbaH36cAxzJcwaXf8y5jdTESsGNBLFaxGuWENxLa2ZL3cX9foNJcWbRq",
                first.Transaction.Message.Instructions[0].Data);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("D8qh6AeX4KaTe6ZBpsZDdntTQUyPy7x6Xjp7NnEigCWH", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<BlockInfo> res = sut.GetBlock(79662905, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);
            // everything else was already validated above
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlockConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlockConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<BlockInfo> res = sut.GetConfirmedBlock(79662905, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);
            // everything else was already validated above
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlockInvalid()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<BlockInfo> res = sut.GetBlock(79662905, Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetBlockInvalid()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<BlockInfo> res = sut.GetBlock(79662905, Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetBlockProductionNoArgs()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionNoArgsResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionNoArgsRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ResponseValue<BlockProductionInfo>> res = sut.GetBlockProduction();

            Assert.AreEqual(requestData, sentMessage);

            Assert.AreEqual(3, res.Result.Value.ByIdentity.Count);
            Assert.AreEqual(79580256UL, res.Result.Value.Range.FirstSlot);
            Assert.AreEqual(79712285UL, res.Result.Value.Range.LastSlot);

            Assert.IsTrue(res.Result.Value.ByIdentity.ContainsKey("121cur1YFVPZSoKQGNyjNr9sZZRa3eX2bSuYjXHtKD6"));
            Assert.AreEqual(60, res.Result.Value.ByIdentity["121cur1YFVPZSoKQGNyjNr9sZZRa3eX2bSuYjXHtKD6"][0]);


            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockProductionInvalidCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<ResponseValue<BlockProductionInfo>> res = sut.GetBlockProduction(lastSlot: 1234556UL);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetBlockProductionIdentity()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ResponseValue<BlockProductionInfo>> res =
                sut.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

            Assert.AreEqual(requestData, sentMessage);

            Assert.AreEqual(1, res.Result.Value.ByIdentity.Count);
            Assert.AreEqual(79580256UL, res.Result.Value.Range.FirstSlot);
            Assert.AreEqual(79712285UL, res.Result.Value.Range.LastSlot);

            Assert.IsTrue(res.Result.Value.ByIdentity.ContainsKey("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu"));
            Assert.AreEqual(96, res.Result.Value.ByIdentity["Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu"][0]);


            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockProductionRangeStart()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionRangeStartResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionRangeStartRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ResponseValue<BlockProductionInfo>> res =
                sut.GetBlockProduction(commitment: Commitment.Processed, firstSlot: 79714135UL);

            Assert.AreEqual(requestData, sentMessage);

            Assert.AreEqual(35, res.Result.Value.ByIdentity.Count);
            Assert.AreEqual(79714135UL, res.Result.Value.Range.FirstSlot);
            Assert.AreEqual(79714275UL, res.Result.Value.Range.LastSlot);

            Assert.IsTrue(res.Result.Value.ByIdentity.ContainsKey("123vij84ecQEKUvQ7gYMKxKwKF6PbYSzCzzURYA4xULY"));
            Assert.AreEqual(4, res.Result.Value.ByIdentity["123vij84ecQEKUvQ7gYMKxKwKF6PbYSzCzzURYA4xULY"][0]);
            Assert.AreEqual(3, res.Result.Value.ByIdentity["123vij84ecQEKUvQ7gYMKxKwKF6PbYSzCzzURYA4xULY"][1]);


            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockProductionIdentityRange()
        {
            string responseData =
                File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityRangeResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityRangeRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ResponseValue<BlockProductionInfo>> res =
                sut.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", 79000000UL, 79500000UL);

            Assert.AreEqual(requestData, sentMessage);

            Assert.AreEqual(1, res.Result.Value.ByIdentity.Count);
            Assert.AreEqual(79000000UL, res.Result.Value.Range.FirstSlot);
            Assert.AreEqual(79500000UL, res.Result.Value.Range.LastSlot);

            Assert.IsTrue(res.Result.Value.ByIdentity.ContainsKey("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu"));
            Assert.AreEqual(416, res.Result.Value.ByIdentity["Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu"][0]);
            Assert.AreEqual(341, res.Result.Value.ByIdentity["Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu"][1]);


            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedTransaction()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/GetConfirmedTransactionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/GetConfirmedTransactionRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<TransactionMetaSlotInfo> res = sut.GetConfirmedTransaction(
                "5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1",
                Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(79700345UL, res.Result.Slot);

            Assert.AreEqual(1622655364, res.Result.BlockTime);

            TransactionMetaInfo first = res.Result;

            Assert.IsNull(first.Meta.Error);

            Assert.AreEqual(5000UL, first.Meta.Fee);
            Assert.AreEqual(0, first.Meta.InnerInstructions.Length);
            Assert.AreEqual(2, first.Meta.LogMessages.Length);
            Assert.AreEqual(5, first.Meta.PostBalances.Length);
            Assert.AreEqual(395383573380UL, first.Meta.PostBalances[0]);
            Assert.AreEqual(5, first.Meta.PreBalances.Length);
            Assert.AreEqual(395383578380UL, first.Meta.PreBalances[0]);
            Assert.AreEqual(0, first.Meta.PostTokenBalances.Length);
            Assert.AreEqual(0, first.Meta.PreTokenBalances.Length);

            Assert.AreEqual(1, first.Transaction.Signatures.Length);
            Assert.AreEqual("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1",
                first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("EvVrzsxoj118sxxSTrcnc9u3fRdQfCc7d4gRzzX6TSqj", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual(
                "2kr3BYaDkghC7rvHsQYnBNoB4dhXrUmzgYMM4kbHSG7ALa3qsMPxfC9cJTFDKyJaC8VYSjrey9pvyRivtESUJrC3qzr89pvS2o6MQ"
                + "hyRVxmh3raQStxFFYwZ6WyKFNoQXvcchBwy8uQGfhhUqzuLNREwRmZ5U2VgTjFWX8Vikqya6iyzvALQNZEvqz7ZoGEyRtJ6AzNyWbkUyEo63rZ5w3wnxmhr3Uood",
                first.Transaction.Message.Instructions[0].Data);

            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("6XGYfEJ5CGGBA5E8E7Gw4ToyDLDNNAyUCb7CJj1rLk21", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedTransactionProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Transaction/GetConfirmedTransactionProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<TransactionMetaSlotInfo> res = sut.GetConfirmedTransaction(
                "5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1",
                Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTransaction()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/GetTransactionRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<TransactionMetaSlotInfo> res =
                sut.GetTransaction(
                    "5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(79700345UL, res.Result.Slot);

            Assert.AreEqual(1622655364, res.Result.BlockTime);

            TransactionMetaInfo first = res.Result;

            Assert.IsNull(first.Meta.Error);

            Assert.AreEqual(5000UL, first.Meta.Fee);
            Assert.AreEqual(0, first.Meta.InnerInstructions.Length);
            Assert.AreEqual(2, first.Meta.LogMessages.Length);
            Assert.AreEqual(5, first.Meta.PostBalances.Length);
            Assert.AreEqual(395383573380UL, first.Meta.PostBalances[0]);
            Assert.AreEqual(5, first.Meta.PreBalances.Length);
            Assert.AreEqual(395383578380UL, first.Meta.PreBalances[0]);
            Assert.AreEqual(0, first.Meta.PostTokenBalances.Length);
            Assert.AreEqual(0, first.Meta.PreTokenBalances.Length);

            Assert.AreEqual(1, first.Transaction.Signatures.Length);
            Assert.AreEqual("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1",
                first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("EvVrzsxoj118sxxSTrcnc9u3fRdQfCc7d4gRzzX6TSqj", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual(
                "2kr3BYaDkghC7rvHsQYnBNoB4dhXrUmzgYMM4kbHSG7ALa3qsMPxfC9cJTFDKyJaC8VYSjrey9pvyRivtESUJrC3qzr89pvS2o6MQ"
                + "hyRVxmh3raQStxFFYwZ6WyKFNoQXvcchBwy8uQGfhhUqzuLNREwRmZ5U2VgTjFWX8Vikqya6iyzvALQNZEvqz7ZoGEyRtJ6AzNyWbkUyEo63rZ5w3wnxmhr3Uood",
                first.Transaction.Message.Instructions[0].Data);

            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("6XGYfEJ5CGGBA5E8E7Gw4ToyDLDNNAyUCb7CJj1rLk21", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTransactionProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Transaction/GetTransactionProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<TransactionMetaSlotInfo> res = sut.GetTransaction(
                "5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1",
                Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlocks()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetBlocks(79_499_950, 79_500_000);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(39, res.Result.Count);
            Assert.AreEqual(79499950UL, res.Result[0]);
            Assert.AreEqual(79500000UL, res.Result[38]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlocks()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetConfirmedBlocks(79_499_950, 79_500_000);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(39, res.Result.Count);
            Assert.AreEqual(79499950UL, res.Result[0]);
            Assert.AreEqual(79500000UL, res.Result[38]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlocksInvalidCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);
            try
            {
                RequestResult<List<ulong>> res = sut.GetBlocks(79_499_950, 79_500_000, Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetConfirmedBlocksInvalidCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);
            try
            {
                RequestResult<List<ulong>> res = sut.GetConfirmedBlocks(79_499_950, 79_500_000, Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetBlocksConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetBlocks(79_499_950, 79_500_000, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(39, res.Result.Count);
            Assert.AreEqual(79499950UL, res.Result[0]);
            Assert.AreEqual(79500000UL, res.Result[38]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlocksConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetConfirmedBlocks(79_499_950, 79_500_000, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(39, res.Result.Count);
            Assert.AreEqual(79499950UL, res.Result[0]);
            Assert.AreEqual(79500000UL, res.Result[38]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlocksWithLimit()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetBlocksWithLimit(79_699_950, 2);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Count);
            Assert.AreEqual(79699950UL, res.Result[0]);
            Assert.AreEqual(79699951UL, res.Result[1]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlocksWithLimit()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksWithLimitRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetConfirmedBlocksWithLimit(79_699_950, 2);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Count);
            Assert.AreEqual(79699950UL, res.Result[0]);
            Assert.AreEqual(79699951UL, res.Result[1]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlocksWithLimitConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetBlocksWithLimit(79_699_950, 2, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Count);
            Assert.AreEqual(79699950UL, res.Result[0]);
            Assert.AreEqual(79699951UL, res.Result[1]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlocksWithLimitConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            string requestData =
                File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksWithLimitConfirmedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<List<ulong>> res = sut.GetConfirmedBlocksWithLimit(79_699_950, 2, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Count);
            Assert.AreEqual(79699950UL, res.Result[0]);
            Assert.AreEqual(79699951UL, res.Result[1]);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlocksWithLimitBadCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<List<ulong>> res = sut.GetBlocksWithLimit(79_699_950, 2, Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetConfirmedBlocksWithLimitBadCommitment()
        {
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                RequestResult<List<ulong>> res = sut.GetConfirmedBlocksWithLimit(79_699_950, 2, Commitment.Processed);
                Assert.Fail("Should throw exception before here.");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentException), e.InnerException.GetType());
            }
        }

        [TestMethod]
        public void TestGetFirstAvailableBlock()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetFirstAvailableBlockResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetFirstAvailableBlockRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};
            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ulong> res = sut.GetFirstAvailableBlock();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(39368303UL, res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockHeight()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightRequest.json");

            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ulong> result = sut.GetBlockHeight();
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1233UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockHeightConfirmed()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightConfirmedRequest.json");

            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            RequestResult<ulong> result = sut.GetBlockHeight(Commitment.Confirmed);
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1233UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockCommitment()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockCommitmentResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockCommitmentRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<BlockCommitment> result = sut.GetBlockCommitment(78561320);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(null, result.Result.Commitment);
            Assert.AreEqual(78380558524696194UL, result.Result.TotalStake);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockTime()
        {
            string responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockTimeResponse.json");
            string requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockTimeRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ulong> result = sut.GetBlockTime(78561320);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1621971949UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetRecentBlockHash()
        {
            string responseData = File.ReadAllText("Resources/Http/GetRecentBlockhashResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetRecentBlockhashRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<BlockHash>> result = sut.GetRecentBlockHash();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79206433UL, result.Result.Context.Slot);
            Assert.AreEqual("FJGZeJiYkwCZCnFbGujHxfVGnFgrgZiomczHr247Tn2p", result.Result.Value.Blockhash);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetRecentBlockHashProcessed()
        {
            string responseData = File.ReadAllText("Resources/Http/GetRecentBlockhashResponse.json");
            string requestData = File.ReadAllText("Resources/Http/GetRecentBlockhashProcessedRequest.json");
            string sentMessage = string.Empty;
            Mock<HttpMessageHandler> messageHandlerMock = SetupTest(
                s => sentMessage = s, responseData);

            HttpClient httpClient = new HttpClient(messageHandlerMock.Object) {BaseAddress = TestnetUri};

            SolanaRpcClient sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            RequestResult<ResponseValue<BlockHash>> result = sut.GetRecentBlockHash(Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(79206433UL, result.Result.Context.Slot);
            Assert.AreEqual("FJGZeJiYkwCZCnFbGujHxfVGnFgrgZiomczHr247Tn2p", result.Result.Value.Blockhash);
            Assert.AreEqual(5000UL, result.Result.Value.FeeCalculator.LamportsPerSignature);

            FinishTest(messageHandlerMock, TestnetUri);
        }
    }
}