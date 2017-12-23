using System;
using System.Collections.Generic;
using System.Text;
using ChatProtos.Networking;
using CoreServer.HMessaging.HCommands;

namespace CoreServer.HMessaging
{
    public class HCommandRegistry
    {
        private readonly Dictionary<HCommandIdentifier, ICommand> _commands = new Dictionary<HCommandIdentifier, ICommand>();
        private readonly object _lock = new object(); // sync lock

        /// <summary>
        /// Registers a command to the registry.
        /// </summary>
        /// <param name="identifier">Command identifier</param>
        /// <param name="command">Class that implements ICommand interface</param>
        /// <exception cref="ArgumentException">Thrown if identifier already exists</exception>
        public void RegisterCommand(HCommandIdentifier identifier, ICommand command)
        {
            lock (_lock)
                _commands.Add(identifier, command);
        }

        public ICommand GetCommand(HCommandIdentifier identifier)
        {
            bool doesExist;
            ICommand command;
            lock (_lock)
                doesExist = _commands.TryGetValue(identifier, out command);
            
            if (doesExist)
            {
                return command;
            }
            throw new CommandNotExistsException();
        }
    }

    internal class CommandNotExistsException : Exception
    {
    }
}
