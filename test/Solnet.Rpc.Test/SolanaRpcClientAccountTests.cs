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
    public class SolanaRpcClientAccountTest : SolanaRpcClientTestBase
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
        public void TestGetTokenAccountInfo()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetTokenAccountInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetTokenAccountInfoRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetTokenAccountInfo("FMFMUFqRsGnKm2tQzsaeytATzSG6Evna4HEbKuS6h9uk");
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(103677806UL, result.Result.Context.Slot);
            Assert.AreEqual(false, result.Result.Value.Executable);
            Assert.AreEqual(2039280UL, result.Result.Value.Lamports);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", result.Result.Value.Owner);
            Assert.AreEqual(239UL, result.Result.Value.RentEpoch);

            Assert.AreEqual("spl-token", result.Result.Value.Data.Program);
            Assert.AreEqual(165UL, result.Result.Value.Data.Space);

            Assert.AreEqual("account", result.Result.Value.Data.Parsed.Type);

            Assert.AreEqual("2v6JjYRt93Z1h8iTZavSdGdDufocHCFKT8gvHpg3GNko", result.Result.Value.Data.Parsed.Info.Mint);
            Assert.AreEqual("47vp5BqxBQoMJkitajbsZRhyAR5phW28nKPvXhFDKTFH", result.Result.Value.Data.Parsed.Info.Owner);
            Assert.AreEqual(false, result.Result.Value.Data.Parsed.Info.IsNative);
            Assert.AreEqual("initialized", result.Result.Value.Data.Parsed.Info.State);

            Assert.AreEqual("1", result.Result.Value.Data.Parsed.Info.TokenAmount.Amount);
            Assert.AreEqual(0, result.Result.Value.Data.Parsed.Info.TokenAmount.Decimals);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetTokenMintInfo()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetTokenMintInfoResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetTokenMintInfoRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetTokenMintInfo("2v6JjYRt93Z1h8iTZavSdGdDufocHCFKT8gvHpg3GNko", Commitment.Confirmed);
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(103677835UL, result.Result.Context.Slot);
            Assert.AreEqual(false, result.Result.Value.Executable);
            Assert.AreEqual(1461600UL, result.Result.Value.Lamports);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", result.Result.Value.Owner);
            Assert.AreEqual(239UL, result.Result.Value.RentEpoch);

            Assert.AreEqual("spl-token", result.Result.Value.Data.Program);
            Assert.AreEqual(82UL, result.Result.Value.Data.Space);

            Assert.AreEqual("mint", result.Result.Value.Data.Parsed.Type);

            Assert.AreEqual("Ad35ryfDYGvwGETsvkbgFoGasxdGAEtLPv8CYG3eNaMu", result.Result.Value.Data.Parsed.Info.FreezeAuthority);
            Assert.AreEqual("Ad35ryfDYGvwGETsvkbgFoGasxdGAEtLPv8CYG3eNaMu", result.Result.Value.Data.Parsed.Info.MintAuthority);
            Assert.AreEqual("1", result.Result.Value.Data.Parsed.Info.Supply);
            Assert.AreEqual(0, result.Result.Value.Data.Parsed.Info.Decimals);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetAccountInfoParsed()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoParsedResponse.json");
            var parsedJsonDataOnly = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoParsedResponseDataOnly.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetAccountInfoParsedRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetAccountInfo("2v6JjYRt93Z1h8iTZavSdGdDufocHCFKT8gvHpg3GNko", Commitment.Confirmed, BinaryEncoding.JsonParsed);
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(103659529UL, result.Result.Context.Slot);
            Assert.AreEqual(parsedJsonDataOnly, result.Result.Value.Data[0]);
            Assert.AreEqual("jsonParsed", result.Result.Value.Data[1]);
            Assert.AreEqual(false, result.Result.Value.Executable);
            Assert.AreEqual(1461600UL, result.Result.Value.Lamports);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", result.Result.Value.Owner);
            Assert.AreEqual(239UL, result.Result.Value.RentEpoch);

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
        public void TestGetProgramAccountsDataSize()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsDataSizeRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetProgramAccounts("4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T", dataSize: 500);
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
        public void TestGetProgramAccountsMemoryCompare()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsMemoryCompareRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetProgramAccounts("4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T", dataSize: 500,
                memCmpList: new List<MemCmp> { new() { Offset = 25, Bytes = "3Mc6vR" } });
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
        public void TestGetProgramAccountsProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetProgramAccountsProcessedRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetProgramAccounts("GrAkKfEpTKQuVHG2Y97Y2FF4i7y7Q5AHLK94JBy7Y5yv", Commitment.Processed);
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


        [TestMethod]
        public void TestGetMultipleAccountsConfirmed()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetMultipleAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetMultipleAccountsConfirmedRequest.json");

            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);

            var result = sut.GetMultipleAccounts(new List<string> { "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5" }, Types.Commitment.Confirmed);
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


        [TestMethod]
        public void TestGetLargestAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetLargestAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetLargestAccountsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetLargestAccounts(Rpc.Types.AccountFilterType.Circulating);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(20, result.Result.Value.Count);
            Assert.AreEqual("6caH6ayzofHnP8kcPQTEBrDPG4A2qDo1STE5xTMJ52k8", result.Result.Value[0].Address);
            Assert.AreEqual(20161157050000000UL, result.Result.Value[0].Lamports);
            Assert.AreEqual("gWgqQ4udVxE3uNxRHEwvftTHwpEmPHAd8JR9UzaHbR2", result.Result.Value[19].Address);
            Assert.AreEqual(2499999990454560UL, result.Result.Value[19].Lamports);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetLargestAccountsNonCirculatingProcessed()
        {
            var responseData = File.ReadAllText("Resources/Http/Accounts/GetLargestAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Accounts/GetLargestAccountsNonCirculatingProcessedRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetLargestAccounts(Rpc.Types.AccountFilterType.NonCirculating, Types.Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(20, result.Result.Value.Count);
            Assert.AreEqual("6caH6ayzofHnP8kcPQTEBrDPG4A2qDo1STE5xTMJ52k8", result.Result.Value[0].Address);
            Assert.AreEqual(20161157050000000UL, result.Result.Value[0].Lamports);
            Assert.AreEqual("gWgqQ4udVxE3uNxRHEwvftTHwpEmPHAd8JR9UzaHbR2", result.Result.Value[19].Address);
            Assert.AreEqual(2499999990454560UL, result.Result.Value[19].Lamports);

            FinishTest(messageHandlerMock, TestnetUri);
        }


        [TestMethod]
        public void TestGetVoteAccounts()
        {
            var responseData = File.ReadAllText("Resources/Http/GetVoteAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetVoteAccountsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetVoteAccounts();

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(1, result.Result.Current.Length);
            Assert.AreEqual(1, result.Result.Delinquent.Length);

            Assert.AreEqual(81274518UL, result.Result.Current[0].RootSlot);
            Assert.AreEqual("3ZT31jkAGhUaw8jsy4bTknwBMP8i4Eueh52By4zXcsVw", result.Result.Current[0].VotePublicKey);
            Assert.AreEqual("B97CCUW3AEZFGy6uUg6zUdnNYvnVq5VG8PUtb2HayTDD", result.Result.Current[0].NodePublicKey);
            Assert.AreEqual(42UL, result.Result.Current[0].ActivatedStake);
            Assert.AreEqual(0, result.Result.Current[0].Commission);
            Assert.AreEqual(147UL, result.Result.Current[0].LastVote);
            Assert.AreEqual(true, result.Result.Current[0].EpochVoteAccount);
            Assert.AreEqual(2, result.Result.Current[0].EpochCredits.Length);

            Assert.AreEqual(1234UL, result.Result.Delinquent[0].RootSlot);
            Assert.AreEqual("CmgCk4aMS7KW1SHX3s9K5tBJ6Yng2LBaC8MFov4wx9sm", result.Result.Delinquent[0].VotePublicKey);
            Assert.AreEqual("6ZPxeQaDo4bkZLRsdNrCzchNQr5LN9QMc9sipXv9Kw8f", result.Result.Delinquent[0].NodePublicKey);
            Assert.AreEqual(0UL, result.Result.Delinquent[0].ActivatedStake);
            Assert.AreEqual(false, result.Result.Delinquent[0].EpochVoteAccount);
            Assert.AreEqual(127UL, result.Result.Delinquent[0].Commission);
            Assert.AreEqual(0UL, result.Result.Delinquent[0].LastVote);
            Assert.AreEqual(0, result.Result.Delinquent[0].EpochCredits.Length);

            FinishTest(messageHandlerMock, TestnetUri);
        }

        [TestMethod]
        public void TestGetVoteAccountsWithConfigParams()
        {
            var responseData = File.ReadAllText("Resources/Http/GetVoteAccountsResponse.json");
            var requestData = File.ReadAllText("Resources/Http/GetVoteAccountsWithParamsRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetVoteAccounts("6ZPxeQaDo4bkZLRsdNrCzchNQr5LN9QMc9sipXv9Kw8f", Commitment.Processed);

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);

            FinishTest(messageHandlerMock, TestnetUri);
        }

    }
}