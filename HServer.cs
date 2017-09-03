using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatProtos.Networking;
using Google.Protobuf;

namespace CoreServer
{
    public class HServer
    {
        private readonly TcpListener listener;
        private readonly HClientManager _clientManager = new HClientManager();
        private readonly HMessageProcessor _messageProcessor = new HMessageProcessor();
        private const int SizeLimit = 5000000;
        private object _lock = new object(); // sync lock
        
        public HServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Server.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.KeepAlive, 
                true);
        }

        public void Run()
        {
            Console.WriteLine("[SERVER] Server started");
            StartListener().Wait();
            Console.WriteLine("[SERVER] Server exiting");
        }

        private Task StartListener()
        {
            return Task.Run(async () =>
            {
                listener.Start();
                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("[SERVER] Client connected");
                    var hClient = await _clientManager.AddClientTask(tcpClient);

                    if (hClient == null) continue;

                    var task = StartReadingMessagesTask(hClient);
                    if (task.IsFaulted)
                        task.Wait();
                }
            });
        }
        
        private async Task StartReadingMessagesTask(HClient hClient)
        {
            await Task.Yield();
            Console.WriteLine("[SERVER] Reading messages for tcpclient");
            using (var networkStream = hClient.NetworkStream)
            {
                while (hClient.IsConnected)
                {
                    var packetSizeBytes = new byte[4];
                    var packetSize = await networkStream.ReadAsync(packetSizeBytes, 0, 4);
                    var size = BitConverter.ToInt32(packetSizeBytes, 0);
                    
                    if (size > SizeLimit)
                    {
                        await DumpDataTask(networkStream, size);
                        continue;
                    }

                    var buffer = new byte[size];
                    var byteCount = await networkStream.ReadAsync(buffer, 0, size);

                    if (byteCount <= 0) continue;

                    try
                    {
                        var message = RequestMessage.Parser.ParseFrom(buffer);
                        Console.WriteLine("[SERVER] Client {1} wrote protobuf of type: {0}", message.Type, hClient.GetDisplayName());
                        var responseMessage = await _messageProcessor.ProcessMessage(message, hClient);
                    }
                    catch (InvalidProtocolBufferException e)
                    {
                        Console.WriteLine(e);
                        await hClient.CloseAsync();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Dumps data to a MemoryStream and then disposes if the size is bigger than limit.
        /// </summary>
        /// <param name="networkStream">Incoming NetworkStream.</param>
        /// <param name="size">Size of the packet to dump.</param>
        /// <returns></returns>
        private static async Task DumpDataTask(NetworkStream networkStream, int size)
        {
            try
            {
                var ms = new MemoryStream();
                await networkStream.CopyToAsync(ms, size);
                ms.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
