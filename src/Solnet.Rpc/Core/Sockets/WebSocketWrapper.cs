using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets;

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

    public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription,
        CancellationToken cancellationToken)
    {
        return webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
    }

    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        return webSocket.ConnectAsync(uri, cancellationToken);
    }

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        return webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
    }

    public ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        return webSocket.ReceiveAsync(buffer, cancellationToken);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
        CancellationToken cancellationToken)
    {
        return webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }

    #region IDisposable Support

    private bool disposedValue; // To detect redundant calls

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