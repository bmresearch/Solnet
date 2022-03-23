using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sol.Unity.Programs.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sol.Unity.Rpc;
using Sol.Unity.Rpc.Core.Http;
using Sol.Unity.Rpc.Messages;
using Sol.Unity.Rpc.Models;
using System.IO;
using Sol.Unity.Rpc.Types;

namespace Sol.Unity.Programs.Test
{
    [TestClass]
    public class NameServiceClientTest
    {
        [TestMethod]
        public void GetAddressFromNameAsyncTest()
        {
            var rpc = new Mock<IRpcClient>();

            MockGetAccountInfo(rpc, "Resources/NameClient/GetAddressFromNameAsyncTest.txt");

            var sut = new NameServiceClient(rpc.Object);

            var test1 = sut.GetAddressFromNameAsync("bonfida.sol").Result;

            Assert.IsTrue(test1.WasSuccessful);
            Assert.AreEqual(Models.NameService.RecordType.NameRecord, test1.ParsedResult.Type);
            Assert.AreEqual(NameServiceClient.SolTLD, test1.ParsedResult.Header.ParentName);
            Assert.AreEqual("BriW4tTAiAm541uB2Fua3dUNoGayRa8Wt7pZUshUbrPB", test1.ParsedResult.Header.Owner.Key);

            Assert.AreEqual("ipfs=QmezhhSYPetm1uFgpznAcucQ9qrchChxW4HqwYRZtxJjFb", Encoding.UTF8.GetString(test1.ParsedResult.Value).Trim('\0'));
        }

        [TestMethod]
        public void GetAddressFromTwitterHandleTest()
        {
            var rpc = new Mock<IRpcClient>();

            MockGetAccountInfo(rpc, "Resources/NameClient/GetAddressFromTwitterHandleAsyncTest.txt");

            var sut = new NameServiceClient(rpc.Object);

            var test1 = sut.GetAddressFromTwitterHandleAsync("bonfida").Result;

            Assert.IsTrue(test1.WasSuccessful);
            Assert.AreEqual(Models.NameService.RecordType.TwitterRecord, test1.ParsedResult.Type);
            Assert.AreEqual(NameServiceClient.TwitterTLD, test1.ParsedResult.Header.ParentName);
            Assert.AreEqual("FidaeBkZkvDqi1GXNEwB8uWmj9Ngx2HXSS5nyGRuVFcZ", test1.ParsedResult.Header.Owner.Key);

            Assert.IsTrue(string.IsNullOrEmpty(Encoding.UTF8.GetString(test1.ParsedResult.Value).Trim('\0')));
        }

        [TestMethod]
        public void GetTwitterHandleFromAddressAsyncTest()
        {
            var rpc = new Mock<IRpcClient>();

            MockGetAccountInfo(rpc, "Resources/NameClient/GetTwitterHandleFromAddressAsyncTest.txt");

            var sut = new NameServiceClient(rpc.Object);

            var test1 = sut.GetTwitterHandleFromAddressAsync("FidaeBkZkvDqi1GXNEwB8uWmj9Ngx2HXSS5nyGRuVFcZ").Result;

            Assert.IsTrue(test1.WasSuccessful);
            Assert.AreEqual(Models.NameService.RecordType.ReverseTwitterRecord, test1.ParsedResult.Type);
            Assert.AreEqual(NameServiceClient.TwitterTLD, test1.ParsedResult.Header.ParentName);
            Assert.AreEqual(NameServiceClient.ReverseTwitterNameClass, test1.ParsedResult.Header.Class);
            Assert.AreEqual("FidaeBkZkvDqi1GXNEwB8uWmj9Ngx2HXSS5nyGRuVFcZ", test1.ParsedResult.Header.Owner.Key);
            Assert.AreEqual("bonfida", test1.ParsedResult.TwitterHandle);
        }

        private static void MockGetAccountInfo(Mock<IRpcClient> rpc, string fileName)
        {
            string payload = File.ReadAllText(fileName);

            RequestResult<ResponseValue<AccountInfo>> res = new RequestResult<ResponseValue<AccountInfo>>();
            res.WasHttpRequestSuccessful = true;
            res.WasRequestSuccessfullyHandled = true;
            res.Result = new ResponseValue<AccountInfo>();
            res.Result.Value = new();
            res.Result.Value.Data = new List<string>();
            res.Result.Value.Data.Add(payload);

            rpc.Setup(_ => _.GetAccountInfoAsync(It.IsAny<string>(), It.IsAny<Commitment>(), It.IsAny<BinaryEncoding>()))
                .Returns(() => Task.FromResult(res));
        }
    }
}
