using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Threading;

namespace Solnet.Examples
{
    public class AssociatedTokenAccountsExample : IExample
    {
        
        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new Wallet.Wallet(MnemonicWords);
            
            /*
             * The following region creates and initializes a mint account, it also creates a token account
             * that is initialized with the same mint account and then mints tokens to this newly created token account.
             */
            #region Create and Initialize a token Mint Account
            
            
            RequestResult<ResponseValue<BlockHash>> blockHash = RpcClient.GetRecentBlockHash();
            
            ulong minBalanceForExemptionAcc =
                RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            ulong minBalanceForExemptionMint =
                RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;
            
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");

            Account ownerAccount = wallet.GetAccount(10);
            Account mintAccount = wallet.GetAccount(1002);
            Account initialAccount = wallet.GetAccount(1102);
            Console.WriteLine($"OwnerAccount: {ownerAccount.PublicKey.Key}");
            Console.WriteLine($"MintAccount: {mintAccount.PublicKey.Key}");
            Console.WriteLine($"InitialAccount: {initialAccount.PublicKey.Key}");
            
            byte[] createAndInitializeMintToTx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(ownerAccount).
                AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    minBalanceForExemptionMint,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey)).
                AddInstruction(TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey)).
                AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey)).
                AddInstruction(TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey)).
                AddInstruction(TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    1_000_000,
                    ownerAccount)).
                AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).Build();
            
            string createAndInitializeMintToTxSignature = SubmitTxAndLog(createAndInitializeMintToTx);

            PollConfirmedTx(createAndInitializeMintToTxSignature);
            
            #endregion
            
            /*
             * The following region creates an associated token account (ATA) for a random account and a certain token mint
             * (in this case it's the previously created token mintAccount) and transfers tokens from the previously created
             * token account to the newly created ATA.
             */
            #region Create Associated Token Account
            
            // this public key is from a random account created via www.sollet.io
            // to test this locally I recommend creating a wallet on sollet and deriving this
            PublicKey associatedTokenAccountOwner = new ("65EoWs57dkMEWbK4TJkPDM76rnbumq7r3fiZJnxggj2G");
            PublicKey associatedTokenAccount =
                AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(associatedTokenAccountOwner, mintAccount);
            Console.WriteLine($"AssociatedTokenAccountOwner: {associatedTokenAccountOwner.Key}");
            Console.WriteLine($"AssociatedTokenAccount: {associatedTokenAccount.Key}");
            
            byte[] createAssociatedTokenAccountTx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(ownerAccount).
                AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                    ownerAccount,
                    associatedTokenAccountOwner,
                    mintAccount)).
                AddInstruction(TokenProgram.Transfer(
                    initialAccount,
                    associatedTokenAccount,
                    25000,
                    ownerAccount)).// the ownerAccount was set as the mint authority
                AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net")).Build();
            
            string createAssociatedTokenAccountTxSignature = SubmitTxAndLog(createAssociatedTokenAccountTx);
            
            PollConfirmedTx(createAssociatedTokenAccountTxSignature);

            #endregion
        }

        /// <summary>
        /// Submits a transaction and logs the output from SimulateTransaction.
        /// </summary>
        /// <param name="tx">The transaction data ready to simulate or submit to the network.</param>
        private static string SubmitTxAndLog(byte[] tx)
        {
            Console.WriteLine($"Tx Data: {Convert.ToBase64String(tx)}");

            RequestResult<ResponseValue<SimulationLogs>> txSim = RpcClient.SimulateTransaction(tx);
            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            RequestResult<string> txReq = RpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");

            return txReq.Result;
        }

        /// <summary>
        /// Polls the rpc client until a transaction signature has been confirmed.
        /// </summary>
        /// <param name="signature">The first transaction signature.</param>
        private static void PollConfirmedTx(string signature)
        {
            RequestResult<TransactionMetaSlotInfo> txMeta = RpcClient.GetTransaction(signature);
            if (!txMeta.WasSuccessful)
            {
                Thread.Sleep(2500);
                PollConfirmedTx(signature);
            }
            if (txMeta.Result != null) return;
        }
    }
}