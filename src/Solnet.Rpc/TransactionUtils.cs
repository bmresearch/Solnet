using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Utilities to check transaction confirmation status.
    /// </summary>
    public static class TransactionConfirmationUtils
    {
        /// <summary>
        /// Confirms a transaction - same method as web3.js.
        /// </summary>
        /// <param name="rpc">The rpc client instance.</param>
        /// <param name="streamingRpcClient">The streaming rpc client instance.</param>
        /// <param name="hash">The hash of the transaction.</param>
        /// <param name="validBlockHeight">The last valid block height of the blockhash used in the transaction.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns null if the transaction wasn't confirmed, otherwise returns the confirmation slot and possible transaction error.</returns>
        public static async Task<ResponseValue<ErrorResult>> ConfirmTransaction(IRpcClient rpc, IStreamingRpcClient streamingRpcClient, 
            string hash, ulong validBlockHeight, Commitment commitment = Commitment.Finalized)
        {
            TaskCompletionSource t = new();
            ResponseValue<ErrorResult> result = null;

            var s = await streamingRpcClient.SubscribeSignatureAsync(hash, (s, e) =>
            {
                result = e;
                t.SetResult();
            },
            commitment);

            var checkTask = Task.Run(async () =>
            {
                var currHeight = await rpc.GetBlockHeightAsync(commitment);
                while (currHeight.Result < validBlockHeight)
                {
                    await Task.Delay(1000);
                    currHeight = await rpc.GetBlockHeightAsync(commitment);
                }
            });


            Task.WaitAny(t.Task, checkTask);

            if (!t.Task.IsCompleted)
            {
                await s.UnsubscribeAsync();
            }

            return result;
        }

        /// <summary>
        /// Confirms a transaction - old web3.js using constant timeout based on commitment parameter.
        /// </summary>
        /// <param name="rpc">The rpc client instance.</param>
        /// <param name="streamingRpcClient">The streaming rpc client instance.</param>
        /// <param name="hash">The hash of the transaction.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns null if the transaction wasn't confirmed, otherwise returns the confirmation slot and possible transaction error.</returns>
        public static async Task<ResponseValue<ErrorResult>> ConfirmTransaction(IRpcClient rpc, IStreamingRpcClient streamingRpcClient,
            string hash, Commitment commitment = Commitment.Finalized)
        {
            TaskCompletionSource t = new();
            ResponseValue<ErrorResult> result = null;

            var s = await streamingRpcClient.SubscribeSignatureAsync(hash, (s, e) =>
            {
                result = e;
                t.SetResult();
            },
            commitment);

            var timeout = commitment == Commitment.Finalized ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(30);
            var delay = Task.Delay(timeout);

            Task.WaitAny(t.Task, delay);

            if (!t.Task.IsCompleted)
            {
                await s.UnsubscribeAsync();
            }

            return result;
        }

    }
}
