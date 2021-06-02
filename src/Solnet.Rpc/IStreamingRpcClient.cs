using System;
using System.Threading.Tasks;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;

namespace Solnet.Rpc
{
    public interface IStreamingRpcClient
    {
        Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback);
        SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback);
        Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback);
        Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback);
        SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback);
        SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback);
        Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback);
        SubscriptionState SubscribeSignature(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback);
        Task<SubscriptionState> SubscribeProgramAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ProgramInfo>> callback);
        SubscriptionState SubscribeProgram(string transactionSignature, Action<SubscriptionState, ResponseValue<ProgramInfo>> callback);
        Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback);
        SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback);
        Task UnsubscribeAsync(SubscriptionState subscription);
        void Unsubscribe(SubscriptionState subscription);
        Task Init();
    }
}