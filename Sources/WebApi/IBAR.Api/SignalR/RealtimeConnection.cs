using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace IBAR.Api.SignalR
{
    public class RealtimeConnection : PersistentConnection
    {

        // TODO: log onConnected, onDisconnected...

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            // Console.WriteLine($@"Client {connectionId} connected");
            return base.OnConnected(request, connectionId);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            Console.WriteLine($@"Data was sent by client {connectionId}: {data}");
            return base.OnReceived(request, connectionId, data);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            // Console.WriteLine($@"Clint {connectionId} disconnected");
            return base.OnDisconnected(request, connectionId, stopCalled);
        }
    }
}