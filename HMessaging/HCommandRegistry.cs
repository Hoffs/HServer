namespace HServer.HMessaging
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <inheritdoc />
    /// <summary>
    /// The command registry.
    /// </summary>
    /// <typeparam name="T">
    /// Type of commands
    /// </typeparam>
    public class HCommandRegistry<T> : ICommandRegistry<T> where T : class
    {
        /// <summary>
        /// The commands.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<HCommandIdentifier, T> _commands = new ConcurrentDictionary<HCommandIdentifier, T>();

        /// <inheritdoc />
        public void RegisterCommand(HCommandIdentifier identifier, T serverCommand)
        {
            _commands.TryAdd(identifier, serverCommand);
        }

        /// <inheritdoc />
        public async Task<T> GetCommand(HCommandIdentifier identifier)
        {
            await Task.Yield();
            bool doesExist;
            doesExist = _commands.TryGetValue(identifier, out var serverCommand);
            return doesExist ? serverCommand : null;
        }
    }
}
