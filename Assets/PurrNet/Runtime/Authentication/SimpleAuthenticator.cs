using System.Threading.Tasks;
using PurrNet.Transports;
using UnityEngine;

namespace PurrNet.Authentication
{
    public class SimpleAuthenticator : AuthenticationBehaviour<string>
    {
        [Tooltip("The password required to authenticate the client.")] [SerializeField]
        private string _password = "PurrNet";

        protected override Task<AuthenticationRequest<string>> GetClientPayload()
        {
            return Task.FromResult(new AuthenticationRequest<string>(_password));
        }

        protected override Task<AuthenticationResponse> ValidateClientPayload(Connection conn, string payload)
        {
            return Task.FromResult<AuthenticationResponse>(_password == payload);
        }

        protected override void UnAuthenticateClient(Connection conn) { }
    }
}
