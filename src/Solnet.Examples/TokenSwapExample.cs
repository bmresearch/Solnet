using Solnet.Programs;
using Solnet.Programs.TokenSwap;
using Solnet.Programs.TokenSwap.Models;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;

namespace Solnet.Examples
{
    public class TokenSwapExample : IExample
    {

        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new Wallet.Wallet(MnemonicWords);

            var tokenAMint = new Account();
            var tokenAUserAccount = new Account();
            var tokenBMint = new Account();
            var tokenBUserAccount = new Account();

            //setup some mints and tokens owned by wallet
            RequestResult<ResponseValue<BlockHash>> blockHash = RpcClient.GetRecentBlockHash();
            var tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    tokenAMint,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    tokenBMint,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    tokenAUserAccount,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    tokenBUserAccount,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(TokenProgram.InitializeMint(
                    tokenAMint,
                    9,
                    wallet.Account
                ))
                .AddInstruction(TokenProgram.InitializeMint(
                    tokenBMint,
                    9,
                    wallet.Account
                ))
                .AddInstruction(TokenProgram.InitializeAccount(
                    tokenAUserAccount,
                    tokenAMint,
                    wallet.Account
                ))
                .AddInstruction(TokenProgram.InitializeAccount(
                    tokenBUserAccount,
                    tokenBMint,
                    wallet.Account
                ))
                .AddInstruction(TokenProgram.MintTo(
                    tokenAMint,
                    tokenAUserAccount,
                    1_000_000_000_000,
                    wallet.Account
                ))
                .AddInstruction(TokenProgram.MintTo(
                    tokenBMint,
                    tokenBUserAccount,
                    1_000_000_000_000,
                    wallet.Account
                ))
                .Build(new Account[] { wallet.Account, tokenAMint, tokenBMint, tokenAUserAccount, tokenBUserAccount });
            var txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            var swap = new Account();
            var program = new TokenSwapProgram(swap);

            var swapTokenAAccount= new Account();
            var swapTokenBAccount = new Account();

            //init the swap authority's token accounts
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    swapTokenAAccount,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(TokenProgram.InitializeAccount(
                    swapTokenAAccount,
                    tokenAMint,
                    program.SwapAuthority
                ))
                .AddInstruction(TokenProgram.Transfer(
                    tokenAUserAccount,
                    swapTokenAAccount,
                    5_000_000_000,
                    wallet.Account
                ))
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    swapTokenBAccount,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(TokenProgram.InitializeAccount(
                    swapTokenBAccount,
                    tokenBMint,
                    program.SwapAuthority
                ))
                .AddInstruction(TokenProgram.Transfer(
                    tokenBUserAccount,
                    swapTokenBAccount,
                    5_000_000_000,
                    wallet.Account
                ))
                .Build(new Account[] { wallet.Account, swapTokenAAccount, swapTokenBAccount });
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            var poolMint = new Account();
            var poolUserAccount = new Account();
            var poolFeeAccount = new Account();

            //create the pool mint and the user and fee pool token accounts
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    poolMint,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(TokenProgram.InitializeMint(
                    poolMint,
                    9,
                    program.SwapAuthority
                ))
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    poolUserAccount,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(TokenProgram.InitializeAccount(
                    poolUserAccount,
                    poolMint,
                    wallet.Account
                ))
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    poolFeeAccount,
                    RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey
                ))
                .AddInstruction(TokenProgram.InitializeAccount(
                    poolFeeAccount,
                    poolMint,
                    program.OwnerKey
                ))
                .Build(new Account[] { wallet.Account, poolMint, poolUserAccount, poolFeeAccount });
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            //create the swap
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(SystemProgram.CreateAccount(
                    wallet.Account,
                    swap,
                    RpcClient.GetMinimumBalanceForRentExemption((long)program.TokenSwapAccountDataSize).Result,
                    program.TokenSwapAccountDataSize,
                    program.ProgramIdKey
                ))
                .AddInstruction(program.Initialize(
                    swapTokenAAccount,
                    swapTokenBAccount,
                    poolMint,
                    poolFeeAccount,
                    poolUserAccount,
                    new Fees()
                    {
                        TradeFeeNumerator = 25,
                        TradeFeeDenominator = 10000,
                        OwnerTradeFeeNumerator = 5,
                        OwnerTradeFeeDenomerator = 10000,
                        OwnerWithrawFeeNumerator = 0,
                        OwnerWithrawFeeDenomerator = 0,
                        HostFeeNumerator = 20,
                        HostFeeDenomerator = 100
                    },
                    SwapCurve.ConstantProduct
                ))
                .Build(new Account[] { wallet.Account, swap });
            Console.WriteLine($"Swap Account: {swap}");
            Console.WriteLine($"Swap Auth Account: {program.SwapAuthority}");
            Console.WriteLine($"Pool Mint Account: {poolMint}");
            Console.WriteLine($"Pool User Account: {poolUserAccount}");
            Console.WriteLine($"Pool Fee Account: {poolFeeAccount}");
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            //now a user can swap in the pool
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(program.Swap(
                    wallet.Account,
                    tokenAUserAccount,
                    swapTokenAAccount,
                    swapTokenBAccount,
                    tokenBUserAccount,
                    poolMint,
                    poolFeeAccount,
                    null,
                    1_000_000_000,
                    500_000))
                .Build(wallet.Account);
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            //user can add liq
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(program.DepositAllTokenTypes(
                    wallet.Account,
                    tokenAUserAccount,
                    tokenBUserAccount,
                    swapTokenAAccount,
                    swapTokenBAccount,
                    poolMint,
                    poolUserAccount,
                    1_000_000,
                    100_000_000_000,
                    100_000_000_000))
                .Build(wallet.Account);
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            //user can remove liq
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(program.WithdrawAllTokenTypes(
                    wallet.Account,
                    poolMint,
                    poolUserAccount,
                    swapTokenAAccount,
                    swapTokenBAccount,
                    tokenAUserAccount,
                    tokenBUserAccount,
                    poolFeeAccount,
                    1_000_000,
                    1_000,
                    1_000))
                .Build(wallet.Account);
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            //user can deposit single
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(program.DepositSingleTokenTypeExactAmountIn(
                    wallet.Account,
                    tokenAUserAccount,
                    swapTokenAAccount,
                    swapTokenBAccount,
                    poolMint,
                    poolUserAccount,
                    1_000_000_000,
                    1_000))
                .Build(wallet.Account);
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);

            //user can withdraw single
            blockHash = RpcClient.GetRecentBlockHash();
            tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(wallet.Account)
                .AddInstruction(program.WithdrawSingleTokenTypeExactAmountOut(
                    wallet.Account,
                    poolMint,
                    poolUserAccount,
                    swapTokenAAccount,
                    swapTokenBAccount,
                    tokenAUserAccount,
                    poolFeeAccount,
                    1_000_000,
                    100_000))
                .Build(wallet.Account);
            txSig = Examples.SubmitTxSendAndLog(tx);
            Examples.PollConfirmedTx(txSig);
        }
    }
}