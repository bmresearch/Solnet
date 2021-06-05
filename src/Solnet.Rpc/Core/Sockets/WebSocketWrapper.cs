using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    internal class WebSocketWrapper : IWebSocket
    {
        private readonly ClientWebSocket webSocket;

        internal WebSocketWrapper(ClientWebSocket webSocket)
        {
            this.webSocket = webSocket;
        }

        public WebSocketState State => webSocket.State;

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            => webSocket.ConnectAsync(uri, cancellationToken);

        public ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            => webSocket.ReceiveAsync(buffer, cancellationToken);

        public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            => webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    webSocket.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}