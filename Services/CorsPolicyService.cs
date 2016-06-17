using System;
using System.Threading.Tasks;
using IdentityServer4.Services;

namespace KidsPrize.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(true);
        }
    }
}