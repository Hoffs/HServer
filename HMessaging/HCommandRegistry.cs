using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace CoreServer.HMessaging
{
    public class HCommandRegistry : ICommandRegistry
    {
        private readonly ConcurrentDictionary<HCommandIdentifier, IServerCommand> _commands = new ConcurrentDictionary<HCommandIdentifier, IServerCommand>();

        /// <summary>
        /// Registers a serverCommand to the registry.
        /// </summary>
        /// <param name="identifier">Command identifier</param>
        /// <param name="serverCommand">Class that implements IServerCommand interface</param>
        /// <exception cref="ArgumentException">Thrown if identifier already exists</exception>
        public void RegisterCommand(HCommandIdentifier identifier, IServerCommand serverCommand)
        {
            _commands.TryAdd(identifier, serverCommand);
        }

        [CanBeNull]
        public IServerCommand GetCommand(HCommandIdentifier identifier)
        {
            bool doesExist;
            doesExist = _commands.TryGetValue(identifier, out var serverCommand);
            return doesExist ? serverCommand : null;
        }
    }
}
