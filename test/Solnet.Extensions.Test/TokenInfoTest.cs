using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Extensions.TokenInfo;
using System.IO;

namespace Solnet.Extensions.Test
{
    [TestClass]
    public class TokenInfoTest
    {
        [TestMethod]
        public void TestTokenInfoResolverParseAndFind()
        {

            // load simple 
            var json = File.ReadAllText("Resources/TokenInfo/SimpleTokenList.json");
            ITokenInfoResolver tokens = TokenInfoResolver.ParseTokenList(json);

            // lookup by mint
            var wsol = tokens.Resolve(WellKnownTokens.WrappedSOL.TokenMint);
            Assert.IsNotNull(wsol);
            Assert.AreEqual(WellKnownTokens.WrappedSOL.Symbol, wsol.Symbol);
            Assert.AreEqual(WellKnownTokens.WrappedSOL.TokenName, wsol.TokenName);
            Assert.AreEqual(WellKnownTokens.WrappedSOL.TokenMint, wsol.TokenMint);
            Assert.AreEqual(WellKnownTokens.WrappedSOL.DecimalPlaces, wsol.DecimalPlaces);

        }

        [TestMethod]
        public void TestTokenInfoResolverUnknowns()
        {

            // load simple 
            var json = File.ReadAllText("Resources/TokenInfo/SimpleTokenList.json");
            var tokens = TokenInfoResolver.ParseTokenList(json);

            // lookup unknown mint - non-fatal - returns unknown mint
            var unknown = tokens.Resolve("deadbeef11111111111111111111111111111111112");
            Assert.IsNotNull(unknown);
            Assert.AreEqual(-1, unknown.DecimalPlaces);
            Assert.IsTrue(unknown.TokenName.IndexOf("deadbeef") >= 0);
            Assert.AreEqual("deadbeef11111111111111111111111111111111112", unknown.TokenMint);

            // repeat lookup and check we are reusing same instance
            var unknown2 = tokens.Resolve(unknown.TokenMint);
            Assert.AreSame(unknown, unknown2);

            // replace with known def
            tokens.Add(new TokenDef(unknown2.TokenMint, "Test Mint", "MINT", 4));
            var known = tokens.Resolve(unknown.TokenMint);
            Assert.IsNotNull(known);
            Assert.AreNotSame(known, unknown);
            Assert.AreEqual(4, known.DecimalPlaces);
            Assert.AreEqual("Test Mint", known.TokenName);
            Assert.AreEqual(unknown.TokenMint, known.TokenMint);

        }

    }

}
