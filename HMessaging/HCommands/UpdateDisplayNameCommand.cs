﻿using System.Threading.Tasks;
using ChatProtos.Networking;

namespace CoreServer.HMessaging.HCommands
{
    public class UpdateDisplayNameServerCommand : IServerCommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            throw new System.NotImplementedException();
        }
    }
}