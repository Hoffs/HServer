using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatProtos.Networking;

namespace CoreServer
{
    public class HChannelManager
    {
        private readonly List<HChannel> _hChannels = new List<HChannel>();
        private readonly object _lock = new object();

        public void CreateChannel(string name)
        {
            _hChannels.Add(new HChannel(name));
        }

        public HChannel FindChannelByName(string name)
        {
            lock (_lock)
                return _hChannels.Find(HChannel.ByChannelName(name));
        }

        public HChannel FindChannelById(string id)
        {
            lock (_lock)
                return _hChannels.Find(HChannel.ByChannelId(id));
        }

        public async Task SendToAllInChannel(HChannel channel, ResponseMessage message)
        {
            Console.WriteLine("[SERVER] Sending message to everyone in channe: {0}", channel.Name);
            var tasks = channel.GetClients().Select(async client => await client.SendMessageToUserTask(message));
            await Task.WhenAll(tasks);
        }
    }
}
