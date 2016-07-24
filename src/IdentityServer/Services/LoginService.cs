using System.Linq;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public interface ILoginService
    {
        Task<bool> ValidateCredentials(string username, string password);
        Task<IdentityUser<Guid>> FindByUsername(string username);
        Task<IdentityUser<Guid>> FindByExternalProvider(string provider, string providerKey);
        Task<IdentityUser<Guid>> FindByEmail(string email);
        Task<IdentityUser<Guid>> AutoProvisionUser(string provider, string providerKey, string email, List<Claim> claims);
    }
    public class LoginService : ILoginService
    {
        private readonly IdentityContext _context;

        public LoginService(IdentityContext context)
        {
            _context = context;
        }

        public Task<bool> ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser<Guid>> FindByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityUser<Guid>> FindByExternalProvider(string provider, string providerKey)
        {
            return await _context.Users.Include(i => i.Roles).Include(i => i.Logins).Include(i => i.Claims)
                .FirstOrDefaultAsync(x => x.Logins.Any(p => p.LoginProvider == provider && p.ProviderKey == providerKey));
        }

        public async Task<IdentityUser<Guid>> FindByEmail(string email)
        {
            return await _context.Users.Include(i => i.Roles).Include(i => i.Logins).Include(i => i.Claims)
                .FirstOrDefaultAsync(x => x.NormalizedEmail == NormalizeEmail(email));
        }

        public async Task<IdentityUser<Guid>> AutoProvisionUser(string provider, string providerKey, string email, List<Claim> claims)
        {
            var filtered = FilterClaims(claims);

            var user = await FindByExternalProvider(provider, providerKey) ?? await FindByEmail(email);
            if (user == null)
            {
                user = new IdentityUser<Guid>()
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    NormalizedEmail = NormalizeEmail(email)
                };
                user.Logins.Add(new IdentityUserLogin<Guid>()
                {
                    LoginProvider = provider,
                    ProviderKey = providerKey
                });
                filtered.ToList().ForEach(i => user.Claims.Add(i));
                _context.Users.Add(user);
            }
            else
            {
                if (!user.Logins.Any(i => i.LoginProvider == provider && i.ProviderKey == providerKey))
                {
                    user.Logins.Add(new IdentityUserLogin<Guid>()
                    {
                        LoginProvider = provider,
                        ProviderKey = providerKey
                    });
                }
                foreach (var claim in filtered)
                {
                    var existing = user.Claims.FirstOrDefault(i => i.ClaimType == claim.ClaimType);
                    if (existing == null)
                    {
                        user.Claims.Add(claim);
                    }
                    else
                    {
                        existing.ClaimValue = claim.ClaimValue;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return user;
        }

        private string NormalizeEmail(string email)
        {
            return email.Trim().ToLower();
        }

        private IEnumerable<IdentityUserClaim<Guid>> FilterClaims(IEnumerable<Claim> claims)
        {
            var filtered = new List<Claim>();
            foreach (var claim in claims)
            {
                if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                else
                {
                    filtered.Add(claim);
                }
            }

            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
            }

            return filtered.Select(i => new IdentityUserClaim<Guid>()
            {
                ClaimType = i.Type,
                ClaimValue = i.Value
            });
        }


    }
}
