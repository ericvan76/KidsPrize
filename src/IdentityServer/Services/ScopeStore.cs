using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;

namespace IdentityServer.Services
{
    public class ScopeStore : IScopeStore
    {
        private readonly IList<Scope> _scopes = new List<Scope>
        {
            StandardScopes.OpenId,
            StandardScopes.Profile,
            StandardScopes.OfflineAccess
        };

        public ScopeStore(IOptions<List<ScopeOption>> scopeOptions)
        {
            scopeOptions.Value.ForEach(i => _scopes.Add(new Scope
            {
                Name = i.Name,
                DisplayName = i.DisplayName,
                Description = i.Description,
                Type = i.Type == "Identity" ? ScopeType.Identity : ScopeType.Resource
            }));
        }

        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(_scopes.Where(i => scopeNames.Contains(i.Name)));
        }

        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            if (publicOnly)
            {
                Task.FromResult(_scopes.Where(i => i.ShowInDiscoveryDocument == true));
            }
            return Task.FromResult(_scopes.AsEnumerable());
        }
    }

    public class ScopeOption
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}