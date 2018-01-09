using System;
using System.Threading.Tasks;
using ChatProtos.Networking;
using Google.Protobuf;

namespace CoreServer.HMessaging
{
    public class HMessageProcessor : IMessageProcessor
    {
        private ICommandRegistry _commandRegistry;

        public HMessageProcessor(ICommandRegistry registry)
        {
            _commandRegistry = registry;
        }

        /// <summary>
        /// Processes the incoming message and returns the appropriate ResponseMessage.
        /// </summary>
        /// <param name="message">Incoming message.</param>
        /// <param name="connection">HClient which sent the message.</param>
        /// <returns>Returns ResponseMessage.</returns>
        public async Task ProcessMessageTask(HConnection connection, byte[] message)
        {
            try
            {
                var requestMessage = RequestMessage.Parser.ParseFrom(message);
                var command = _commandRegistry.GetCommand(new HCommandIdentifier(requestMessage.Type));
                Console.WriteLine("[SERVER] Got serverCommand {0}", command?.ToString());
                if (command != null) await command.Execute(connection, requestMessage);
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine("[SERVER] Command not implemented.");
            }
            catch (InvalidProtocolBufferException e)
            {
                Console.WriteLine("[SERVER] Message of could not be parsed.");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
