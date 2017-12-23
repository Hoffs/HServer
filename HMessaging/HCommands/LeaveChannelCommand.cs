using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using CoreServer.HChannel;

namespace CoreServer.HMessaging.HCommands
{
    public class LeaveChannelCommand : ICommand
    {
        private readonly HChannelManager _channelManager;

        public LeaveChannelCommand(HChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

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
                HChannel.HChannel channel = null;
                if (joinRequest.ChannelId != null)
                {
                    channel = _channelManager.FindChannelById(joinRequest.ChannelId);
                } else if (joinRequest.ChannelName != null && channel == null)
                {
                    channel = _channelManager.FindChannelByName(joinRequest.ChannelName);
                }

                channel?.RemoveClient(client);

                Console.WriteLine("[SERVER] User {0} tried leaving channel {1}.",
                    client.GetDisplayName(), joinRequest.ChannelName);
            }
        }
    }
}