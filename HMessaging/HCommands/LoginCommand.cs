using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using Google.Protobuf;

namespace CoreServer.HMessaging.HCommands
{
    public class LoginServerCommand : IServerCommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            var loginRequest = LoginMessageRequest.Parser.ParseFrom(message.Message);
            var result = await client.TryAuthenticatingTask(loginRequest.Username, loginRequest.Password, loginRequest.Token);
            Console.WriteLine("[SERVER] After login for client {0}: {1} {2}", client.GetDisplayName(), result.Item1, result.Item2);
            if (client.Authenticated)
            {
                await client.SendMessageToUserTask(new ResponseMessage
                {
                    Status = ResponseStatus.Success,
                    Type = RequestType.Login,
                    Message = new LoginMessageResponse
                    {
                        Token = client.Token,
                        UserId = client.Id
                    }.ToByteString()
                });
            }
            else
            {
                await client.SendMessageToUserTask(new ResponseMessage
                {
                    Status = ResponseStatus.Error,
                    Type = RequestType.Login
                });
            }
        }
    }
}