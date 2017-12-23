using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreServer.HChannel
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

        public void AddClient(HClient hClient)
        {
            lock (_lock) // Add a check for permissions or 
                _joinedClients.Add(hClient);
        }

        public void RemoveClient(HClient hClient)
        {
            lock (_lock)
                _joinedClients.Remove(hClient);
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
