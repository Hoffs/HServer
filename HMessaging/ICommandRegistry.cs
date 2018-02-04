namespace HServer.HMessaging
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The CommandRegistry interface.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the command.
    /// </typeparam>
    public interface ICommandRegistry<T> where T : class
    {
        /// <summary>
        /// Registers a command using identifier.
        /// </summary>
        /// <param name="identifier">
        /// The command identifier.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        void RegisterCommand([NotNull] HCommandIdentifier identifier, [NotNull] T command);

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [NotNull]
        [ItemCanBeNull]
        Task<T> GetCommand([NotNull] HCommandIdentifier identifier);
    }
}