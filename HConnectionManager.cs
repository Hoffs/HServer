using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CoreServer
{
    public class HConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, HConnection> _connections = new ConcurrentDictionary<Guid, HConnection>();

        /// <summary>
        /// Adds a Client to the active connection list.
        /// </summary>
        /// <param name="tcpClient">Instance of connecting client</param>
        /// <returns></returns>
        [NotNull]
        public async Task<HConnection> AddConnectiontTask([NotNull] TcpClient tcpClient)
        {
            await Task.Yield();
            var connection = new HConnection(tcpClient);
            _connections.TryAdd(connection.Guid, connection);
            return connection;
        }

        /// <summary>
        /// Sends a message to all connected Clients
        /// </summary>
        /// <param name="message">Message that is being sent</param>
        /// <returns></returns>
        public async Task SendMessageToAllTask([NotNull] byte[] message)
        {
            await Task.Yield();
            var tasks = _connections.Values.Select(connection => connection.SendAyncTask(message));
            await Task.WhenAll(tasks);
        }

        [CanBeNull]
        public HConnection FindHConnectionById(string id)
        {
            return _connections.Values.FirstOrDefault(connection => connection.Guid.ToString() == id);
        }
    }
}
