using System;
using System.Threading.Tasks;
using IBAR.TradeModel.Business.Data;
using Microsoft.AspNet.SignalR;

namespace IBAR.TradeModel.Business.SignalR
{
    public class LogConnection : PersistentConnection
    {
        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return Connection.Send(connectionId, "Connected");
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            if (!Enum.TryParse(data, true, out JobEnum job)) return base.OnReceived(request, connectionId, data);

            ConnectionMapping.RemoveById(connectionId);

            ConnectionMapping.Add(job, connectionId);

            Console.WriteLine("Client: " + data);

            return base.OnReceived(request, connectionId, data);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            ConnectionMapping.RemoveById(connectionId);

            return base.OnDisconnected(request, connectionId, stopCalled);
        }
    }
}