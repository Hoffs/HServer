using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChatProtos.Networking;
using ChatProtos.Networking.Messages;
using CoreServer.HMessaging;
using CoreServer.HMessaging.HCommands;
using Google.Protobuf;

namespace CoreServer
{
    class HMessageProcessor
    {
        private HCommandRegistry _commandRegistry;


        public HMessageProcessor(HCommandRegistry registry)
        {
            _commandRegistry = registry;
        }

        /// <summary>
        /// Processes the incoming message and returns the appropriate ResponseMessage.
        /// </summary>
        /// <param name="message">Incoming message.</param>
        /// <param name="hClient">HClient which sent the message.</param>
        /// <returns>Returns ResponseMessage.</returns>
        public async Task ProcessMessage(RequestMessage message, HClient hClient)
        {
            try
            {
                var command = _commandRegistry.GetCommand(new HCommandIdentifier(message.Type));
                Console.WriteLine("[SERVER] Got command {0}", command.ToString());
                await command.Execute(message, hClient);

                /*
                switch (message.Type)
                {                        
                        
                    case RequestType.ChatMessage:
                        
                    case RequestType.UserInfo:
                        
                    default:
                        Console.WriteLine("Type not found or not implemented");
                        break;
                }
                */
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine("[SERVER] Command not implemented.");
            }
            catch (CommandNotExistsException e)
            {
                Console.WriteLine("[SERVER] Command does not exist");
            }
            catch (InvalidProtocolBufferException e)
            {
                Console.WriteLine("[SERVER] Message of type {0} could not be parsed.", message.Type);
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<RequestMessage> MakeResponse()
        {
            return new RequestMessage();
        }
    }
}
