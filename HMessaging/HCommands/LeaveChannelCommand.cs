using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;

namespace CoreServer.HMessaging.HCommands
{
    public class LeaveChannelCommand : ICommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            var joinRequest = LeaveChannelMessageRequest.Parser.ParseFrom(message.Message);
            if (!client.Authenticated)
            {
                Console.WriteLine("[SERVER] User {0} not authenticated to perform this action.",
                    client.GetDisplayName());
            }
            else
            {
                Console.WriteLine("[SERVER] User {0} tried leaving channel {1}.",
                    client.GetDisplayName(), joinRequest.ChannelName);
            }
        }
    }
}