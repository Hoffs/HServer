using System;
using System.Collections.Generic;
using System.Text;

namespace CoreServer
{

    class HChannelManager
    {
        private List<HChannel> _hChannels = new List<HChannel>();
        private Object _lock = new Object();

        public HChannelManager()
        {
            
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
    }
}
