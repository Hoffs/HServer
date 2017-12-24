using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatProtos.Networking;
using CoreServer.HMessaging;
using CoreServer.HMessaging.HCommands;
using Google.Protobuf;

namespace CoreServer
{
    public class HServer
    {
        private readonly TcpListener _listener;
        private readonly HClientManager _clientManager = new HClientManager();
        private readonly HChannelManager _channelManager = new HChannelManager();
        private readonly HMessageProcessor _messageProcessor;
        private const int SizeLimit = 5_000_000;
        private object _lock = new object(); // sync lock
        public HCommandRegistry CommandRegistry { get; } = new HCommandRegistry(); // Maybe change to private and have a command in HServer to add commands.
        
        
        public HServer(int port)
        {
            _messageProcessor = new HMessageProcessor(CommandRegistry);
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.KeepAlive, 
                true);

            _channelManager.CreateChannel("memes");
            Console.WriteLine(_channelManager.FindChannelByName("memes").Id);
        }

        public void RegisterDefaultCommands()
        {
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.Login), new LoginCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.Logout), new LogoutCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.JoinChannel), new JoinChannelCommand(_channelManager));
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.LeaveChannel), new LeaveChannelCommand(_channelManager));
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.AddRole), new AddRoleCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.RemoveRole), new RemoveRoleCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.BanUser), new BanUserCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.KickUser), new KickUserCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.UserInfo), new UserInfoCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.UpdateDisplayName), new UpdateDisplayNameCommand());
            CommandRegistry.RegisterCommand(new HCommandIdentifier(RequestType.ChatMessage), new ChatMessageCommand(_channelManager));
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
                _listener.Start();
                while (true)
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine("[SERVER] Client connected");
                    var hClient = await _clientManager.AddClientTask(tcpClient);

                    if (hClient == null) continue;

                    var task = StartReadingMessagesTask(hClient);
                    if (task.IsFaulted)
                        task.Wait();
                }
            });
        }
        
        /// <summary>
        /// Task that constatly reads packets from HClient.
        /// </summary>
        /// <param name="hClient">HClient from whom to start accepting packets.</param>
        /// <returns></returns>
        private async Task StartReadingMessagesTask(HClient hClient)
        {
            await Task.Yield();
            Console.WriteLine("[SERVER] Reading messages for tcpclient");
            using (var networkStream = hClient.NetworkStream)
            {
                while (hClient.IsConnected())
                {
                    var packetSizeBytes = new byte[4];
                    await networkStream.ReadAsync(packetSizeBytes, 0, 4);
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
                        await _messageProcessor.ProcessMessage(message, hClient);
                        Console.WriteLine("[SERVER] Processed message from Client {0}", hClient.GetDisplayName());
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
        private static async Task DumpDataTask(Stream networkStream, int size)
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
