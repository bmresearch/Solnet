using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Core.Sockets
{
    internal interface IWebSocket : IDisposable
    {
        WebSocketState State { get; }
        string CloseStatusDescription { get; }
        WebSocketCloseStatus? CloseStatus { get; }
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);
        Task CloseAsync(CancellationToken cancellationToken);
        Task SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
        Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);
        Task<WebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    }
}