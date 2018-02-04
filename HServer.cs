namespace HServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using global::HServer.HMessaging;

    using JetBrains.Annotations;

    /// <summary>
    /// The server.
    /// </summary>
    public class HServer
    {
        /// <summary>
        /// The TCP listener.
        /// </summary>
        [NotNull]
        private readonly TcpListener _listener;

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        [NotNull]
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// The connection manager.
        /// </summary>
        [NotNull]
        private readonly HConnectionManager connectionManager = new HConnectionManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="HServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The server listening port.
        /// </param>
        /// <param name="messageProcessor">
        /// The message processor.
        /// </param>
        public HServer(int port, [CanBeNull] IMessageProcessor messageProcessor)
        {
            this.messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Run()
        {
            Console.WriteLine("[SERVER] Server started");
            StartListener().Wait();
            Console.WriteLine("[SERVER] Server exiting");
        }

        /// <summary>
        /// Called before starting to read packets from the client.
        /// Useful for initializing client.
        /// </summary>
        /// <param name="connection">HConnection of the client</param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        protected virtual async Task InitializeClientTask([NotNull] HConnection connection)
        {
            return;
        }

        /// <summary>
        /// Starts reading messages from client.
        /// </summary>
        /// <param name="connection">HConnection from whom to start accepting packets.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected virtual async Task StartReadingMessagesTask([NotNull] HConnection connection)
        {
            await Task.Yield();
            Console.WriteLine("[SERVER] Reading messages for tcpclient");
            while (connection.IsConnected())
            {
                var message = await connection.ReadMessageTask().ConfigureAwait(false);   
                if (message != null)
                {
                    await messageProcessor.ProcessMessageTask(connection, message).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Starts server listener.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task StartListener()
        {
            await Task.Yield();
            _listener.Start();
            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                Console.WriteLine("[SERVER] Client connected");
                
                var connection = await connectionManager.AddConnectiontTask(tcpClient).ConfigureAwait(false);
                await InitializeClientTask(connection).ConfigureAwait(false);

                var task = StartReadingMessagesTask(connection);
                if (task.IsFaulted)
                {
                    task.Wait();
                }
            }
        }
    }
}
