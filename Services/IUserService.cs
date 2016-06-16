using System.Linq;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Services
{
    public interface IUserService
    {
        Task<User> FindUserByIdentifier(string issuer, string value, string email);
        Task<User> RegisterUser(string issuer, string value, string email, List<Claim> claims);
        Task UpdateUser(User user, string issuer, string value, List<Claim> claims);
    }

    public class UserService : IUserService
    {
        private readonly KidsPrizeDbContext dbContext;

        public UserService(KidsPrizeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> FindUserByIdentifier(string issuer, string value, string email)
        {
            return await dbContext.Users.Include(u => u.Identifiers).FirstOrDefaultAsync(u => u.Identifiers.Any(i => i.Issuer == issuer && i.Value == value))
                ?? await dbContext.Users.Include(u => u.Identifiers).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> RegisterUser(string issuer, string value, string email, List<Claim> claims)
        {
            var givenName = claims.FirstOrDefault(i => i.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            var familyName = claims.FirstOrDefault(i => i.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
            var displayName = claims.FirstOrDefault(i => i.Type == ClaimTypes.Name)?.Value ?? email.Split('@')[0];

            var identifiers = new List<Identifier>() { new Identifier(0, issuer, value) };
            var user = new User(0, Guid.NewGuid(), email, givenName, familyName, displayName, identifiers, new List<Child>());

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUser(User user, string issuer, string value, List<Claim> claims)
        {
            var givenName = claims.FirstOrDefault(i => i.Type == ClaimTypes.GivenName)?.Value;
            var familyName = claims.FirstOrDefault(i => i.Type == ClaimTypes.Surname)?.Value;
            var displayName = claims.FirstOrDefault(i => i.Type == ClaimTypes.Name)?.Value;

            user.AddIdentifier(issuer, value);
            user.Update(givenName, familyName, displayName);
            await dbContext.SaveChangesAsync();
        }
    }
}
