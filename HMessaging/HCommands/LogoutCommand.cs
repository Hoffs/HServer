using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;

namespace CoreServer.HMessaging.HCommands
{
    public class LogoutServerCommand : IServerCommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            var logoutRequest = LogoutMessageRequest.Parser.ParseFrom(message.Message);
            await client.TryDeauthenticatingTask();
            Console.WriteLine("[SERVER] After logout for client {0}", client.GetDisplayName());
        }
    }
}