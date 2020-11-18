using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.ServiceModel;

namespace IBAR.SyncerManager
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    class Service : IContract
    {

        public void Status(string input)
        {
            var parts = input.Split('$');

            if (Enum.TryParse(parts[0], true, out JobEnum job))
            {
                var context = GlobalHost.ConnectionManager.GetConnectionContext<LogConnection>();
                //ConnectionMapping.GetConnections(job).ToList().ForEach(conn => context.Connection.Send(conn, parts[1]));
                Console.WriteLine(parts[1]);
                return;
            }

            Console.WriteLine(parts[0]);
        }

        public void ReloadJob()
        {
            Console.WriteLine("Reload Job");
            IMyContractCallback callbackInstance
           = OperationContext.Current.GetCallbackChannel<IMyContractCallback>();
            callbackInstance.OnCallback();
        }

        public void GetInstance()
        {
        }
    }
}