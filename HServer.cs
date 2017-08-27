using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CoreServer
{
    public class HServer
    {
        private List<HClient> clients = new List<HClient>();
        private TcpListener listener;
        object _lock = new Object(); // sync lock 
        List<Task> _connections = new List<Task>(); // pending connections

        /*
         * Have a processing Class that has a switch case.
         * Handle requests as they come in
         */

        public HServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Server.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.KeepAlive, 
                true);
        }

        public void Run()
        {
            StartListener().Wait();
        }

        private Task StartListener()
        {
            return Task.Run(async () =>
            {
                listener.Start();
                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected");
                    var task = StartHandleConnectionAsync(tcpClient);
                    // if already faulted, re-throw any error on the calling context
                    if (task.IsFaulted)
                        task.Wait();
                }
            });
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            // start the new connection task
            var connectionTask = HandleConnectionAsync(tcpClient);

            // add it to the list of pending task 
            lock (_lock)
                _connections.Add(connectionTask);

            // catch all errors of HandleConnectionAsync
            try
            {
                await connectionTask;
                // we may be on another thread after "await"
            }
            catch (Exception ex)
            {
                // log the error
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                // remove pending task
                lock (_lock)
                    _connections.Remove(connectionTask);
            }
        }

        private async Task HandleConnectionAsync(TcpClient tcpClient)
        {
            await Task.Yield();
            // continue asynchronously on another threads

            using (var networkStream = tcpClient.GetStream())
            {
                while (tcpClient.Connected)
                {

                    var buffer = new byte[4096];
                    Console.WriteLine("[Server] Reading from client. Conncted {0}", tcpClient.Connected);
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("[Server] Client wrote {0}", request);
                }
                // var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");
                // await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                // Console.WriteLine("[Server] Response has been written");
            }
        }
    }
}
