using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using CoreServer.HChannel;

namespace CoreServer.HMessaging.HCommands
{
    public class JoinChannelCommand : ICommand
    {
        private readonly HChannelManager _channelManager;

        public JoinChannelCommand(HChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

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
                HChannel.HChannel channel = null;
                if (joinRequest.ChannelId != null)
                {
                    channel = _channelManager.FindChannelById(joinRequest.ChannelId);
                } 
                if (joinRequest.ChannelName != null && channel == null)
                {
                    channel = _channelManager.FindChannelByName(joinRequest.ChannelName);
                }

                channel?.AddClient(client);

                Console.WriteLine("[SERVER] User {0} tried joining channel {1}.",
                    client.GetDisplayName(), joinRequest.ChannelName);
            }
        }
    }
}