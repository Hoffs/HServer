using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CoreServer
{
    internal class HClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private object _lock = new Object(); // sync lock 
        private List<string> _channels = new List<string>();
        public String Id { get; private set; }
        public String DisplayName { get; private set; }

        public HClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        /// <summary>
        /// Initializes networkStream for the client.
        /// </summary>
        /// <returns></returns>
        public Task HandleConnectionTask()
        {
            return Task.Run(() =>
            {
                networkStream = tcpClient.GetStream();
            });
        }

        /// <summary>
        /// Method to receive the messages from the client 
        /// Usage: var result = await GetMessageTask();
        /// </summary>
        /// <returns>Instance of the message</returns>
        public async Task<string> GetMessageTask()
        {
            var buffer = new byte[4096];
            var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
            var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
            return request;
        }

        public async Task SendMessageToUserTask(string message)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await networkStream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static Predicate<HClient> ByIdPredicate(string id)
        {
            return hClient => hClient.Id == id;
        }

        public static Predicate<HClient> ByDisplayNamePredicate(string displayName)
        {
            return hClient => hClient.DisplayName == displayName;
        }

        public static Predicate<HClient> ByChannelId(string id)
        {
            return hClient => hClient._channels.Contains(id); // Actually change to use actual channel objects  
        }
        /*
         * Have async methods for establishing a stream
         * getting the messages etc., and call those methods on main server with await.
         * Have list of Channels and have Channels with list of Users (because references are cheap)
         */
    }
}
