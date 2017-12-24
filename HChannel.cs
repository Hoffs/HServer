using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ChatProtos.Networking;

namespace CoreServer
{
    public class HChannel
    {
        private readonly HashSet<HClient> _joinedClients = new HashSet<HClient>();
        public string Id { get; private set; }
        public string Name { get; private set; }
        private readonly object _lock = new object();

        public HChannel(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
        }

        public async Task SendToAll(ResponseMessage message)
        {
            Console.WriteLine("[SERVER] Sending message to everyone in channel: {0}", Name);
            var tasks = _joinedClients.Select(async client => await client.SendMessageToUserTask(message));
            await Task.WhenAll(tasks);
        }

        public bool AddClient(HClient hClient)
        {
            lock (_lock) // Add a check for permissions or if was invited.
            {
                try
                {
                    _joinedClients.Add(hClient);
                    return true;
                } catch (ArgumentException)
                {
                    Console.WriteLine("[SERVER] User already in the channel.");
                    return false;
                }
            }

        }

        public bool RemoveClient(HClient hClient)
        {
            lock (_lock)
            {
                if (_joinedClients.Contains(hClient))
                {
                    _joinedClients.Remove(hClient);
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        public bool HasClient(HClient client)
        {
            lock (_lock)
            {
                return _joinedClients.Contains(client);
            }
        }

        public List<HClient> GetClients()
        {
            lock (_lock)
            {
                return _joinedClients.ToList();
            }
        }

        public static Predicate<HChannel> ByChannelName(string name)
        {
            return hChannel => hChannel.Name == name;
        }

        public static Predicate<HChannel> ByChannelId(string id)
        {
            return hChannel => hChannel.Id == id;
        }
    }
}
