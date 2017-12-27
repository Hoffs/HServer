using System;
using System.Threading.Tasks;
using ChatProtos.Data;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using Google.Protobuf;

namespace CoreServer.HMessaging.HCommands
{
    public class ChatMessageServerCommand : IServerCommand
    {
        private readonly HChannelManager _channelManager;

        public ChatMessageServerCommand(HChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

        public async Task Execute(RequestMessage message, HClient client)
        {
            var request = ChatMessageRequest.Parser.ParseFrom(message.Message);
            if (request.ChannelId == null || request.Message == null) return;
            var chatMessage = request.Message;
            var channel = _channelManager.FindChannelById(request.ChannelId);
            if (channel == null || !channel.HasClient(client)) return;

            Console.WriteLine("[SERVER] Sending message to channel {0}", channel.Id);
            await channel.SendToAll(
                new ResponseMessage
                {
                    Type = RequestType.ChatMessage,
                    Message = new ChatMessageResponse
                    {
                        Message = new ChatMessage
                        {
                            AuthorId = chatMessage.AuthorId,
                            MessageId = chatMessage.MessageId,
                            Text = chatMessage.Text,
                            Timestamp = chatMessage.Timestamp
                        }
                    }.ToByteString()
                });
        }
    }
}