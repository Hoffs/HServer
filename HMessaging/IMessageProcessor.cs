namespace HServer.HMessaging
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The MessageProcessor interface.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [NotNull]
        Task ProcessMessageTask([NotNull] HConnection connection, [NotNull] byte[] message);
    }
}