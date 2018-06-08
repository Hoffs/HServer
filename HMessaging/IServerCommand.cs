namespace HServer.HMessaging
{
    using System.Threading.Tasks;

    using global::HServer.Networking;

    using JetBrains.Annotations;

    /// <summary>
    /// The ServerCommand interface.
    /// </summary>
    public interface IServerCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [NotNull]
        Task Execute([NotNull] HConnection client, [NotNull] RequestMessage message);
    }
}
