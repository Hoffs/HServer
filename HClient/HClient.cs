using System;
using System.Collections.Generic;
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
        private TcpClient _tcpClient;
        private object _lock = new Object(); // sync lock 
        private List<string> _channels = new List<string>();
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

        public static Predicate<HClient> ByIdPredicate(string id)
        {
            return hClient => hClient.Id == id;
        }

        public static Predicate<HClient> ByDisplayNamePredicate(string displayName)
        {
            return hClient => hClient.DisplayName == displayName;
        }

        public static Predicate<HClient> ByChannelId(string id)
        {
            return hClient => hClient._channels.Contains(id); // Actually change to use actual channel objects  
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

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (_tcpClient == null || _tcpClient.Client == null || !_tcpClient.Client.Connected) return false;
                    /* pear to the documentation on Poll:
                         * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                         * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                         * -or- true if data is available for reading; 
                         * -or- true if the connection has been closed, reset, or terminated; 
                         * otherwise, returns false
                         */

                    // Detect if client disconnected
                    if (!_tcpClient.Client.Poll(0, SelectMode.SelectRead)) return true;
                    byte[] buff = new byte[1];
                    return _tcpClient.Client.Receive(buff, SocketFlags.Peek) != 0;
                }
                catch
                {
                    return false;
                }
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
