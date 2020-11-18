using System;
using System.Linq;
using IBAR.TradeModel.Business.Data;
using IBAR.TradeModel.Business.SignalR;
using Microsoft.AspNet.SignalR;

namespace IBAR.TradeModel.Business.Services.Wcf
{
    public class MyCallBack : IContractCallBack
    {
        public void OnCallback(string input)
        {
            var jobNameIndex = input.IndexOf('$');
            var jobName = input.Substring(0, jobNameIndex);
            
            if (Enum.TryParse(jobName, true, out JobEnum job))
            {
                var bodyInput = input.Substring(jobNameIndex + 1);
                var context = GlobalHost.ConnectionManager.GetConnectionContext<LogConnection>();
                ConnectionMapping.GetConnections(job).ToList().ForEach(conn => context.Connection.Send(conn, bodyInput));
                Console.WriteLine(bodyInput);
                return;
            }

            Console.WriteLine(input);
        }
    }
}