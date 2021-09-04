using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Models;
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
    public class SolanaRpcClientBlockTest : SolanaRpcClientTestBase
    {
        [TestMethod]
        public void TestGetBlock()
        {

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlock(79662905);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);

            Assert.AreEqual(66130135, res.Result.BlockHeight);
            Assert.AreEqual(1622632900, res.Result.BlockTime);
            Assert.AreEqual(79662904UL, res.Result.ParentSlot);
            Assert.AreEqual("5wLhsKAH9SCPbRZc4qWf3GBiod9CD8sCEZfMiU25qW8", res.Result.Blockhash);
            Assert.AreEqual("CjJ97j84mUq3o67CEqzEkTifXpHLBCD8GvmfBYLz4Zdg", res.Result.PreviousBlockhash);

            Assert.AreEqual(1, res.Result.Rewards.Length);
            var rewards = res.Result.Rewards[0];

            Assert.AreEqual(1785000, rewards.Lamports);
            Assert.AreEqual(365762267923UL, rewards.PostBalance);
            Assert.AreEqual("9zkU8suQBdhZVax2DSGNAnyEhEzfEELvA25CJhy5uwnW", rewards.Pubkey);
            Assert.AreEqual(RewardType.Fee, rewards.RewardType);

            TransactionMetaInfo first = res.Result.Transactions[0];

            Assert.IsNotNull(first.Meta.Error);
            Assert.AreEqual(TransactionErrorType.InstructionError, first.Meta.Error.Type);
            Assert.IsNotNull(first.Meta.Error.InstructionError);
            Assert.AreEqual(InstructionErrorType.Custom, first.Meta.Error.InstructionError.Type);
            Assert.AreEqual(0, first.Meta.Error.InstructionError.CustomError);

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
            Assert.AreEqual("2Hh35eZPP1wZLYQ1HHv8PqGoRo73XirJeKFpBVc19msi6qeJHk3yUKqS1viRtqkdb545CerTWeywPFXxjKEhDWTK", first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("DjuMPGThkGdyk2vDvDDYjTFSyxzTumdapnDNbvVZbYQE", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual("2ZjTR1vUs2pHXyTLxtFDhN2tsm2HbaH36cAxzJcwaXf8y5jdTESsGNBLFaxGuWENxLa2ZL3cX9foNJcWbRq", first.Transaction.Message.Instructions[0].Data);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("D8qh6AeX4KaTe6ZBpsZDdntTQUyPy7x6Xjp7NnEigCWH", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlock()
        {
            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlockRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedBlock(79662905);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);

            Assert.AreEqual(66130135, res.Result.BlockHeight);
            Assert.AreEqual(1622632900, res.Result.BlockTime);
            Assert.AreEqual(79662904UL, res.Result.ParentSlot);
            Assert.AreEqual("5wLhsKAH9SCPbRZc4qWf3GBiod9CD8sCEZfMiU25qW8", res.Result.Blockhash);
            Assert.AreEqual("CjJ97j84mUq3o67CEqzEkTifXpHLBCD8GvmfBYLz4Zdg", res.Result.PreviousBlockhash);

            Assert.AreEqual(1, res.Result.Rewards.Length);
            var rewards = res.Result.Rewards[0];

            Assert.AreEqual(1785000, rewards.Lamports);
            Assert.AreEqual(365762267923UL, rewards.PostBalance);
            Assert.AreEqual("9zkU8suQBdhZVax2DSGNAnyEhEzfEELvA25CJhy5uwnW", rewards.Pubkey);
            Assert.AreEqual(RewardType.Fee, rewards.RewardType);

            TransactionMetaInfo first = res.Result.Transactions[0];

            Assert.IsNotNull(first.Meta.Error);
            Assert.AreEqual(TransactionErrorType.InstructionError, first.Meta.Error.Type);
            Assert.IsNotNull(first.Meta.Error.InstructionError);
            Assert.AreEqual(InstructionErrorType.Custom, first.Meta.Error.InstructionError.Type);
            Assert.AreEqual(0, first.Meta.Error.InstructionError.CustomError);

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
            Assert.AreEqual("2Hh35eZPP1wZLYQ1HHv8PqGoRo73XirJeKFpBVc19msi6qeJHk3yUKqS1viRtqkdb545CerTWeywPFXxjKEhDWTK", first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("DjuMPGThkGdyk2vDvDDYjTFSyxzTumdapnDNbvVZbYQE", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual("2ZjTR1vUs2pHXyTLxtFDhN2tsm2HbaH36cAxzJcwaXf8y5jdTESsGNBLFaxGuWENxLa2ZL3cX9foNJcWbRq", first.Transaction.Message.Instructions[0].Data);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("D8qh6AeX4KaTe6ZBpsZDdntTQUyPy7x6Xjp7NnEigCWH", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockConfirmed()
        {

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlock(79662905, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);
            // everything else was already validated above
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlockConfirmed()
        {

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlockConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedBlock(79662905, Commitment.Confirmed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(2, res.Result.Transactions.Length);
            // everything else was already validated above
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedBlockInvalid()
        {
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetBlock(79662905, Commitment.Processed);
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
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetBlock(79662905, Types.Commitment.Processed);
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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionNoArgsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionNoArgsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlockProduction();

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
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetBlockProduction(lastSlot: 1234556UL);
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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionRangeStartResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionRangeStartRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlockProduction(commitment: Commitment.Processed, firstSlot: 79714135UL);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityRangeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockProductionIdentityRangeRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", 79000000UL, 79500000UL);

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
            var responseData = File.ReadAllText("Resources/Http/Transaction/GetConfirmedTransactionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Transaction/GetConfirmedTransactionRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedTransaction("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1", Commitment.Confirmed);

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
            Assert.AreEqual("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1", first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("EvVrzsxoj118sxxSTrcnc9u3fRdQfCc7d4gRzzX6TSqj", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual("2kr3BYaDkghC7rvHsQYnBNoB4dhXrUmzgYMM4kbHSG7ALa3qsMPxfC9cJTFDKyJaC8VYSjrey9pvyRivtESUJrC3qzr89pvS2o6MQ"
                + "hyRVxmh3raQStxFFYwZ6WyKFNoQXvcchBwy8uQGfhhUqzuLNREwRmZ5U2VgTjFWX8Vikqya6iyzvALQNZEvqz7ZoGEyRtJ6AzNyWbkUyEo63rZ5w3wnxmhr3Uood",
                first.Transaction.Message.Instructions[0].Data);

            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("6XGYfEJ5CGGBA5E8E7Gw4ToyDLDNNAyUCb7CJj1rLk21", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetConfirmedTransactionProcessed()
        {

            var responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Transaction/GetConfirmedTransactionProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedTransaction("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1", Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTransaction()
        {
            var responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Transaction/GetTransactionRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetTransaction("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1");

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
            Assert.AreEqual("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1", first.Transaction.Signatures[0]);

            Assert.AreEqual(5, first.Transaction.Message.AccountKeys.Length);
            Assert.AreEqual("EvVrzsxoj118sxxSTrcnc9u3fRdQfCc7d4gRzzX6TSqj", first.Transaction.Message.AccountKeys[0]);

            Assert.AreEqual(0, first.Transaction.Message.Header.NumReadonlySignedAccounts);
            Assert.AreEqual(3, first.Transaction.Message.Header.NumReadonlyUnsignedAccounts);
            Assert.AreEqual(1, first.Transaction.Message.Header.NumRequiredSignatures);

            Assert.AreEqual(1, first.Transaction.Message.Instructions.Length);
            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].Accounts.Length);
            Assert.AreEqual("2kr3BYaDkghC7rvHsQYnBNoB4dhXrUmzgYMM4kbHSG7ALa3qsMPxfC9cJTFDKyJaC8VYSjrey9pvyRivtESUJrC3qzr89pvS2o6MQ"
                + "hyRVxmh3raQStxFFYwZ6WyKFNoQXvcchBwy8uQGfhhUqzuLNREwRmZ5U2VgTjFWX8Vikqya6iyzvALQNZEvqz7ZoGEyRtJ6AzNyWbkUyEo63rZ5w3wnxmhr3Uood",
                first.Transaction.Message.Instructions[0].Data);

            Assert.AreEqual(4, first.Transaction.Message.Instructions[0].ProgramIdIndex);

            Assert.AreEqual("6XGYfEJ5CGGBA5E8E7Gw4ToyDLDNNAyUCb7CJj1rLk21", first.Transaction.Message.RecentBlockhash);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTransactionProcessed()
        {

            var responseData = File.ReadAllText("Resources/Http/Transaction/GetTransactionResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Transaction/GetTransactionProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetTransaction("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1", Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlocks()
        {

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlocks(79_499_950, 79_500_000);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedBlocks(79_499_950, 79_500_000);

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
            var sut = new SolanaRpcClient(TestnetUrl, null);
            try
            {
                var res = sut.GetBlocks(79_499_950, 79_500_000, Commitment.Processed);
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
            var sut = new SolanaRpcClient(TestnetUrl, null);
            try
            {
                var res = sut.GetConfirmedBlocks(79_499_950, 79_500_000, Commitment.Processed);
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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlocks(79_499_950, 79_500_000, Commitment.Confirmed);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedBlocks(79_499_950, 79_500_000, Commitment.Confirmed);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlocksWithLimit(79_699_950, 2);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksWithLimitRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedBlocksWithLimit(79_699_950, 2);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetBlocksWithLimit(79_699_950, 2, Commitment.Confirmed);

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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlocksWithLimitResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetConfirmedBlocksWithLimitConfirmedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetConfirmedBlocksWithLimit(79_699_950, 2, Commitment.Confirmed);

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
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetBlocksWithLimit(79_699_950, 2, Commitment.Processed);
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
            var sut = new SolanaRpcClient(TestnetUrl, null);

            try
            {
                var res = sut.GetConfirmedBlocksWithLimit(79_699_950, 2, Commitment.Processed);
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

            var responseData = File.ReadAllText("Resources/Http/Blocks/GetFirstAvailableBlockResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetFirstAvailableBlockRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri
            };
            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var res = sut.GetFirstAvailableBlock();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(res.Result);
            Assert.AreEqual(39368303UL, res.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockHeight()
        {
            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetBlockHeight();
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1233UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockHeightConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockHeightConfirmedRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetBlockHeight(Types.Commitment.Confirmed);
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1233UL, result.Result);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetBlockCommitment()
        {
            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockCommitmentResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockCommitmentRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetBlockCommitment(78561320);

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
            var responseData = File.ReadAllText("Resources/Http/Blocks/GetBlockTimeResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Blocks/GetBlockTimeRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetBlockTime(78561320);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1621971949UL, result.Result);

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

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
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
        public void TestGetRecentBlockHashProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/GetRecentBlockhashResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetRecentBlockhashProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetRecentBlockHash(Commitment.Processed);

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