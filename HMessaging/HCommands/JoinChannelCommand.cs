using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;

namespace CoreServer.HMessaging.HCommands
{
    public class JoinChannelCommand : ICommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            var joinRequest = JoinChannelMessageRequest.Parser.ParseFrom(message.Message);
            if (!client.Authenticated)
            {
                Console.WriteLine("[SERVER] User {0} not authenticated to perform this action.",
                    client.GetDisplayName());
            }
            else
            {
                Console.WriteLine("[SERVER] User {0} tried joining channel {1}.",
                    client.GetDisplayName(), joinRequest.ChannelName);
            }
        }
    }
}