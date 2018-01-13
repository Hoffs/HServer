using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CoreServer.HMessaging;

namespace CoreServer
{
    public class HServer
    {
        private readonly TcpListener _listener;
        public IMessageProcessor MessageProcessor { get; }
        public HConnectionManager ConnectionManager { get; } = new HConnectionManager();
        
        public HServer(int port, IMessageProcessor messageProcessor)
        {
            MessageProcessor = messageProcessor;
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.KeepAlive, 
                true);
        }

        public void Run()
        {
            Console.WriteLine("[SERVER] Server started");
            StartListener().Wait();
            Console.WriteLine("[SERVER] Server exiting");
        }

        private async Task StartListener()
        {
            await Task.Yield();
            _listener.Start();
            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("[SERVER] Client connected");
                
                var connection = await ConnectionManager.AddConnectiontTask(tcpClient);
                await InitializeClientTask(connection);

                var task = StartReadingMessagesTask(connection);
                if (task.IsFaulted)
                    task.Wait();
            }
        }

        /// <summary>
        /// Called before starting to read packets from the client.
        /// Useful for initializing client.
        /// </summary>
        /// <param name="connection">HConnection of the client</param>
        /// <returns></returns>
        protected virtual async Task InitializeClientTask(HConnection connection)
        {
            return;
        }

        /// <summary>
        /// Task that constatly reads packets from HConnection.
        /// </summary>
        /// <param name="connection">HConnection from whom to start accepting packets.</param>
        /// <returns></returns>
        protected virtual async Task StartReadingMessagesTask(HConnection connection)
        {
            await Task.Yield();
            Console.WriteLine("[SERVER] Reading messages for tcpclient");
            while (connection.IsConnected())
            {
                var message = await connection.ReadMessageTask();   
                if (message != null && MessageProcessor != null)
                {
                    await MessageProcessor.ProcessMessageTask(connection, message);
                }
            }
        }
    }
}
