using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var batch = new SolanaRpcBatchComposer();
            batch.GetBalance("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");
            batch.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey);
            batch.GetConfirmedSignaturesForAddress2("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null);
            batch.GetConfirmedSignaturesForAddress2("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null);
            batch.GetConfirmedSignaturesForAddress2("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null);

            // how many requests in batch?
            Assert.AreEqual(5, batch.Count);

            // serialize
            var reqs = batch.CreateJsonRequests();
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
            var batch = new SolanaRpcBatchComposer();
            batch.GetBalance("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",
                callback: x => found_lamports = x.Value);
            batch.GetTokenAccountsByOwner("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", null, TokenProgram.ProgramIdKey,
                callback: x => found_balance = x.Value[0].Account.Data.Parsed.Info.TokenAmount.AmountDecimal);
            batch.GetConfirmedSignaturesForAddress2("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", 200, null, null,
                callback: x => sig_callback_count++);
            batch.GetConfirmedSignaturesForAddress2("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex", 200, null, null,
                callback: x => sig_callback_count++);
            batch.GetConfirmedSignaturesForAddress2("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", 200, null, null,
                callback: x => sig_callback_count++);

            // how many requests in batch?
            Assert.AreEqual(5, batch.Count);

            // serialize and check we're good
            var reqs = batch.CreateJsonRequests();
            var serializerOptions = CreateJsonOptions();
            var json = JsonSerializer.Serialize<JsonRpcBatchRequest>(reqs, serializerOptions);
            Assert.IsNotNull(reqs);
            Assert.AreEqual(5, reqs.Count);
            Assert.AreEqual(expected_requests, json);

            // fake response
            var resps = JsonSerializer.Deserialize<JsonRpcBatchResponse>(expected_responses, serializerOptions);
            Assert.IsNotNull(resps);
            Assert.AreEqual(5, resps.Count);

            // process and invoke callbacks
            batch.ProcessBatchResponse(resps);
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

    }
}
