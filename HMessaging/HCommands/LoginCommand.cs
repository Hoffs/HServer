using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;

namespace CoreServer.HMessaging.HCommands
{
    public class LoginCommand : ICommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            var loginRequest = LoginMessageRequest.Parser.ParseFrom(message.Message);
            var result = await client.TryAuthenticatingTask(loginRequest.Username, loginRequest.Password, loginRequest.Token);
            Console.WriteLine("[SERVER] After login for client {0}: {1} {2}", client.GetDisplayName(), result.Item1, result.Item2);
        }
    }
}