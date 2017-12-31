﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChatProtos.Networking;

namespace CoreServer.HMessaging.HCommands
{
    public interface IServerCommand
    {
        Task Execute(RequestMessage message, HClient client);
    }
}