using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatProtos.Networking;
using CoreServer.Messaging.Authentication;
using Google.Protobuf;

namespace CoreServer
{
    public class HClient
    {
        private readonly TcpClient _tcpClient;
        private object _lock = new object(); // sync lock 
        private readonly HashSet<HChannel> _channels = new HashSet<HChannel>();
        public NetworkStream NetworkStream { get; private set; }
        public string Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public string Token { get; private set; } = string.Empty;
        public bool Authenticated { get; private set; }

        public HClient(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true );
            Id = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes networkStream for the client.
        /// </summary>
        /// <returns></returns>
        public Task HandleConnectionTask()
        {
            return Task.Run(() =>
            {
                NetworkStream = _tcpClient.GetStream();
            });
        }

        /// <summary>
        /// Method to receive the messages from the client 
        /// Usage: var result = await GetMessageTask();
        /// </summary>
        /// <returns>Instance of the message</returns>
        public async Task<string> GetMessageTask()
        {
            var buffer = new byte[4096];
            var byteCount = await NetworkStream.ReadAsync(buffer, 0, buffer.Length);
            var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
            return request;
        }

        public async Task SendMessageToUserTask(ResponseMessage responseMessage)
        {
            try
            {
                Console.WriteLine("[SERVER] Sending message to user of type: {0}", responseMessage.Type);
                var responseBytes = responseMessage.ToByteArray();
                var packet = new byte[4 + responseBytes.Length];

                Buffer.BlockCopy(BitConverter.GetBytes(responseBytes.Length), 0, packet, 0, 4);
                Buffer.BlockCopy(responseBytes, 0, packet, 4, responseBytes.Length);

                await NetworkStream.WriteAsync(packet, 0, packet.Length);
                await NetworkStream.FlushAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetDisplayName()
        {
            if (DisplayName != string.Empty)
            {
                return DisplayName;
            }

            return (Username != string.Empty) ? Username : Id;
        }

        public List<HChannel> GetChannels()
        {
            lock (_lock)
            {
                return _channels.ToList();
            }
        }

        public void AddChannel(HChannel channel)
        {
            lock (_lock)
            {
                try
                {
                    _channels.Add(channel);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("[SERVER] User already in channel.");
                    throw;
                }
            }
        }

        public void RemoveChannel(HChannel channel)
        {
            lock (_lock)
            {
                _channels.Remove(channel);
            }
        }

        public static Predicate<HClient> ByIdPredicate(string id)
        {
            return hClient => hClient.Id == id;
        }

        public static Predicate<HClient> ByDisplayNamePredicate(string displayName)
        {
            return hClient => hClient.DisplayName == displayName;
        }

        public static Predicate<HClient> ByChannel(HChannel channel)
        {
            return hClient => hClient._channels.Contains(channel); // Actually change to use actual channel objects  
        }

        public async Task<Tuple<string, string>> TryAuthenticatingTask(string username, string password = null, string token = null)
        {
            var authenticator = new HAuthenticator(AuthenticatorBackend.None);
            var authenticationResponse = new AuthenticationResponse(false, null, null, null);
            if (password != null)
            {
                authenticationResponse = await authenticator.TryPasswordAuthenticationTask(this, password);
            } else if (token != null)
            {
                authenticationResponse = await authenticator.TryTokenAuthenticationTask(this, token);
            }

            if (authenticationResponse.Success)
            {
                Username = username;
                Id = authenticationResponse.Id;
                DisplayName = authenticationResponse.DisplayName;
                Authenticated = true;
            }
            return Tuple.Create(Id, Token);
        }

        public async Task TryDeauthenticatingTask()
        {
            var authenticator = new HAuthenticator(AuthenticatorBackend.None);
            var success = await authenticator.DeauthenticateClientTask(this);
            if (success)
            {
                await CloseAsync();
            }
        }

        /*
         * Have async methods for establishing a stream
         * getting the messages etc., and call those methods on main server with await.
         * Have list of Channels and have Channels with list of Users (because references are cheap)
         */

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

        public async Task CloseAsync()
        {
            await Task.Yield();
            Close();
        }

        private void Close()
        {
            NetworkStream?.Dispose();
            _tcpClient?.Dispose();
        }
    }
}
