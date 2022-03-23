using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Core.Sockets
{
    internal class WebSocketWrapper : IWebSocket
    {
        private readonly ClientWebSocket webSocket;

        internal WebSocketWrapper(ClientWebSocket webSocket)
        {
            this.webSocket = webSocket;
        }

        public WebSocketCloseStatus? CloseStatus => webSocket.CloseStatus;

        public string CloseStatusDescription => webSocket.CloseStatusDescription;

        public WebSocketState State => webSocket.State;

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            => webSocket.ConnectAsync(uri, cancellationToken);

        public Task CloseAsync(CancellationToken cancellationToken)
            => webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);

        public Task<WebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            => webSocket.ReceiveAsync(new ArraySegment<byte>(buffer.ToArray()), cancellationToken);

        public Task SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            => webSocket.SendAsync(new ArraySegment<byte>(buffer.ToArray()), messageType, endOfMessage, cancellationToken);

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