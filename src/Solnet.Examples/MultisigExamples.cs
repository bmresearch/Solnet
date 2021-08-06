using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Examples
{
    /// <summary>
    /// An example on how to use multisig accounts to control the mint of a token.
    /// </summary>
    public class CreateInitializeAndMintToMultiSigExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new (MnemonicWords);

            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            
            ulong minBalanceForExemptionMultiSig =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MultisigAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption MultiSig >> {minBalanceForExemptionMultiSig}");
            ulong minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");
            ulong minBalanceForExemptionMint =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");
            
            Account ownerAccount = wallet.GetAccount(10);
            Account mintAccount = wallet.GetAccount(94224);
            Account initialAccount = wallet.GetAccount(84224);
            
            Account multiSignature = wallet.GetAccount(2011);
            
            Account signerAccount1 = wallet.GetAccount(25100);
            Account signerAccount2 = wallet.GetAccount(25101);
            Account signerAccount3 = wallet.GetAccount(25102);
            Account signerAccount4 = wallet.GetAccount(25103);
            Account signerAccount5 = wallet.GetAccount(25104);

            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    multiSignature,
                    minBalanceForExemptionMultiSig,
                    TokenProgram.MultisigAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMultiSignature(
                    multiSignature,
                    new List<PublicKey>{signerAccount1, signerAccount2, signerAccount3, signerAccount4, signerAccount5},
                    3))
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    mintAccount.PublicKey,
                    minBalanceForExemptionMint,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    10,
                    multiSignature))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    multiSignature.Sign(msgData),
                    mintAccount.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);
            
            string createMultiSigAndMintSignature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(createMultiSigAndMintSignature);
            
            blockHash = rpcClient.GetRecentBlockHash();

            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    multiSignature,
                    new List<PublicKey>
                    {
                        signerAccount1,
                        signerAccount2,
                        signerAccount4
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);
            
            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    initialAccount.Sign(msgData),
                    signerAccount1.Sign(msgData),
                    signerAccount2.Sign(msgData),
                    signerAccount4.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);
            
            string mintToSignature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(mintToSignature);
        }
    }
}