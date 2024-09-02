using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class SolanaRpcClientFeeTest : SolanaRpcClientTestBase
    {

        [TestMethod]
        public void TestGetFeeForMessage()
        {
            var responseData = File.ReadAllText("Resources/Http/Fees/GetFeeForMessageResponse.json");
            var requestData = File.ReadAllText("Resources/Http/Fees/GetFeeForMessageRequest.json");
            var sentMessage = string.Empty;
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = TestnetUri,
            };

            var sut = new SolanaRpcClient(TestnetUrl, null, httpClient);
            var result = sut.GetFeeForMessage("AQABAu\u002BOVfa66vZfLI0xdX9GcGk/\u002BU65\u002Bdox\u002BiHABM3DOSGuBUpTWpkpIQZNJOhxYNo4fHw1td28kruB5B\u002BoQEEFRI3tj0g2caCBX14VjqrxK4Daz/4WvmWxU698Okvp8lYDjAEBACNIZWxsbyBTb2xhbmEgV29ybGQsIHVzaW5nIFNvbG5ldCA6KQ==");

            Assert.AreEqual(requestData, sentMessage);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual(132177311UL, result.Result.Context.Slot);
            Assert.AreEqual(5000UL, result.Result.Value);
            FinishTest(messageHandlerMock, TestnetUri);
        }

    }
}