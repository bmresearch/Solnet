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
            Wallet.Wallet wallet = new(MnemonicWords);

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
                    new List<PublicKey> { signerAccount1, signerAccount2, signerAccount3, signerAccount4, signerAccount5 },
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

    /// <summary>
    /// An example on how to use multisig accounts to control the mint of a token.
    /// </summary>
    public class MintToCheckedMultisigExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

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
            Account signerAccount4 = wallet.GetAccount(25103);

            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.MintToChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    multiSignature,
                    25000,
                    10,
                    new List<PublicKey>
                    {
                        signerAccount1,
                        signerAccount2,
                        signerAccount4
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    signerAccount1.Sign(msgData),
                    signerAccount2.Sign(msgData),
                    signerAccount4.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);

            string mintToSignature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(mintToSignature);
        }
    }

    /// <summary>
    /// An example on how to use multisig accounts to control a token account.
    /// </summary>
    public class TransferCheckedMultiSigExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

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

            Account tokenAccountWithMultisigOwner = wallet.GetAccount(3042);
            Account tokenMultiSignature = wallet.GetAccount(3043);

            // The signers for the token account
            Account tokenAccountSigner1 = wallet.GetAccount(25280);
            Account tokenAccountSigner2 = wallet.GetAccount(25281);
            Account tokenAccountSigner3 = wallet.GetAccount(25282);
            Account tokenAccountSigner4 = wallet.GetAccount(25283);
            Account tokenAccountSigner5 = wallet.GetAccount(25284);

            // First we create a multi sig account to use as the token account authority
            // In this same transaction we transfer tokens using TransferChecked from the initialAccount in the example above
            // to the same token account we just finished creating
            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    tokenMultiSignature,
                    minBalanceForExemptionMultiSig,
                    TokenProgram.MultisigAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMultiSignature(
                    tokenMultiSignature,
                    new List<PublicKey>
                    {
                        tokenAccountSigner1,
                        tokenAccountSigner2,
                        tokenAccountSigner3,
                        tokenAccountSigner4,
                        tokenAccountSigner5
                    },
                    3))
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    tokenAccountWithMultisigOwner,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                    tokenAccountWithMultisigOwner,
                    mintAccount,
                    tokenMultiSignature))
                .AddInstruction(TokenProgram.TransferChecked(
                    initialAccount,
                    tokenAccountWithMultisigOwner,
                    10000, 10,
                    ownerAccount,
                    mintAccount))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    tokenMultiSignature.Sign(msgData),
                    tokenAccountWithMultisigOwner.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);

            string signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            // After the previous transaction is confirmed we use TransferChecked to transfer tokens using the
            // multi sig account back to the initial account
            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.Transfer(
                    tokenAccountWithMultisigOwner,
                    initialAccount,
                    10000,
                    tokenMultiSignature,
                    new List<PublicKey>()
                    {
                        tokenAccountSigner3,
                        tokenAccountSigner4,
                        tokenAccountSigner5
                    })).CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    tokenAccountSigner3.Sign(msgData),
                    tokenAccountSigner4.Sign(msgData),
                    tokenAccountSigner5.Sign(msgData)
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);
        }
    }

    /// <summary>
    /// An example on how to control the freeze authority of a token using multi signatures
    /// </summary>
    public class FreezeAuthorityExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

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
            Account mintAccount = wallet.GetAccount(94330);
            Account initialAccount = wallet.GetAccount(84330);

            // the signers for the token mint
            Account mintMultiSignature = wallet.GetAccount(10116);
            Account mintSigner1 = wallet.GetAccount(251280);
            Account mintSigner2 = wallet.GetAccount(251281);
            Account mintSigner3 = wallet.GetAccount(251282);
            Account mintSigner4 = wallet.GetAccount(251283);
            Account mintSigner5 = wallet.GetAccount(251284);

            // The signers for the freeze account
            Account freezeMultiSignature = wallet.GetAccount(3057);
            Account freezeSigner1 = wallet.GetAccount(25410);
            Account freezeSigner2 = wallet.GetAccount(25411);
            Account freezeSigner3 = wallet.GetAccount(25412);
            Account freezeSigner4 = wallet.GetAccount(25413);
            Account freezeSigner5 = wallet.GetAccount(25414);

            // First we create a multi sig account to use as the token's freeze authority
            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    freezeMultiSignature,
                    minBalanceForExemptionMultiSig,
                    TokenProgram.MultisigAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMultiSignature(
                    freezeMultiSignature,
                    new List<PublicKey>
                    {
                        freezeSigner1,
                        freezeSigner2,
                        freezeSigner3,
                        freezeSigner4,
                        freezeSigner5
                    }, 3))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    freezeMultiSignature.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);

            string signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();


            // Then we create an account which will be the token's mint authority
            // In this same transaction we initialize the token mint with said authorities
            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintMultiSignature,
                    minBalanceForExemptionMultiSig,
                    TokenProgram.MultisigAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMultiSignature(
                    mintMultiSignature,
                    new List<PublicKey>
                    {
                        mintSigner1,
                        mintSigner2,
                        mintSigner3,
                        mintSigner4,
                        mintSigner5
                    }, 3))
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    minBalanceForExemptionMint,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMint(
                    mintAccount,
                    10,
                    mintMultiSignature,
                    freezeMultiSignature))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    mintMultiSignature.Sign(msgData),
                    mintAccount.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();

            // Here we mint tokens to an account using the mint authority multi sig
            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                    initialAccount,
                    mintAccount,
                    ownerAccount.PublicKey))
                .AddInstruction(TokenProgram.MintTo(
                    mintAccount,
                    initialAccount,
                    25000,
                    mintMultiSignature,
                    new List<PublicKey>
                    {
                        mintSigner1,
                        mintSigner2,
                        mintSigner4
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
                    mintSigner1.Sign(msgData),
                    mintSigner2.Sign(msgData),
                    mintSigner4.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();

            // After doing this, we freeze the account to which we just minted tokens
            // Notice how the signers used are different, because the `freezeAuthority` has different signers
            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.FreezeAccount(
                        initialAccount,
                        mintAccount,
                        freezeMultiSignature,
                        TokenProgram.ProgramIdKey,
                        new List<PublicKey>
                        {
                            freezeSigner2,
                            freezeSigner3,
                            freezeSigner4,
                        }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    freezeSigner2.Sign(msgData),
                    freezeSigner3.Sign(msgData),
                    freezeSigner4.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();

            // Because we're actually cool people, we now thaw that same account and then set the authority to nothing
            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.ThawAccount(
                    initialAccount,
                    mintAccount,
                    freezeMultiSignature,
                    TokenProgram.ProgramIdKey,
                    new List<PublicKey>
                    {
                        freezeSigner2,
                        freezeSigner3,
                        freezeSigner4,
                    }))
                .AddInstruction(TokenProgram.SetAuthority(
                    mintAccount,
                    AuthorityType.FreezeAccount,
                    freezeMultiSignature,
                    null,
                    new List<PublicKey>
                    {
                        freezeSigner2,
                        freezeSigner3,
                        freezeSigner4,
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    freezeSigner2.Sign(msgData),
                    freezeSigner3.Sign(msgData),
                    freezeSigner4.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);
        }
    }

    /// <summary>
    /// Example of how to approve and revoke a delegate to transfer tokens using multisig
    /// </summary>
    public class ApproveCheckedMultisigExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

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
            Account delegateAccount = wallet.GetAccount(194330);
            Account mintAccount = wallet.GetAccount(94330);
            Account initialAccount = wallet.GetAccount(84330);

            // the token mint multisig
            Account mintMultiSignature = wallet.GetAccount(10116);

            // the signers for the token mint authority multisig
            Account mintSigner1 = wallet.GetAccount(251280);
            Account mintSigner2 = wallet.GetAccount(251281);
            Account mintSigner3 = wallet.GetAccount(251282);
            Account mintSigner4 = wallet.GetAccount(251283);
            Account mintSigner5 = wallet.GetAccount(251284);

            // The token account
            Account tokenAccountWithMultisigOwner = wallet.GetAccount(4044);
            // The multisig which is the token account authority
            Account tokenMultiSignature = wallet.GetAccount(4045);

            // the signers for the token authority multisig
            Account tokenAccountSigner1 = wallet.GetAccount(25490);
            Account tokenAccountSigner2 = wallet.GetAccount(25491);
            Account tokenAccountSigner3 = wallet.GetAccount(25492);
            Account tokenAccountSigner4 = wallet.GetAccount(25493);
            Account tokenAccountSigner5 = wallet.GetAccount(25494);

            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    tokenMultiSignature,
                    minBalanceForExemptionMultiSig,
                    TokenProgram.MultisigAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMultiSignature(
                    tokenMultiSignature,
                    new List<PublicKey>
                    {
                        tokenAccountSigner1,
                        tokenAccountSigner2,
                        tokenAccountSigner3,
                        tokenAccountSigner4,
                        tokenAccountSigner5
                    },
                    3))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    tokenMultiSignature.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);

            string signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();

            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    tokenAccountWithMultisigOwner,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                    tokenAccountWithMultisigOwner,
                    mintAccount,
                    tokenMultiSignature))
                .AddInstruction(TokenProgram.MintTo(
                    mintAccount,
                    tokenAccountWithMultisigOwner,
                    25000,
                    mintMultiSignature,
                    new List<PublicKey>
                    {
                        mintSigner1,
                        mintSigner2,
                        mintSigner4
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    tokenAccountWithMultisigOwner.Sign(msgData),
                    mintSigner1.Sign(msgData),
                    mintSigner2.Sign(msgData),
                    mintSigner4.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();

            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.ApproveChecked(
                        tokenAccountWithMultisigOwner,
                        delegateAccount,
                        5000,
                        10,
                        tokenMultiSignature,
                        mintAccount,
                        new List<PublicKey>
                        {
                            tokenAccountSigner1,
                            tokenAccountSigner2,
                            tokenAccountSigner3,
                        }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    tokenAccountSigner1.Sign(msgData),
                    tokenAccountSigner2.Sign(msgData),
                    tokenAccountSigner3.Sign(msgData),
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

            blockHash = rpcClient.GetRecentBlockHash();


            msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.TransferChecked(
                    tokenAccountWithMultisigOwner,
                    initialAccount,
                    5000,
                    10,
                    delegateAccount,
                    mintAccount))
                .AddInstruction(TokenProgram.Revoke(
                    tokenAccountWithMultisigOwner,
                    tokenMultiSignature,
                    new List<PublicKey>
                    {
                        tokenAccountSigner1,
                        tokenAccountSigner2,
                        tokenAccountSigner3,
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            tx = Transaction.Populate(msg,
                new List<byte[]> {
                    ownerAccount.Sign(msgData),
                    delegateAccount.Sign(msgData),
                    tokenAccountSigner1.Sign(msgData),
                    tokenAccountSigner2.Sign(msgData),
                    tokenAccountSigner3.Sign(msgData)
                });

            txBytes = Examples.LogTransactionAndSerialize(tx);

            signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

        }
    }

    /// <summary>
    /// Example of how to mint and burn using multisigs
    /// </summary>
    public class SimpleMintToAndBurnCheckedMultisigExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

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
            Account mintAccount = wallet.GetAccount(94330);
            Account initialAccount = wallet.GetAccount(84330);

            // the token mint multisig
            Account mintMultiSignature = wallet.GetAccount(10116);
            Account mintSigner1 = wallet.GetAccount(251280);
            Account mintSigner2 = wallet.GetAccount(251281);
            Account mintSigner3 = wallet.GetAccount(251282);

            // The token account
            Account tokenAccountWithMultisigOwner = wallet.GetAccount(4044);
            // The multisig which is the token account authority
            Account tokenMultiSignature = wallet.GetAccount(4045);

            // the signers for the token authority multisig
            Account tokenAccountSigner1 = wallet.GetAccount(25490);
            Account tokenAccountSigner2 = wallet.GetAccount(25491);
            Account tokenAccountSigner3 = wallet.GetAccount(25492);
            Account tokenAccountSigner4 = wallet.GetAccount(25493);
            Account tokenAccountSigner5 = wallet.GetAccount(25494);

            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.MintToChecked(
                    mintAccount,
                    tokenAccountWithMultisigOwner,
                    mintMultiSignature,
                    1_000_000_000,
                    10,
                    new List<PublicKey>()
                    {
                        mintSigner1,
                        mintSigner2,
                        mintSigner3
                    }))
                .AddInstruction(TokenProgram.BurnChecked(
                    mintAccount,
                    tokenAccountWithMultisigOwner,
                    tokenMultiSignature,
                    500_000,
                    10,
                    new List<PublicKey>()
                    {
                        tokenAccountSigner1,
                        tokenAccountSigner2,
                        tokenAccountSigner3
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    mintSigner1.Sign(msgData),
                    mintSigner2.Sign(msgData),
                    mintSigner3.Sign(msgData),
                    tokenAccountSigner1.Sign(msgData),
                    tokenAccountSigner2.Sign(msgData),
                    tokenAccountSigner3.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);

            string signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

        }
    }

    /// <summary>
    /// Example of how to close a multisig account
    /// </summary>
    public class BurnCheckedAndCloseAccountMultisigExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

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
            Account mintAccount = wallet.GetAccount(94330);

            // The multisig which is the token account authority
            Account tokenAccountWithMultisigOwner = wallet.GetAccount(4044);
            Account tokenMultiSignature = wallet.GetAccount(4045);
            Account tokenAccountSigner1 = wallet.GetAccount(25490);
            Account tokenAccountSigner2 = wallet.GetAccount(25491);
            Account tokenAccountSigner3 = wallet.GetAccount(25492);

            // The account has balance so we'll burn it before
            RequestResult<ResponseValue<TokenBalance>> balance =
                rpcClient.GetTokenAccountBalance(tokenAccountWithMultisigOwner.PublicKey);

            Console.WriteLine($"Account Balance >> {balance.Result.Value.UiAmountString}");

            byte[] msgData = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(TokenProgram.BurnChecked(
                    mintAccount,
                    tokenAccountWithMultisigOwner,
                    tokenMultiSignature,
                    balance.Result.Value.AmountUlong,
                    10,
                    new List<PublicKey>()
                    {
                        tokenAccountSigner1,
                        tokenAccountSigner2,
                        tokenAccountSigner3
                    }))
                .AddInstruction(TokenProgram.CloseAccount(
                    tokenAccountWithMultisigOwner,
                    ownerAccount,
                    tokenMultiSignature,
                    TokenProgram.ProgramIdKey,
                    new List<PublicKey>()
                    {
                        tokenAccountSigner1,
                        tokenAccountSigner2,
                        tokenAccountSigner3
                    }))
                .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
                .CompileMessage();

            Message msg = Examples.DecodeMessageFromWire(msgData);

            Console.WriteLine("\n\tPOPULATING TRANSACTION WITH SIGNATURES\t");
            Transaction tx = Transaction.Populate(msg,
                new List<byte[]>
                {
                    ownerAccount.Sign(msgData),
                    tokenAccountSigner1.Sign(msgData),
                    tokenAccountSigner2.Sign(msgData),
                    tokenAccountSigner3.Sign(msgData),
                });

            byte[] txBytes = Examples.LogTransactionAndSerialize(tx);

            string signature = Examples.SubmitTxSendAndLog(txBytes);
            Examples.PollConfirmedTx(signature);

        }
    }

}