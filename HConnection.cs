using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HServer
{
    public class HConnection
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private const int SizeLimit = 5_000_000;
        public Guid Guid { get; } = Guid.NewGuid(); // Identifies connection

        public HConnection(TcpClient client)
        {
            _tcpClient = client;
            _stream = _tcpClient.GetStream();
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket,
                SocketOptionName.KeepAlive,
                true);
        }

        public bool IsConnected()
        {
            try
            {
                if (_tcpClient?.Client == null || !_tcpClient.Client.Connected) return false;
                if (!_tcpClient.Client.Poll(0, SelectMode.SelectRead)) return true;
                var buff = new byte[1];
                return _tcpClient.Client.Receive(buff, SocketFlags.Peek) != 0;
            }
            catch
            {
                return false;
            }
        }

        [ItemCanBeNull]
        public async Task<byte[]> ReadMessageTask()
        {
            await Task.Yield();
            var packetSizeBytes = new byte[4];
            await _stream.ReadAsync(packetSizeBytes, 0, 4);
            var size = BitConverter.ToInt32(packetSizeBytes, 0);
                    
            if (size > SizeLimit)
            {
                await DumpDataTask(_stream, size);
                return null;
            }

            var buffer = new byte[size];
            var byteCount = await _stream.ReadAsync(buffer, 0, size);

            return byteCount <= 0 ? null : buffer;
        }

        public async Task SendAyncTask([NotNull] byte[] message)
        {
            try
            {
                var packet = new byte[4 + message.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(message.Length), 0, packet, 0, 4);
                Buffer.BlockCopy(message, 0, packet, 4, message.Length);
                await _stream.WriteAsync(packet, 0, packet.Length); // Cancelation token?
                await _stream.FlushAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Dumps data to a MemoryStream and then disposes if the size is bigger than limit.
        /// </summary>
        /// <param name="networkStream">Incoming NetworkStream.</param>
        /// <param name="size">Size of the packet to dump.</param>
        /// <returns></returns>
        private static async Task DumpDataTask(Stream networkStream, int size)
        {
            try
            {
                var ms = new MemoryStream();
                await networkStream.CopyToAsync(ms, size);
                ms.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task CloseTask()
        {
            await Task.Yield();
            Close();
        }

        private void Close()
        {
            _tcpClient?.Dispose();
            _stream?.Dispose();
        }
    }
}