using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaRpcClientBatchTests
    {

        [TestMethod]
        public void TestCreateAndSerializeBatchRequest()
        {

            // compose a new batch of requests
            var batch = new SolanaRpcBatchWithCallbacks();
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
            var batch = new SolanaRpcBatchWithCallbacks();
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
        public void TestCreateAndProcessBatchAsyncs()
        {

            var expected_requests = File.ReadAllText("Resources/Http/Batch/SampleBatchRequest.json");
            var expected_responses = File.ReadAllText("Resources/Http/Batch/SampleBatchResponse.json");

            ulong found_lamports = 0;
            decimal found_balance = 0M;
            int sig_callback_count = 0;

            // compose a new batch of requests
            var batch = new SolanaRpcBatchWithAsyncs();
            var balance = batch.GetBalanceAsync("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");
            var tokensAccounts = batch.GetTokenAccountsByOwnerAsync("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey);
            var sigResults1 = batch.GetConfirmedSignaturesForAddress2Async("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null);
            var sigResults2 = batch.GetConfirmedSignaturesForAddress2Async("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null);
            var sigResults3 = batch.GetConfirmedSignaturesForAddress2Async("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null);

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

            // pull task results that would otherwise haved blocked
            found_lamports = balance.Result.Value;
            found_balance = tokensAccounts.Result.Value[0].Account.Data.Parsed.Info.TokenAmount.AmountDecimal;
            sig_callback_count += sigResults1.Result.Count;
            sig_callback_count += sigResults2.Result.Count;
            sig_callback_count += sigResults3.Result.Count;

            // assertions
            Assert.AreEqual((ulong)237543960, found_lamports);
            Assert.AreEqual(12.5M, found_balance);
            Assert.AreEqual(3, sig_callback_count);

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

    }
}
