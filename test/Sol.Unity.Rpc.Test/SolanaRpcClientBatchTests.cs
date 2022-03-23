using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sol.Unity.Programs;
using Sol.Unity.Rpc.Core.Http;
using Sol.Unity.Rpc.Messages;
using Sol.Unity.Rpc.Models;
using Sol.Unity.Rpc.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientBatchTests
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        public void TestCreateAndSerializeBatchRequest()
        {

            // compose a new batch of requests
            var unusedRpcClient = ClientFactory.GetClient(Cluster.MainNet);
            var batch = new SolanaRpcBatchWithCallbacks(unusedRpcClient);
            batch.GetBalance("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");
            batch.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey);

            batch.GetConfirmedSignaturesForAddress2("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null);
            batch.GetConfirmedSignaturesForAddress2("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null);
            batch.GetConfirmedSignaturesForAddress2("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null);

            // how many requests in batch?
            Assert.AreEqual(5, batch.Composer.Count);

            // serialize
            var reqs = batch.Composer.CreateJsonRequests();
            Assert.IsNotNull(reqs);
            Assert.AreEqual(5, reqs.Count);

            // serialize and check we're good
            var serializerOptions = CreateJsonOptions();
            var json = JsonSerializer.Serialize<JsonRpcBatchRequest>(reqs, serializerOptions);
            var expected = File.ReadAllText("Resources/Http/Batch/SampleBatchRequest.json");
            Assert.AreEqual(expected, json);

        }

        [TestMethod]
        public void TestDeserializeBatchResponse()
        {
            var responseData = File.ReadAllText("Resources/Http/Batch/SampleBatchResponse.json");
            var serializerOptions = CreateJsonOptions();
            var res = JsonSerializer.Deserialize<JsonRpcBatchResponse>(responseData, serializerOptions);
            Assert.IsNotNull(res);
            Assert.AreEqual(5, res.Count);
        }

        [TestMethod]
        public void TestCreateAndProcessBatchCallbacks()
        {

            var expected_requests = File.ReadAllText("Resources/Http/Batch/SampleBatchRequest.json");
            var expected_responses = File.ReadAllText("Resources/Http/Batch/SampleBatchResponse.json");

            ulong found_lamports = 0;
            decimal found_balance = 0M;
            int sig_callback_count = 0;

            // compose a new batch of requests
            var unusedRpcClient = ClientFactory.GetClient(Cluster.MainNet);
            var batch = new SolanaRpcBatchWithCallbacks(unusedRpcClient);
            batch.GetBalance("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",
                callback: (x, ex) => found_lamports = x.Value);
            batch.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey,
                callback: (x, ex) => found_balance = x.Value[0].Account.Data.Parsed.Info.TokenAmount.AmountDecimal);
            batch.GetConfirmedSignaturesForAddress2("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null,
                callback: (x, ex) => sig_callback_count += x.Count);
            batch.GetConfirmedSignaturesForAddress2("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null,
                callback: (x, ex) => sig_callback_count += x.Count);
            batch.GetConfirmedSignaturesForAddress2("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null,
                callback: (x, ex) => sig_callback_count += x.Count);

            // how many requests in batch?
            Assert.AreEqual(5, batch.Composer.Count);

            // serialize and check we're good
            var reqs = batch.Composer.CreateJsonRequests();
            var serializerOptions = CreateJsonOptions();
            var json = JsonSerializer.Serialize<JsonRpcBatchRequest>(reqs, serializerOptions);
            Assert.IsNotNull(reqs);
            Assert.AreEqual(5, reqs.Count);
            Assert.AreEqual(expected_requests, json);

            // fake RPC response
            var resp = CreateMockRequestResult<JsonRpcBatchResponse>(expected_requests, expected_responses, HttpStatusCode.OK);
            Assert.IsNotNull(resp.Result);
            Assert.AreEqual(5, resp.Result.Count);

            // process and invoke callbacks
            batch.Composer.ProcessBatchResponse(resp);
            Assert.AreEqual((ulong)237543960, found_lamports);
            Assert.AreEqual(12.5M, found_balance);
            Assert.AreEqual(3, sig_callback_count);

        }

        [TestMethod]
        public void TestAutoExecuteMode()
        {

            var expected_requests = File.ReadAllText("Resources/Http/Batch/SampleBatchRequest.json");
            var expected_responses = File.ReadAllText("Resources/Http/Batch/SampleBatchResponse.json");

            ulong found_lamports = 0;
            decimal found_balance = 0M;
            int sig_callback_count = 0;

            // setup mock RPC client
            var baseAddress = new Uri("https://api.mainnet-beta.solana.com");
            var mockHander = new MyMockHttpMessageHandler();
            mockHander.Add(expected_requests, expected_responses);
            var mockHttpClient = new HttpClient(mockHander) { BaseAddress = baseAddress };
            var mockRpcClient = ClientFactory.GetClient(Cluster.MainNet, null, mockHttpClient);

            // compose a new batch of requests
            var batch = new SolanaRpcBatchWithCallbacks(mockRpcClient);
            batch.AutoExecute(BatchAutoExecuteMode.ExecuteWithFatalFailure, 10);
            batch.GetBalance("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",
                callback: (x, ex) => found_lamports = x.Value);
            batch.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey,
                callback: (x, ex) => found_balance = x.Value[0].Account.Data.Parsed.Info.TokenAmount.AmountDecimal);
            batch.GetConfirmedSignaturesForAddress2("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null,
                callback: (x, ex) => sig_callback_count += x.Count);
            batch.GetConfirmedSignaturesForAddress2("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null,
                callback: (x, ex) => sig_callback_count += x.Count);
            batch.GetConfirmedSignaturesForAddress2("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null,
                callback: (x, ex) => sig_callback_count += x.Count);

            // run through any remaining requests in batch
            batch.Flush();

            // how many requests in batch? should be zero - already flushed/executed
            Assert.AreEqual(0, batch.Composer.Count);

            // assertions
            Assert.AreEqual((ulong)237543960, found_lamports);
            Assert.AreEqual(12.5M, found_balance);
            Assert.AreEqual(3, sig_callback_count);

        }

        /// <summary>
        /// Test deserialization of TransactionError
        /// </summary>
        [TestMethod]
        public void TestTransactionError_1()
        {
            var example_fail = "{'InstructionError':[0,'InvalidAccountData']}";
            example_fail = example_fail.Replace('\'', '"');

            // from json...
            var options = CreateJsonOptions();
            var obj = JsonSerializer.Deserialize<TransactionError>(example_fail, options);
            Assert.IsNotNull(obj);

            // and back again
            var json = JsonSerializer.Serialize<TransactionError>(obj, options);
            Assert.IsNotNull(json);
            Assert.AreEqual(example_fail, json);
        }

        /// <summary>
        /// Common JSON options
        /// </summary>
        /// <returns></returns>
        private JsonSerializerOptions CreateJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        /// <summary>
        /// Create a mocked RequestResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public RequestResult<T> CreateMockRequestResult<T>(string req, string resp, HttpStatusCode status)
        {
            var x = new RequestResult<T>();
            x.HttpStatusCode = status;
            x.RawRpcRequest = req;
            x.RawRpcResponse = resp;

            // deserialize resp
            if (status == HttpStatusCode.OK)
            {
                var serializerOptions = CreateJsonOptions();
                x.Result = JsonSerializer.Deserialize<T>(resp, serializerOptions);
            }

            return x;
        }

        [TestMethod]
        public void TestBatchFailed()
        {
            // we're going to simulate an RPC failure
            var expected_requests = File.ReadAllText("Resources/Http/Batch/SampleBatchRequest.json");
            var expected_responses = "BAD REQUEST";
            var exceptions_encountered = 0;

            // compose a new batch of requests
            var unusedRpcClient = ClientFactory.GetClient(Cluster.MainNet);
            var batch = new SolanaRpcBatchWithCallbacks(unusedRpcClient);
            BatchRequestException catch_for_assert = null;
            batch.GetBalance("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",
                callback: (x, ex) => exceptions_encountered += ex != null ? 1 : 0);
            batch.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey,
                callback: (x, ex) => exceptions_encountered += ex != null ? 1 : 0);
            batch.GetConfirmedSignaturesForAddress2("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null,
                callback: (x, ex) => exceptions_encountered += ex != null ? 1 : 0);
            batch.GetConfirmedSignaturesForAddress2("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null,
                callback: (x, ex) => exceptions_encountered += ex != null ? 1 : 0);
            batch.GetConfirmedSignaturesForAddress2("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null,
                callback: (x, ex) =>
                {
                    Assert.IsInstanceOfType(ex, typeof(BatchRequestException));
                    catch_for_assert = (BatchRequestException)ex;
                    exceptions_encountered += ex != null ? 1 : 0;
                });

            // how many requests in batch?
            Assert.AreEqual(5, batch.Composer.Count);

            // fake RPC response
            var resp = CreateMockRequestResult<JsonRpcBatchResponse>(expected_requests, expected_responses, HttpStatusCode.BadRequest);
            Assert.IsNotNull(resp);
            Assert.IsNull(resp.Result);
            Assert.AreEqual(expected_responses, resp.RawRpcResponse);

            // process and invoked callbacks?
            batch.Composer.ProcessBatchFailure(resp);
            Assert.AreEqual(5, exceptions_encountered);

            // exception good?
            Assert.IsNotNull(catch_for_assert);
            Assert.IsNotNull(catch_for_assert.RpcResult);
            Assert.AreEqual(expected_responses, catch_for_assert.RpcResult.RawRpcResponse);

        }

#pragma warning restore CS0618 // Type or member is obsolete
    }






    /// <summary>
    /// Mockery is afoot - preload with requests and responses
    /// </summary>
    public class MyMockHttpMessageHandler : HttpMessageHandler
    {
        private Queue<Tuple<string, string>> _queue;

        public MyMockHttpMessageHandler()
        {
            _queue = new Queue<Tuple<string, string>>();
        }

        internal void Add(string expected_requests, string expected_responses)
        {
            _queue.Enqueue(new Tuple<string, string>(expected_requests, expected_responses));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var item = _queue.Dequeue();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(item.Item2)
            };
            return await Task.FromResult(responseMessage);
        }

    }

}


