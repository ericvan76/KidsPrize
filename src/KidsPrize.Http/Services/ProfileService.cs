using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using KidsPrize.Http.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Services
{
    public class ProfileService : IProfileService
    {
        private readonly KidsPrizeDbContext _dbContext;

        public ProfileService(KidsPrizeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var uid = Guid.Parse(context.Subject.GetSubjectId());
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Uid == uid);

            var claims = user.Claims();
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

            var uid = Guid.Parse(context.Subject.GetSubjectId());
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Uid == uid);

            context.IsActive = (user != null);// && user.Enabled;

        }
    }
}