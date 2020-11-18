using System;
using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Business.Data;

namespace IBAR.TradeModel.Business.SignalR
{
    public static class ConnectionMapping
    {
        private static readonly Dictionary<JobEnum, HashSet<string>> Connections;

        static ConnectionMapping()
        {
            Connections = new Dictionary<JobEnum, HashSet<string>>();
            foreach (var value in Enum.GetValues(typeof(JobEnum)))
            {
                var job = value is JobEnum job1 ? job1 : JobEnum.Ftp;
                Connections.Add(job, new HashSet<string>());
            }
        }

        public static void Add(JobEnum jobEnum, string id)
        {
            lock (Connections)
            {
                if (!Connections[jobEnum].Contains(id))
                {
                    Connections[jobEnum].Add(id);
                }
            }
        }

        public static void RemoveById(string id)
        {
            lock (Connections)
            {
                Connections.FirstOrDefault(jobGroup => jobGroup.Value.Contains(id)).Value?.Remove(id);
            }
        }

        public static IEnumerable<string> GetConnections(JobEnum jobEnum)
        {
            lock (Connections)
            {
                return Connections[jobEnum];
            }
        }
    }
}