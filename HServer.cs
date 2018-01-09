using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CoreServer.HMessaging;
using JetBrains.Annotations;

namespace CoreServer
{
    public class HServer
    {
        private readonly TcpListener _listener;
        public IMessageProcessor MessageProcessor { get; }
        public ICommandRegistry CommandRegistry { get; } // Maybe change to private and have a serverCommand in HServer to add commands.
        public HConnectionManager ConnectionManager { get; } = new HConnectionManager();
        
        public HServer(int port, ICommandRegistry commandRegistry, IMessageProcessor messageProcessor)
        {
            MessageProcessor = messageProcessor;
            CommandRegistry = commandRegistry;
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
                
                if (message != null)
                {
                    await MessageProcessor.ProcessMessageTask(connection, message);
                }
            }
        }
    }
}
