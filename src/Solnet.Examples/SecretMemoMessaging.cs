using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin.DataEncoders;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class SecretMemoMessaging
    {
        private static Base58Encoder Encoder = new();
        
        private static string ShaSeed = "pseudo secure secret messaging on solana";
        
        public static byte[] HmacSha512(byte[] data)
        {
            using var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(ShaSeed));
            var i = hmacSha512.ComputeHash(data);
            
            return i;
        }
        
        public abstract class Guardian
        {
            private List<string> _lastMessagesSignature;
            private SolanaStreamingRpcClient _streamingRpcClient;
            
            private static string PrettyPrintTransactionSimulationLogs(LogInfo logMessage)
            {
                var logString = "[" + DateTime.UtcNow.ToLongTimeString() + $"][Received]\n\tSignature: {logMessage.Signature}\n";
                foreach (var log in logMessage.Logs)
                {
                    logString += $"\tLog Trace: {log}\n";
                }

                return logString;
            }

            protected Guardian()
            {
                _lastMessagesSignature = new List<string>();
                _streamingRpcClient = new SolanaStreamingRpcClient("wss://testnet.solana.com/");
            }

            protected void SubscribeLogs()
            {
                _streamingRpcClient.Init().Wait();

                var sub = 
                    _streamingRpcClient.SubscribeLogInfo("Memo1UhkJRfHyvLMcVucJwxXeuD728EqVDDwQDxFMNo", OnLogMessage);

                sub.SubscriptionChanged += SubscriptionChanged;

                Console.ReadKey();
            }

            private void OnLogMessage(SubscriptionState s, ResponseValue<LogInfo> data)
            {
                if (_lastMessagesSignature.Contains(data.Value.Signature)) return;
                _lastMessagesSignature.Add(data.Value.Signature);
                
                var logs = PrettyPrintTransactionSimulationLogs(data.Value);
                Console.WriteLine($"{logs}");
                
                HandleNewMemo(data.Value.Signature);
                
                if (_lastMessagesSignature.Count > 10)
                    _lastMessagesSignature.RemoveAt(0);
            }

            private static void SubscriptionChanged(object sender, SubscriptionEvent e)
            {
                Console.WriteLine("Subscription changed to: " + e.Status);
            }

            protected abstract void HandleNewMemo(string transactionSignature);
        }

        public class Channel
        {
            public string Name { get; set; }
            public string Password { get; set; }
            public int PrivacyType { get; set; }
            
            public byte[] Sha { get; set; }
            
            public string EncodedSha { get; set; }

            public Channel(string name, string password, int type)
            {
                Name = name;
                Password = password;
                PrivacyType = type;

                Sha = CalculateSha();
                EncodedSha = EncodeSha();
            }

            private byte[] CalculateSha()
            {
                return PrivacyType switch
                {
                    0 => HmacSha512(Encoding.UTF8.GetBytes(Name)),
                    1 => HmacSha512(Encoding.UTF8.GetBytes($"{Name}#{Password}")),
                    _ => HmacSha512(Encoding.UTF8.GetBytes(Name))
                };
            }            
            private string EncodeSha()
            {
                return Encoder.EncodeData(Sha)[..16];
            }

            public override string ToString()
            {
                return $"{Name}-{PrivacyType}/{EncodedSha}";
            }
        }

        public class Message
        {
            private string _data;

            public string Destination { get; set; }
            
            public Message(string data)
            {
                _data = data;
            }
            
            

            private string EncodeMessage()
            {
                return "";
            }

            private void DecodeMessage()
            {
                
            }
        }

        public class MessagingClient : Guardian
        {
            private Account _account;

            private SolanaRpcClient _rpcClient;

            private IList<Channel> _subscriptions;
            
            public MessagingClient(Account account, IList<Channel> subscriptions)
            {
                _account = account;
                _subscriptions = subscriptions;
                _rpcClient = new SolanaRpcClient("https://testnet.solana.com");
            }

            public void ReadAndSendMessages()
            {
                var t = Task.Run(SubscribeLogs);

                var blockHash = _rpcClient.GetRecentBlockHash();
                var tx = new TransactionBuilder().
                    SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                    AddInstruction(MemoProgram.NewMemo(_account, "memo")).
                    Build(_account);
                var sig = _rpcClient.SendTransaction(tx);
                Console.WriteLine("[" + DateTime.UtcNow.ToLongTimeString() + $"][Sent]\n" +
                                  $"\tPayload: memo\n\tSignature: {sig.Result}");

            }


            protected override void HandleNewMemo(string transactionSignature)
            {
            }
        }

        static void Example(string[] args)
        {
            var encoder = new Base58Encoder();
            
            var rpcClient = new SolanaRpcClient("https://testnet.solana.com");
            var wallet = new Wallet.Wallet("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");

            var channels = new List<Channel>
            {
                new("public", "", 0),
                new("solana-channel", "", 0),
                new("solana-channel", "prandpasswordk3k1", 0),
            };

            var clientA = new MessagingClient(wallet.GetAccount(0), channels);
            var t = Task.Run(clientA.ReadAndSendMessages);

            var index = 0;
            var msgChannel = new [] {"public", "blockmountain", "solana-channel", "solana-channel#prandpassword"};
            for (;;)
            {
                var channelIndex = RandomNumberGenerator.GetInt32(0, 4);
                var splitChannelString = msgChannel[channelIndex].Split("#");
                var message = "";
                var messageChannel = "";
                var channelName = "";
                var channelPrivacyType = 0;
                
                if (splitChannelString.Length > 1)
                {
                    channelPrivacyType = 1;
                    messageChannel = msgChannel[channelIndex];
                    channelName = splitChannelString[0];
                }
                else
                {
                    messageChannel = msgChannel[channelIndex];
                    channelName = msgChannel[channelIndex];
                }
                
                var messageChannelSha
                    = HmacSha512(Encoding.UTF8.GetBytes(messageChannel));
                message = $"{(byte)channelPrivacyType}/{encoder.EncodeData(messageChannelSha)[..16]}/{(byte)index} " +
                          $"Hello from Sol.Net to {channelPrivacyType}{channelName} :)";
                
                //var txSim = rpcClient.SimulateTransaction(tx);
                //var logs = TransactionBuilderExample.PrettyPrintTransactionSimulationLogs(txSim.Result.Value);
                //Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs );
                Thread.Sleep(new TimeSpan(0,0,15));
                index++;
            }

            t.Wait();
        }
    }
}