using System;
using System.Threading.Tasks;
using ChatProtos.Data;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using CoreServer.HChannel;
using Google.Protobuf;

namespace CoreServer.HMessaging.HCommands
{
    public class ChatMessageCommand : ICommand
    {
        private readonly HChannelManager _channelManager;

        public ChatMessageCommand(HChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

        public async Task Execute(RequestMessage message, HClient client)
        {
            var chatMessageRequest = ChatMessageRequest.Parser.ParseFrom(message.Message);
            var chatMessage = chatMessageRequest.Message;
            var channel = _channelManager.FindChannelByName(chatMessageRequest.ChannelId);
            if (channel == null) return;
            Console.WriteLine("[SERVER] Sending message to channel {0}", channel.Id);
            await _channelManager.SendToAllInChannel(channel,
                new ResponseMessage()
                {
                    Type = RequestType.ChatMessage,
                    Message = new ChatMessageResponse()
                    {
                        Message = new ChatMessage()
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