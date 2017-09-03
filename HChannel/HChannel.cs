using System;
using System.Collections.Generic;
using System.Text;

namespace CoreServer
{
    class HChannel
    {
        private List<HClient> _joinedClients = new List<HClient>();
        public String Id { get; private set; }
        public String Name { get; private set; }
        private Object _lock = new Object();

        public HChannel(string name)
        {
            // Name = name;
            // Id = System.Guid.NewGuid().ToString();
        }

        public void AddClient(HClient hClient)
        {
            lock (_lock) // Add a check for permissions or something
                _joinedClients.Add(hClient);
        }

        public void RemoveClient(HClient hClient)
        {
            lock (_lock)
                _joinedClients.Remove(hClient);
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
