using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IdentityContext _context;

        public ProfileService(IdentityContext context)
        {
            _context = context;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var id = Guid.Parse(context.Subject.GetSubjectId());
            var user = await _context.Users.Include(i => i.Claims).FirstOrDefaultAsync(u => u.Id == id);

            var claims = user.Claims.Select(i => new Claim(i.ClaimType, i.ClaimValue));
            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
            }
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var id = Guid.Parse(context.Subject.GetSubjectId());
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            context.IsActive = (user != null);// && user.Enabled;

        }
    }
}