using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Extensions.TokenMint;
using Solnet.Wallet;
using System.IO;

namespace Solnet.Extensions.Test
{
    [TestClass]
    public class TokenMintTest
    {
        [TestMethod]
        public void TestTokenInfoResolverParseAndFind()
        {

            // load simple 
            var json = File.ReadAllText("Resources/TokenMint/SimpleTokenList.json");
            ITokenMintResolver tokens = TokenMintResolver.ParseTokenList(json);

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
            var json = File.ReadAllText("Resources/TokenMint/SimpleTokenList.json");
            var tokens = TokenMintResolver.ParseTokenList(json);

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
            var qty = WellKnownTokens.USDC.CreateQuantityWithRaw(4741784U);
            Assert.AreEqual(4741784U, qty.QuantityRaw);
            Assert.AreEqual(4.741784M, qty.QuantityDecimal);
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

            var resolver = new TokenMintResolver();
            resolver.Add(new TokenDef(pubkey.Key, "Fake Coin", "FK", 3));

            // create via ulong
            var qty = resolver.Resolve(pubkey.Key).CreateQuantityWithRaw(4741784U);
            Assert.AreEqual(pubkey.Key, qty.TokenMint);
            Assert.AreEqual(4741784U, qty.QuantityRaw);
            Assert.AreEqual(4741.784M, qty.QuantityDecimal);
            Assert.AreEqual("FK", qty.Symbol);
            Assert.AreEqual(3, qty.DecimalPlaces);
            Assert.AreEqual("4741.784 FK (Fake Coin)", qty.ToString());

            // create via decimal
            qty = resolver.Resolve(pubkey.Key).CreateQuantityWithDecimal(14741.784M);
            Assert.AreEqual(pubkey.Key, qty.TokenMint);
            Assert.AreEqual(14741784U, qty.QuantityRaw);
            Assert.AreEqual(14741.784M, qty.QuantityDecimal);
            Assert.AreEqual("FK", qty.Symbol);
            Assert.AreEqual(3, qty.DecimalPlaces);
            Assert.AreEqual("14741.784 FK (Fake Coin)", qty.ToString());

        }

        [TestMethod]
        public void TestPreloadedMintResolver()
        {
            var tokens = WellKnownTokens.CreateTokenMintResolver();
            var cope = tokens.Resolve("8HGyAAB1yoM1ttS7pXjHMa3dukTFGQggnFFH3hJZgzQh"); // COPE 
            Assert.AreEqual(6, cope.DecimalPlaces);
        }

        [TestMethod]
        public void TestExtendedTokenMeta() 
        {
            // load simple 
            var json = File.ReadAllText("Resources/TokenMint/SimpleTokenList.json");
            var tokens = TokenMintResolver.ParseTokenList(json);

            // lookup USDC using mint 
            var usdc = tokens.Resolve("EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v");
            Assert.IsNotNull(usdc);
            Assert.AreEqual(6, usdc.DecimalPlaces);
            Assert.AreEqual("EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v", usdc.TokenMint);
            Assert.AreEqual("usd-coin", usdc.CoinGeckoId);
            Assert.AreEqual("https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v/logo.png", usdc.TokenLogoUrl);
            Assert.AreEqual("https://www.centre.io/", usdc.TokenProjectUrl);

        }


    }

}
