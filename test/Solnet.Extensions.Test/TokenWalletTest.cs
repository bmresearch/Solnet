using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Messages;
using Solnet.Wallet;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Extensions.Test
{

    /// <summary>
    /// Reusing testing mock base class from SolnetRpc.Test project
    /// </summary>
    [TestClass]
    public class TokenWalletTest 
    {

        private const string MnemonicWords =
              "route clerk disease box emerge airport loud waste attitude film army tray" +
              " forward deal onion eight catalog surface unit card window walnut wealth medal";

        private const string Blockhash = "5cZja93sopRB9Bkhckj5WzCxCaVyriv2Uh5fFDPDFFfj";


        [TestMethod]
        public void TestMockJsonRpcParse()
        {
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var json = File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json");
            var result = JsonSerializer.Deserialize<JsonRpcResponse<ResponseValue<ulong>>>(json, serializerOptions);
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void TestLoadKnownMint()
        {
            // get owner
            var ownerWallet = new Wallet.Wallet(MnemonicWords);
            var signer = ownerWallet.GetAccount(1);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", signer.PublicKey.Key);

            // get mocked RPC client
            var client = new MockTokenWalletRpc();
            client.Add(File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetTokenAccountsByOwnerResponse.json"));

            // define some mints
            var tokens = new TokenInfoResolver();
            var testToken = new TokenInfo.TokenDef("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819", "TEST", "TEST", 2);
            tokens.Add(testToken);

            // load account
            var wallet = TokenWallet.Load(client, tokens, "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");

            // check wallet 
            Assert.IsNotNull(wallet);

            // check accounts
            var accounts = wallet.TokenAccounts();
            Assert.IsNotNull(accounts);

            // locate known test mint account
            var testAccounts = wallet.TokenAccounts().WithMint("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819");
            Assert.AreEqual(1, testAccounts.Count());
            Assert.AreEqual(0, testAccounts.WhichAreAssociatedTokenAccounts().Count());
            Assert.AreEqual(2, wallet.TokenAccounts().WithSymbol("TEST").First().DecimalPlaces);
            Assert.AreEqual(testToken.TokenMint, wallet.TokenAccounts().WithSymbol("TEST").First().TokenMint);
            Assert.AreEqual(testToken.Symbol, wallet.TokenAccounts().WithMint("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819").First().Symbol);
            Assert.AreEqual(10M, wallet.TokenAccounts().WithSymbol("TEST").First().BalanceDecimal);
            Assert.AreEqual((ulong)125, wallet.TokenAccounts().WithSymbol("TEST").First().ConvertDecimalToUlong(1.25M));
            Assert.AreEqual("G5SA5eMmbqSFnNZNB2fQV9ipHbh9y9KS65aZkAh9t8zv", wallet.TokenAccounts().WithSymbol("TEST").First().Address);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", wallet.TokenAccounts().WithSymbol("TEST").First().Owner);

        }


        [TestMethod]
        public void TestLoadUnknownMint()
        {
            // get owner
            var ownerWallet = new Wallet.Wallet(MnemonicWords);
            var signer = ownerWallet.GetAccount(1);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", signer.PublicKey.Key);

            // get mocked RPC client
            var client = new MockTokenWalletRpc();
            client.Add(File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetTokenAccountsByOwnerResponse.json"));

            // load account
            var tokens = new TokenInfoResolver();
            var wallet = TokenWallet.Load(client, tokens, "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");

            // check wallet 
            Assert.IsNotNull(wallet);

            // check accounts
            var accounts = wallet.TokenAccounts();
            Assert.IsNotNull(accounts);

            // locate unknown mint account
            var unknownAccounts = wallet.TokenAccounts().WithMint("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex");
            Assert.AreEqual(1, unknownAccounts.Count());
            Assert.AreEqual(0, unknownAccounts.WhichAreAssociatedTokenAccounts().Count());
            Assert.AreEqual(2, wallet.TokenAccounts().WithMint("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex").First().DecimalPlaces);
            Assert.AreEqual(10M, wallet.TokenAccounts().WithMint("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex").First().BalanceDecimal);
            Assert.AreEqual("4NSREK36nAr32vooa3L9z8tu6JWj5rY3k4KnsqTgynvm", wallet.TokenAccounts().WithMint("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex").First().Address);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", wallet.TokenAccounts().WithMint("88ocFjrLgHEMQRMwozC7NnDBQUsq2UoQaqREFZoDEex").First().Owner);

        }


        [TestMethod]
        public void TestProvisionAtaInjectBuilder()
        {
            // get owner
            var ownerWallet = new Wallet.Wallet(MnemonicWords);
            var signer = ownerWallet.GetAccount(1);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", signer.PublicKey.Key);

            // get mocked RPC client
            var client = new MockTokenWalletRpc();
            client.Add(File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetTokenAccountsByOwnerResponse.json"));

            // define some mints
            var tokens = new TokenInfoResolver();
            var testToken = new TokenInfo.TokenDef("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819", "TEST", "TEST", 2);
            tokens.Add(testToken);

            // load account
            var wallet = TokenWallet.Load(client, tokens, "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");

            // check wallet 
            Assert.IsNotNull(wallet);

            // check accounts
            var accounts = wallet.TokenAccounts();
            Assert.IsNotNull(accounts);

            // locate known test mint account
            var testAccounts = wallet.TokenAccounts().WithMint("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819");
            Assert.AreEqual(1, testAccounts.Count());
            Assert.AreEqual(0, testAccounts.WhichAreAssociatedTokenAccounts().Count());
          
            // provision the ata
            var builder = new TransactionBuilder();
            builder
                .SetFeePayer(signer)
                .SetRecentBlockHash(Blockhash);
            var before = builder.Build(signer);

            // jit should append create ata command to builder
            var testAta = wallet.JitCreateAssociatedTokenAccount(builder, testToken.TokenMint, new PublicKey("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5"));
            var after = builder.Build(signer);
            Assert.AreEqual("F6qCC87R5cmAJUKbhwERSFQHkQpSKyUkETgrjTJKB2nK", testAta.Key);
            Assert.IsTrue(after.Length > before.Length);

        }


        [TestMethod]
        public void TestLoadRefresh()
        {
            // get owner
            var ownerWallet = new Wallet.Wallet(MnemonicWords);
            var signer = ownerWallet.GetAccount(1);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", signer.PublicKey.Key);

            // get mocked RPC client
            var client = new MockTokenWalletRpc();
            client.Add(File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetTokenAccountsByOwnerResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetTokenAccountsByOwnerResponse.json"));

            // define some mints
            var tokens = new TokenInfoResolver();
            var testToken = new TokenInfo.TokenDef("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819", "TEST", "TEST", 2);
            tokens.Add(testToken);

            // load account
            var wallet = TokenWallet.Load(client, tokens, "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");

            // check wallet 
            Assert.IsNotNull(wallet);

            // refresh
            wallet.Refresh();

        }


        [TestMethod]
        public void TestProvisionAtaExecuteBuilder()
        {
            // get owner
            var ownerWallet = new Wallet.Wallet(MnemonicWords);
            var signer = ownerWallet.GetAccount(1);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", signer.PublicKey.Key);

            // get mocked RPC client
            var client = new MockTokenWalletRpc();
            client.Add(File.ReadAllText("Resources/TokenWallet/GetBalanceResponse.json"));
            client.Add(File.ReadAllText("Resources/TokenWallet/GetTokenAccountsByOwnerResponse.json"));

            // define some mints
            var tokens = new TokenInfoResolver();
            var testToken = new TokenInfo.TokenDef("98mCaWvZYTmTHmimisaAQW4WGLphN1cWhcC7KtnZF819", "TEST", "TEST", 2);
            tokens.Add(testToken);

            // load account
            var wallet = TokenWallet.Load(client, tokens, "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5");

            // check wallet 
            Assert.IsNotNull(wallet);

            // TODO - test send

        }

    }



}
