using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Website
{
    public class JobProgressHub : Hub
    {
        private readonly static ConnectionMapping<Guid> Connections = new ConnectionMapping<Guid>();

        public override Task OnConnected()
        {
            Guid jobid = GetJobId();

            Connections.Add(jobid, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Guid jobid = GetJobId();

            Connections.Remove(jobid, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Guid jobId = GetJobId();

            if (!Connections.GetConnections(jobId).Contains(Context.ConnectionId))
            {
                Connections.Add(jobId, Context.ConnectionId);
            }

            return base.OnReconnected();
        }

        private Guid GetJobId()
        {
            return new Guid(Context.QueryString["jobId"]);
        }

        public static IEnumerable<string> GetUserConnections(Guid jobId)
        {
            return Connections.GetConnections(jobId);
        }
    }
}