namespace HServer
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The connection.
    /// </summary>
    public class HConnection
    {
        /// <summary>
        /// The message size limit.
        /// </summary>
        private const int SizeLimit = 5_000_000;

        /// <summary>
        /// The TCP client.
        /// </summary>
        [NotNull]
        private readonly TcpClient _tcpClient;

        /// <summary>
        /// The network stream.
        /// </summary>
        [NotNull]
        private readonly NetworkStream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="HConnection"/> class.
        /// </summary>
        /// <param name="client">
        /// The TCP client.
        /// </param>
        public HConnection([NotNull] TcpClient client)
        {
            _tcpClient = client;
            _stream = _tcpClient.GetStream();
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        /// <summary>
        /// Gets the GUID of connection.
        /// </summary>
        public Guid Guid { get; } = Guid.NewGuid(); // Identifies connection

        /// <summary>
        /// Is connection active.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsConnected()
        {
            try
            {
                if (_tcpClient?.Client == null || !_tcpClient.Client.Connected)
                {
                    return false;
                }

                if (!_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    return true;
                }

                var buff = new byte[1];
                return _tcpClient.Client.Receive(buff, SocketFlags.Peek) != 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the message from the connection.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [ItemCanBeNull]
        public async Task<byte[]> ReadMessageTask()
        {
            await Task.Yield();
            var packetSizeBytes = new byte[4];
            await _stream.ReadAsync(packetSizeBytes, 0, 4).ConfigureAwait(false);
            var size = BitConverter.ToInt32(packetSizeBytes, 0);

            if (size > SizeLimit)
            {
                await DumpDataTask(_stream, size).ConfigureAwait(false);
                return null;
            }

            var buffer = new byte[size];
            var byteCount = await _stream.ReadAsync(buffer, 0, size).ConfigureAwait(false);
            
            return byteCount <= 0 ? null : buffer;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SendMessageTask([NotNull] byte[] message)
        {
            try
            {
                var packet = new byte[4 + message.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(message.Length), 0, packet, 0, 4);
                Buffer.BlockCopy(message, 0, packet, 4, message.Length);
                await _stream.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false); // Cancelation token?
                await _stream.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CloseTask()
        {
            await Task.Yield();
            Close();
        }

        /// <summary>
        /// Dumps data from the network stream.
        /// </summary>
        /// <param name="networkStream">
        /// The network stream.
        /// </param>
        /// <param name="size">
        /// The size to dump.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task DumpDataTask([NotNull] Stream networkStream, int size)
        {
            try
            {
                var ms = new MemoryStream();
                await networkStream.CopyToAsync(ms, size).ConfigureAwait(false);
                ms.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        private void Close()
        {
            _tcpClient?.Dispose();
            _stream?.Dispose();
        }
    }
}