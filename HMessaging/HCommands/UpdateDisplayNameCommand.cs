﻿using System.Threading.Tasks;
using ChatProtos.Networking;

namespace CoreServer.HMessaging.HCommands
{
    public class UpdateDisplayNameCommand : ICommand
    {
        public async Task Execute(RequestMessage message, HClient client)
        {
            throw new System.NotImplementedException();
        }
    }
}