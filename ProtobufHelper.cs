namespace HServer
{
    using System;

    using Google.Protobuf;

    using JetBrains.Annotations;

    /// <summary>
    /// The protocol buffer helper class.
    /// </summary>
    public static class ProtobufHelper
    {
        /// <summary>
        /// The try parse method for protocol buffer message parser.
        /// </summary>
        /// <param name="parser">
        /// The parser.
        /// </param>
        /// <param name="message">
        /// The message byte string.
        /// </param>
        /// <param name="parsed">
        /// The parsed message.
        /// </param>
        /// <typeparam name="T">
        /// Type of message
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryParse<T>([NotNull] MessageParser<T> parser, [CanBeNull] ByteString message, [NotNull] out T parsed) where T : class, IMessage<T>
        {
            try
            {
                parsed = parser.ParseFrom(message);
                return true;
            }
            catch (InvalidProtocolBufferException)
            {
                Console.WriteLine("[SERVER] Failed to parse protobuf");
                parsed = null;
                return false;
            } 
        }
    }
}