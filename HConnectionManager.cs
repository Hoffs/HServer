namespace HServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The connection manager.
    /// </summary>
    public class HConnectionManager
    {
        /// <summary>
        /// The connections.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Guid, HConnection> _connections = new ConcurrentDictionary<Guid, HConnection>();

        /// <summary>
        /// Adds a Client to the active connection list.
        /// </summary>
        /// <param name="tcpClient">Instance of connecting client</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [ItemNotNull]
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SendMessageToAllTask([NotNull] byte[] message)
        {
            await Task.Yield();
            var tasks = _connections.Values.Select(connection => connection.SendMessageTask(message));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// The find connection by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="HConnection"/>.
        /// </returns>
        [CanBeNull]
        public HConnection FindHConnectionById([CanBeNull] string id)
        {
            return _connections.Values.FirstOrDefault(connection => connection.Guid.ToString() == id);
        }
    }
}
