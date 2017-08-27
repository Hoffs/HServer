using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CoreServer
{
    class HClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private object _lock = new Object(); // sync lock 

        public HClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }
    }
}
