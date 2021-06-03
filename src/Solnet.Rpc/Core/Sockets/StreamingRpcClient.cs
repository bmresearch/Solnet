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
    /// <summary>
    /// Base streaming Rpc client class that abstracts the websocket handling.
    /// </summary>
    public abstract class StreamingRpcClient
    {
        /// <summary>
        /// The web socket client abstraction.
        /// </summary>
        protected readonly IWebSocket ClientSocket;

        /// <summary>
        /// The logger instance.
        /// </summary>
        protected readonly ILogger _logger;

        /// <inheritdoc cref="IStreamingRpcClient.NodeAddress"/>
        public Uri NodeAddress { get; }

        /// <summary>
        /// The internal constructor that setups the client.
        /// </summary>
        /// <param name="url">The url of the streaming RPC server.</param>
        /// <param name="logger">The possible logger instance.</param>
        /// <param name="socket">The possible websocket instance. A new instance will be created if null.</param>
        protected StreamingRpcClient(string url, ILogger logger, IWebSocket socket = default)
        {
            NodeAddress = new Uri(url);
            ClientSocket = socket ?? new WebSocketWrapper(new ClientWebSocket());
            _logger = logger;
        }

        /// <summary>
        /// Initializes the websocket connection and starts receiving messages asynchronously.
        /// </summary>
        /// <returns>Returns the task representing the asynchronous task.</returns>
        public async Task Init()
        {
            await ClientSocket.ConnectAsync(NodeAddress, CancellationToken.None).ConfigureAwait(false);
            _ = Task.Run(StartListening);
        }

        /// <summary>
        /// Starts listeing to new messages.
        /// </summary>
        /// <returns>Returns the task representing the asynchronous task.</returns>
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

        /// <summary>
        /// Reads the next message from the socket.
        /// </summary>
        /// <param name="cancellationToken">The cancelation token.</param>
        /// <returns>Returns the task representing the asynchronous task.</returns>
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

        /// <summary>
        /// Handless a new message payload.
        /// </summary>
        /// <param name="messagePayload">The message payload.</param>
        protected abstract void HandleNewMessage(Memory<byte> messagePayload);
    }
}
