using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatProtos.Networking;

/*
 * Helps handling all the clients
 */

namespace CoreServer
{
    public class HClientManager
    {
        private readonly List<HClient> _clients = new List<HClient>();
        private readonly object _lock = new object();

        /// <summary>
        /// Adds a Client to the _clients List if connection is started successfully.
        /// </summary>
        /// <param name="tcpClient">Instance of connecting client</param>
        /// <returns></returns>
        public async Task<HClient> AddClientTask(TcpClient tcpClient)
        {
            var client = new HClient(tcpClient);
            var connectionTask = client.HandleConnectionTask();

            lock (_lock)
                _clients.Add(client);

            try
            {
                await connectionTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                lock (_lock)
                    _clients.Remove(client);
                client = null;
            }
            return client;
        }

        /// <summary>
        /// Sends a message to all connected Clients
        /// </summary>
        /// <param name="message">Message that is being sent</param>
        /// <returns></returns>
        public async Task SendMessageToAllTask(ResponseMessage message)
        {
            await Task.Yield();
            var tasks = new List<Task>();
            lock (_lock)
            {
                foreach (var hClient in _clients)
                {
                    tasks.Add(hClient.SendMessageToUserTask(message));
                }    
            }
            await Task.WhenAll(tasks);
        }

        public HClient FindHClientById(string id)
        {
            lock (_lock)
                return _clients.Find(HClient.ByIdPredicate(id));
        }
    }
}
