using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CoreServer.HMessaging
{
    public class HCommandRegistry<T> : ICommandRegistry<T> where T : class
    {
        private readonly ConcurrentDictionary<HCommandIdentifier, T> _commands = new ConcurrentDictionary<HCommandIdentifier, T>();

        /// <summary>
        /// Registers a serverCommand to the registry.
        /// </summary>
        /// <param name="identifier">Command identifier</param>
        /// <param name="serverCommand">Class that implements IServerCommand interface</param>
        /// <exception cref="ArgumentException">Thrown if identifier already exists</exception>
        public async Task RegisterCommand(HCommandIdentifier identifier, T serverCommand)
        {
            await Task.Yield();
            _commands.TryAdd(identifier, serverCommand);
        }

        [CanBeNull]
        public async Task<T> GetCommand(HCommandIdentifier identifier)
        {
            await Task.Yield();
            bool doesExist;
            doesExist = _commands.TryGetValue(identifier, out var serverCommand);
            return doesExist ? serverCommand : null;
        }
    }
}
