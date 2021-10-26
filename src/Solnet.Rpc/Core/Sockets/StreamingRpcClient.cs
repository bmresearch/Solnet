using Microsoft.Extensions.Logging;
using Solnet.Rpc.Types;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    /// <summary>
    /// Base streaming Rpc client class that abstracts the websocket handling.
    /// </summary>
    internal abstract class StreamingRpcClient : IDisposable
    {
        private SemaphoreSlim _sem;

        /// <summary>
        /// The web socket client abstraction.
        /// </summary>
        protected IWebSocket ClientSocket;

        private bool disposedValue;

        /// <summary>
        /// The logger instance.
        /// </summary>
        protected readonly ILogger _logger;

        /// <inheritdoc cref="IStreamingRpcClient.NodeAddress"/>
        public Uri NodeAddress { get; }

        /// <inheritdoc cref="IStreamingRpcClient.State"/>
        public WebSocketState State => ClientSocket.State;

        /// <inheritdoc cref="IStreamingRpcClient.ConnectionStateChangedEvent"/>
        public event EventHandler<WebSocketState> ConnectionStateChangedEvent;

        private ConnectionStats _connectionStats;

        /// <summary>
        /// Statistics of the current connection.
        /// </summary>
        public IConnectionStatistics Statistics => _connectionStats;

        /// <summary>
        /// The internal constructor that setups the client.
        /// </summary>
        /// <param name="url">The url of the streaming RPC server.</param>
        /// <param name="logger">The possible logger instance.</param>
        /// <param name="socket">The possible websocket instance. A new instance will be created if null.</param>
        /// <param name="clientWebSocket">The possible ClientWebSocket instance. A new instance will be created if null.</param>
        protected StreamingRpcClient(string url, ILogger logger, IWebSocket socket = default, ClientWebSocket clientWebSocket = default)
        {
            NodeAddress = new Uri(url);
            ClientSocket = socket ?? new WebSocketWrapper(clientWebSocket ?? new ClientWebSocket());
            _logger = logger;
            _sem = new SemaphoreSlim(1, 1);
            _connectionStats = new ConnectionStats();
        }

        /// <summary>
        /// Initializes the websocket connection and starts receiving messages asynchronously.
        /// </summary>
        /// <returns>Returns the task representing the asynchronous task.</returns>
        public async Task ConnectAsync()
        {
            _sem.Wait();
            if (ClientSocket.State != WebSocketState.Open)
            {
                await ClientSocket.ConnectAsync(NodeAddress, CancellationToken.None).ConfigureAwait(false);
                _ = Task.Run(StartListening);
                ConnectionStateChangedEvent?.Invoke(this, State);
            }
            _sem.Release();
        }

        /// <inheritdoc cref="IStreamingRpcClient.DisconnectAsync"/>
        public async Task DisconnectAsync()
        {
            _sem.Wait();
            if (ClientSocket.State == WebSocketState.Open)
            {
                await ClientSocket.CloseAsync(CancellationToken.None);

                //notify at the end of StartListening loop, given that it should end as soon as we terminate connection here
                //and will also notify when there is a non-user triggered disconnection event

                // handle disconnection cleanup
                ClientSocket.Dispose();
                ClientSocket = new WebSocketWrapper(new ClientWebSocket());
                CleanupSubscriptions();
            }
            _sem.Release();
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
            ConnectionStateChangedEvent?.Invoke(this, State);
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
                _connectionStats.AddReceived((uint)count);
                HandleNewMessage(mem);
            }
        }

        /// <summary>
        /// Handless a new message payload.
        /// </summary>
        /// <param name="messagePayload">The message payload.</param>
        protected abstract void HandleNewMessage(Memory<byte> messagePayload);

        /// <summary>
        /// Clean up subscription objects after disconnection.
        /// </summary>
        protected abstract void CleanupSubscriptions();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ClientSocket.Dispose();
                    _sem.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StreamingRpcClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}