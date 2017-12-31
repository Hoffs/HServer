using System;
using System.Collections.Generic;
using System.Text;
using ChatProtos.Networking;
using CoreServer.HMessaging.HCommands;

namespace CoreServer.HMessaging
{
    public class HCommandRegistry
    {
        private readonly Dictionary<HCommandIdentifier, IServerCommand> _commands = new Dictionary<HCommandIdentifier, IServerCommand>();
        private readonly object _lock = new object(); // sync lock

        /// <summary>
        /// Registers a serverCommand to the registry.
        /// </summary>
        /// <param name="identifier">Command identifier</param>
        /// <param name="serverCommand">Class that implements IServerCommand interface</param>
        /// <exception cref="ArgumentException">Thrown if identifier already exists</exception>
        public void RegisterCommand(HCommandIdentifier identifier, IServerCommand serverCommand)
        {
            lock (_lock)
                _commands.Add(identifier, serverCommand);
        }

        public IServerCommand GetCommand(HCommandIdentifier identifier)
        {
            bool doesExist;
            IServerCommand serverCommand;
            lock (_lock)
                doesExist = _commands.TryGetValue(identifier, out serverCommand);
            
            if (doesExist)
            {
                return serverCommand;
            }
            throw new CommandNotExistsException();
        }

        public static void RegisterDefaultCommands(HCommandRegistry registry, HServer server)
        {
            registry.RegisterCommand(new HCommandIdentifier(RequestType.Login), new LoginServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.Logout), new LogoutServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.JoinChannel), new JoinChannelServerCommand(server.ChannelManager));
            registry.RegisterCommand(new HCommandIdentifier(RequestType.LeaveChannel), new LeaveChannelServerCommand(server.ChannelManager));
            registry.RegisterCommand(new HCommandIdentifier(RequestType.AddRole), new AddRoleServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.RemoveRole), new RemoveRoleServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.BanUser), new BanUserServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.KickUser), new KickUserServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.UserInfo), new UserInfoServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.UpdateDisplayName), new UpdateDisplayNameServerCommand());
            registry.RegisterCommand(new HCommandIdentifier(RequestType.ChatMessage), new ChatMessageServerCommand(server.ChannelManager));
        }
    }

    internal class CommandNotExistsException : Exception
    {
    }
}
