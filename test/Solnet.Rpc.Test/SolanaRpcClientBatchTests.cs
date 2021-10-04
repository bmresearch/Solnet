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
            Assert.AreEqual(15, res.Count);
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
