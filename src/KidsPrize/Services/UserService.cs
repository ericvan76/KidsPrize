using System.Linq;
using System;
using System.Threading.Tasks;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Services
{
    public interface IUserService
    {
        Task<User> GetUser(Guid uid);
        Task<User> CreateOrUpdateUser(User user);

    }

    public class UserService : IUserService
    {
        private readonly KidsPrizeDbContext _dbContext;

        public UserService(KidsPrizeDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<User> GetUser(Guid uid)
        {
            return await _dbContext.Users
                .Include(u => u.Identifiers)
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Uid == uid);
        }

        public async Task<User> CreateOrUpdateUser(User user)
        {
            var identifier = user.Identifiers.First();
            var existingUser = await _dbContext.Users.Include(u => u.Identifiers).FirstOrDefaultAsync(u => u.Identifiers.Any(i => i.Issuer == identifier.Issuer && i.Value == identifier.Value))
                ?? await _dbContext.Users.Include(u => u.Identifiers).FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return await _dbContext.Users.Include(u => u.Identifiers).FirstAsync(u => u.Uid == user.Uid);
            }
            else
            {
                existingUser.TryAddIdentifier(identifier);
                existingUser.Update(user.GivenName, user.FamilyName, user.DisplayName);
                await _dbContext.SaveChangesAsync();
                return await _dbContext.Users.Include(u => u.Identifiers).FirstAsync(u => u.Uid == existingUser.Uid);
            }

        }
    }
}
