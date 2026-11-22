using Solnet.Programs;
using Solnet.Programs.Models.Token2022;
using Solnet.Wallet;
using System;

namespace Solnet.Examples
{
    /// <summary>An offline example of extension sizing, configuration instructions, and integer fee calculations. It does not submit transactions.</summary>
    public class Token2022ExtensionsExample : IExample
    {
        /// <summary>Prints example allocation sizes and instructions. Allocate the mint and initialize these extensions before InitializeMint2 in a funded transaction.</summary>
        public void Run()
        {
            var mint = new Account();
            var authority = new Account();
            var extensions = new[] { Token2022ExtensionType.TransferFeeConfig, Token2022ExtensionType.PermanentDelegate };
            int mintSize = Token2022Program.GetMintAccountDataSize(extensions);
            int accountSize = Token2022Program.GetTokenAccountDataSize(Token2022Program.GetRequiredAccountExtensions(extensions));
            var initializeFee = Token2022Program.InitializeTransferFeeConfig(mint.PublicKey, authority.PublicKey,
                authority.PublicKey, basisPoints: 100, maximumFee: 1_000_000);
            var initializeDelegate = Token2022Program.InitializePermanentDelegate(mint.PublicKey, authority.PublicKey);
            var fee = new TransferFee(Epoch: 0, MaximumFee: 1_000_000, BasisPoints: 100);

            Console.WriteLine($"Token-2022 mint: {mint.PublicKey}");
            Console.WriteLine($"Allocate mint: {mintSize} bytes; token account: {accountSize} bytes.");
            Console.WriteLine($"Fee configuration instruction: {Convert.ToBase64String(initializeFee.Data)}");
            Console.WriteLine($"Permanent delegate instruction: {Convert.ToBase64String(initializeDelegate.Data)}");
            Console.WriteLine($"Fee for 10,001 raw token units: {fee.CalculateFee(10_001)}");
            Console.WriteLine("This is an offline example. Fund and allocate the mint, initialize its extensions, then initialize the mint in one transaction.");
        }
    }
}
