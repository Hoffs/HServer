using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreServer.Messaging.Authentication
{
    class HAuthenticator
    {
        private AuthenticatorBackend _backend;

        public HAuthenticator(AuthenticatorBackend backend)
        {
            _backend = backend;
        }

        public async Task<AuthenticationResponse> TryPasswordAuthenticationTask(HClient hClient, string password)
        {
            
            return new AuthenticationResponse(true, hClient.Id, hClient.DisplayName, "0000");
        }

        public async Task<AuthenticationResponse> TryTokenAuthenticationTask(HClient hClient, string token)
        {
            return new AuthenticationResponse(true, hClient.Id, hClient.DisplayName, "0000");
        }

        public async Task<bool> DeauthenticateClientTask(HClient hClient)
        {
            return true;
        }

    }

    /// <summary>
    /// Available authentication backends.
    /// </summary>
    enum AuthenticatorBackend
    {
        SQLite,
        PostgreSQL,
        None,
    }

    struct AuthenticationResponse
    {
        public bool Success;
        public string Id;
        public string DisplayName;
        public string Token;

        public AuthenticationResponse(bool success, string id, string displayName, string token)
        {
            Success = success;
            Id = id;
            DisplayName = displayName;
            Token = token;
        }
    }
}
