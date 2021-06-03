using Microsoft.Extensions.Logging;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    public abstract class StreamingRpcClient
    {
        protected readonly IWebSocket ClientSocket;

        private readonly string _socketUri;

        protected readonly ILogger _logger;

        protected StreamingRpcClient(string nodeUri, ILogger logger, IWebSocket socket = default)
        {
            ClientSocket = socket ?? new WebSocketWrapper(new ClientWebSocket());
            _socketUri = nodeUri;
            _logger = logger;
        }

        public async Task Init()
        {
            await ClientSocket.ConnectAsync(new Uri(_socketUri), CancellationToken.None).ConfigureAwait(false);
            _ = Task.Run(StartListening);
        }

        private async Task StartListening()
        {
            while (ClientSocket.State == WebSocketState.Open)
            {
                try
                {
                    await ReadNextMessage().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger?.LogDebug(new EventId(), e, "Exception trying to read next message.");
                }
            }
            _logger?.LogDebug(new EventId(), $"Stopped reading messages. ClientSocket.State changed to {ClientSocket.State}");
        }

        private async Task ReadNextMessage(CancellationToken cancellationToken = default)
        {
            var buffer = new byte[32768];
            Memory<byte> mem = new Memory<byte>(buffer);
            ValueWebSocketReceiveResult result = await ClientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
            int count = result.Count;

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ClientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            }
            else
            {
                if (!result.EndOfMessage)
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Write(mem.Span);


                    while (!result.EndOfMessage)
                    {
                        result = await ClientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
                        ms.Write(mem.Slice(0, result.Count).Span);
                        count += result.Count;
                    }

                    mem = new Memory<byte>(ms.ToArray());
                }
                else
                {
                    mem = mem.Slice(0, count);
                }

                HandleNewMessage(mem);
            }
        }

        protected abstract void HandleNewMessage(Memory<byte> mem);
    }
}
