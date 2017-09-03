using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using Google.Protobuf;

namespace CoreServer
{
    class HMessageProcessor
    {
        public HMessageProcessor()
        {
            
        }

        /// <summary>
        /// Processes the incoming message and returns the appropriate ResponseMessage.
        /// </summary>
        /// <param name="message">Incoming message.</param>
        /// <param name="hClient">HClient which sent the message.</param>
        /// <returns>Returns ResponseMessage.</returns>
        public async Task<ResponseMessage> ProcessMessage(RequestMessage message, HClient hClient)
        {
            var responseMessage = new ResponseMessage();
            try
            {
                switch (message.Type)
                {
                    case RequestType.Login:
                        var loginRequest = LoginMessageRequest.Parser.ParseFrom(message.Message);
                        var result = await hClient.TryAuthenticatingTask(loginRequest.Username, loginRequest.Password, loginRequest.Token);
                        Console.WriteLine("[SERVER] After login for client {0}: {1} {2}", hClient.GetDisplayName(), result.Item1, result.Item2);
                        break;
                    case RequestType.Logout:
                        var logoutRequest = LogoutMessageRequest.Parser.ParseFrom(message.Message);
                        await hClient.TryDeauthenticatingTask();
                        Console.WriteLine("[SERVER] After logout for client {0}", hClient.GetDisplayName());
                        break;
                    case RequestType.JoinChannel:
                        var joinRequest = JoinChannelMessageRequest.Parser.ParseFrom(message.Message);
                        if (!hClient.Authenticated)
                        {
                            Console.WriteLine("[SERVER] User {0} not authenticated to perform this action.",
                                hClient.GetDisplayName());
                        }
                        else
                        {
                            Console.WriteLine("[SERVER] User {0} tried joining channel {1}.",
                                hClient.GetDisplayName(), joinRequest.ChanneName);
                        }
                        break;
                    case RequestType.LeaveChannel:
                        
                    case RequestType.AddRole:
                        
                    case RequestType.RemoveRole:
                        
                    case RequestType.KickUser:
                        
                    case RequestType.BanUser:
                        
                    case RequestType.UpdateDisplayName:
                        
                    case RequestType.ChatMessage:
                        
                    case RequestType.UserInfo:
                        
                    default:
                        Console.WriteLine("Type not found or not implemented");
                        break;
                }
                return responseMessage;
            }
            catch (InvalidProtocolBufferException e)
            {
                Console.WriteLine("[SERVER] Message of type {0} could not be parsed.", message.Type);
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<RequestMessage> MakeResponse()
        {
            return new RequestMessage();
        }
    }
}
