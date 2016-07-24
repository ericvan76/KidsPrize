using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Options;

namespace IdentityServer.Services
{
    public class ClientStore : IClientStore
    {
        private readonly IEnumerable<Client> _clients;

        public ClientStore(IOptions<List<ClientOption>> clientOptions)
        {
            _clients = clientOptions.Value.Select(o => new Client()
            {
                ClientId = o.ClientId,
                ClientName = o.ClientName,
                AllowedGrantTypes = o.AllowedGrantTypes,
                RedirectUris = o.RedirectUris.ToList(),
                PostLogoutRedirectUris = o.PostLogoutRedirectUris.ToList(),
                ClientSecrets = o.ClientSecrets.Select(i => new Secret(i.Sha256())).ToList(),
                AllowedScopes = o.AllowedScopes.ToList(),
                RequireConsent = false
            });
        }
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(_clients.FirstOrDefault(i => i.ClientId == clientId));
        }
    }

    public class ClientOption
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string[] AllowedGrantTypes { get; set; }
        public string[] RedirectUris { get; set; }
        public string[] PostLogoutRedirectUris { get; set; }
        public string[] ClientSecrets { get; set; }
        public string[] AllowedScopes { get; set; }
        public bool RequireConsent { get; set; }
    }

}