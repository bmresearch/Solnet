using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Extensions.TokenInfo;
using Solnet.Wallet;
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

        [TestMethod]
        public void TestTokenDefCreateQuantity()
        {
            var qty = WellKnownTokens.USDC.CreateQuantity(4741784U);
            Assert.AreEqual(4741784U, qty.BalanceRaw);
            Assert.AreEqual(4.741784M, qty.BalanceDecimal);
            Assert.AreEqual("USDC", qty.Symbol);
            Assert.AreEqual(6, qty.DecimalPlaces);
            Assert.AreEqual("4.741784 USDC (USD Coin)", qty.ToString());

            var dec = WellKnownTokens.Raydium.ConvertUlongToDecimal(123456U);
            Assert.AreEqual(0.123456M, dec);

            var raw = WellKnownTokens.Raydium.ConvertDecimalToUlong(1.23M);
            Assert.AreEqual(1230000U, raw);

        }

        [TestMethod]
        public void TestDynamicTokenDefCreateQuantity()
        {
            var pubkey = new PublicKey("FakekjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");

            var resolver = new TokenInfoResolver();
            resolver.Add(new TokenDef(pubkey.Key, "Fake Coin", "FK", 3));

            var qty = resolver.Resolve(pubkey.Key).CreateQuantity(4741784U);
            Assert.AreEqual(pubkey.Key, qty.TokenMint);
            Assert.AreEqual(4741784U, qty.BalanceRaw);
            Assert.AreEqual(4741.784M, qty.BalanceDecimal);
            Assert.AreEqual("FK", qty.Symbol);
            Assert.AreEqual(3, qty.DecimalPlaces);
            Assert.AreEqual("4741.784 FK (Fake Coin)", qty.ToString());

        }

    }

}
